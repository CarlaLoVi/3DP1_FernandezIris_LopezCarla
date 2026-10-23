using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class FPSController : MonoBehaviour
{
    bool m_AngleLocked = false;

    public Transform m_PitchController;

    float m_Yaw;
    float m_Pitch;
    public float m_Yawspeed;
    public float m_Pitchspeed;
    public bool m_InvertPitch;
    public bool m_InvertYaw;
    public float m_PitchMin;
    public float m_PitchMax;

    CharacterController m_CharacterController;

    public float m_Speed;
    public float m_VerticalSpeed = 0.0f;
    public float m_JumpSpeed = 5.0f;
    public float m_DashSpeed = 10.0f;

    Vector2 m_StartPosition;
    Quaternion m_StartRotation;



    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_SprintKeyCode = KeyCode.LeftShift;
    public KeyCode m_DebugLockAngleCode = KeyCode.I;
    public KeyCode m_DebugLockCursorCode = KeyCode.O;
    public KeyCode m_ReloadKeyCode = KeyCode.R;


    [Header("Shoot")]
    public Camera m_Camera;
    public int m_ShootButton = 0;
    public float m_MaxShootDistance = 200.0f;
    public LayerMask m_ShootLayerMask;
    public GameObject m_HitPartcilesPrefab;
    public CPoolElements m_HitParticlesPool;

    [Header("Weapon Animation")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimation;
    public AnimationClip m_ShootAnimation;
    public AnimationClip m_ReloadAnimation;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }


    private void Start()
    {
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchController.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        m_HitParticlesPool = new CPoolElements(10, m_HitPartcilesPrefab);
        SetIdWeaponAnimation();
        GameController.GetGameController().SetPlayer(this);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_DebugLockAngleCode))
        {
            m_AngleLocked = !m_AngleLocked;
        }
        if (Input.GetKeyDown(m_DebugLockCursorCode))
        {
            /*if (Cursor.lockState != CursorLockMode.Locked)
             {
                 Cursor.lockState = CursorLockMode.Locked;
                 Cursor.visible = false;
             }
             else
             {
                 Cursor.lockState = CursorLockMode.None;
                 Cursor.visible = true;
             }*/

            Cursor.lockState = (Cursor.lockState != CursorLockMode.Locked) ? CursorLockMode.Locked : CursorLockMode.None;
        }

        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        if (m_InvertPitch)
        {
            l_MouseX = -l_MouseX;
        }
        if (m_InvertYaw)
        {
            l_MouseY = -l_MouseY;
        }

        if (!m_AngleLocked)
        {
            m_Yaw = m_Yaw + l_MouseX * m_Yawspeed * Time.deltaTime;
            m_Pitch = m_Pitch + l_MouseY * m_Pitchspeed * Time.deltaTime;
        }

        m_Pitch = Mathf.Clamp(m_Pitch, m_PitchMin, m_PitchMax);

        transform.rotation = Quaternion.Euler(0, m_Yaw, 0);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0, 0);

        Vector3 l_Direction = Vector3.zero;
        Vector3 l_Right = transform.right;
        Vector3 l_Forward = transform.forward;
        l_Right.y = 0.0f;
        l_Forward.y = 0.0f;
        l_Right.Normalize();
        l_Forward.Normalize();



        if (Input.GetKey(m_RightKeyCode))
        {
            l_Direction = l_Right;
        }
        else if (Input.GetKey(m_LeftKeyCode))
        {
            l_Direction = -l_Right;
        }

        if (Input.GetKey(m_UpKeyCode))
        {
            l_Direction += l_Forward;
        }
        if (Input.GetKey(m_DownKeyCode))
        {
            l_Direction -= l_Forward;
        }


        float l_Speed = m_Speed;
        if (Input.GetKey(m_SprintKeyCode))
        {
            l_Speed = m_DashSpeed;
        }
        if (Input.GetKeyDown(m_JumpKeyCode) && (m_VerticalSpeed < 0.2f && m_VerticalSpeed > -0.2f)/*m_CharacterController.isGrounded*/)
        {
            Debug.Log("jump");
            m_VerticalSpeed = m_JumpSpeed;
        }

        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;

        l_Direction.Normalize();
        l_Direction = l_Direction * l_Speed * Time.deltaTime;
        l_Direction.y = m_VerticalSpeed * Time.deltaTime;
        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Direction);  //CollisionFlags variable tipo mascara binaria si colisiona suelo arriba lateral... 

        if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)  // SI colisiono con el suelo y la velocidad es distinta a 0 vertical speed es 0
        {
            m_VerticalSpeed = 0.0f;

        }
        else if ((l_CollisionFlags & CollisionFlags.CollidedAbove) != 0 && m_VerticalSpeed > 0.0f)  // Si colisiono por arriba i la velocidad es ascendiente, la velocidad es 0.
        {
            m_VerticalSpeed = 0.0f;
        }

        //disparo implementación

        if (CanShoot() && MustShoot())
        {
            Shoot();
        }

        if (CanReload() && MustReload())
        {
            Reload();
        }

        UpdateLifeBarEnemies();

        bool CanReload()
        {
            return true;
        }
        bool MustReload()
        {
            return Input.GetKeyDown(m_ReloadKeyCode);
        }

        void Reload()
        {
            SetreloadWeaponAnimation();
        }

        bool CanShoot()
        {
            return true;
        }

        bool MustShoot()
        {
            return Input.GetMouseButtonDown(m_ShootButton);
        }

        // En el raycast definimos distancia de la bala. Podemos jugar con la fisica con las balas. 
        void Shoot()
        {
            SetShootWeaponAnimation();
            Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxShootDistance, m_ShootLayerMask.value))
            {
                if (l_RaycastHit.collider.CompareTag("HitCollider"))
                {
                    l_RaycastHit.collider.GetComponent<HitCollider>().Hit();
                }
                else
                {
                    CreateShoothitparticles(l_RaycastHit.point, l_RaycastHit.normal);
                }                
            }
        }

        void CreateShoothitparticles(Vector3 position, Vector3 normal)
        {
            GameObject l_GameObject = m_HitParticlesPool.GetNextElement();
            l_GameObject.SetActive(true);
            l_GameObject.transform.position = position;
            l_GameObject.transform.rotation = Quaternion.LookRotation(normal);
        }
    }
    void SetIdWeaponAnimation()
    {
        m_Animation.CrossFade(m_IdleAnimation.name, 0.1f);
    }
    void SetShootWeaponAnimation()
    {
        m_Animation.CrossFade(m_ShootAnimation.name, 0.1f);
        m_Animation.CrossFadeQueued(m_IdleAnimation.name, 0.3f);
    }
    void SetreloadWeaponAnimation()
    {
        m_Animation.CrossFade(m_ReloadAnimation.name, 0.1f);
        m_Animation.CrossFadeQueued(m_IdleAnimation.name, 0.3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            var l_Item = other.GetComponent<Item>();
            if (l_Item.CanPick())
            {
                l_Item.Pick();
            }
            else if (other.CompareTag("DeadZone"))
            {
                StartCoroutine(Kill());
            }
        }
    }
    IEnumerator Kill()
    {
        yield return new WaitForSeconds(1.5f);
        GameController.GetGameController().Restart();

    }

    public void Restart()
    {
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }

    void UpdateLifeBarEnemies()
    {
        GameController l_GameController = GameController.GetGameController();
        List<DroneEnemy> l_Enemies = l_GameController.GetEnemies();

        LifeBarElementUI l_LifeBarElementUI = l_GameController.GetIngameUI().m_LifeBarElementUI;
        l_LifeBarElementUI.Hide();

        foreach(DroneEnemy l_Enemy in l_Enemies)
        {
            Vector3 l_ViewporPoint = m_Camera.WorldToViewportPoint(l_Enemy.m_LifeBarPosition.position);
            bool l_EnemyIsVisible = l_ViewporPoint.z>=0.0f && l_ViewporPoint.x >=0f && l_ViewporPoint.x<=1.0f && l_ViewporPoint.y >=0.0f && l_ViewporPoint.y <= 1.0;
            
            if (l_EnemyIsVisible)
            {
                Vector2 l_Position = new Vector2(l_ViewporPoint.x * 1920.0f, l_ViewporPoint.y * 1080.0f);
                l_LifeBarElementUI.Show(l_Position, l_Enemy.GetLife() / (float)l_Enemy.m_MaxLife);
                break;
            }
        }
    }
}
