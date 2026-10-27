using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public bool m_Key = false;

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

    [Header("Damage Feedback")]
    public Image m_DamageFlashImage;
    public float m_DamageFlashDuration = 0.4f;
    public float m_DamageFlashAlpha = 0.45f;
    Coroutine m_DamageFlashCoroutine;

    [Header("Player Life")]
    public int m_MaxLife = 100;
    private int m_Life;

    public TMP_Text m_LifeText;



    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();

        m_MaxLife = Mathf.Max(1, m_MaxLife);
        m_Life = m_MaxLife;

        Debug.Log(
            "VIDA INICIALIZADA: " + m_Life +
            " | Jugador: " + name +
            " | ID: " 
        );
    }


    private void Start()
    {
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchController.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        m_HitParticlesPool = new CPoolElements(10, m_HitPartcilesPrefab);

        if (m_MaxLife <= 0)
        {
            m_MaxLife = 100;
        }

        m_Life = m_MaxLife;

        UpdateLifeUI();

        Debug.Log("VIDA INICIALIZADA: " + m_Life);

        if (m_Animation != null)  // per a que funcionin les animacions. 
        {
            if (m_IdleAnimation != null)
            {
                m_Animation.AddClip(m_IdleAnimation, m_IdleAnimation.name);
            }

            if (m_ShootAnimation != null)
            {
                m_Animation.AddClip(m_ShootAnimation, m_ShootAnimation.name);
            }

            if (m_ReloadAnimation != null)
            {
                m_Animation.AddClip(m_ReloadAnimation, m_ReloadAnimation.name);
            }
        }


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

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }

        void TryOpenDoor()
        {
            Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, 3.0f))
            {
                if (l_RaycastHit.collider.CompareTag("Door"))
                {
                    if (m_Key)
                    {
                        l_RaycastHit.collider.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.Log("Necessites la clau per obrir la porta.");
                    }
                }
            }
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

    public void TakeDamage(int damage)
    {
        Debug.Log(
            "DAÑO RECIBIDO: " + damage +
            " | VIDA ANTES: " + m_Life
        );

        // Si el jugador ya no tiene vida, no aplicar más daño.
        if (m_Life <= 0)
        {
            Debug.LogWarning("El jugador ya tiene 0 de vida.");
            return;
        }

        // Restar el daño sin permitir vida negativa.
        damage = Mathf.Max(0, damage);
        m_Life = Mathf.Max(0, m_Life - damage);

        // Actualizar el texto del HUD.
        UpdateLifeUI();

        Debug.Log("VIDA DESPUÉS: " + m_Life);

        // Mostrar el efecto rojo si la imagen está asignada.
        if (m_DamageFlashImage != null)
        {
            if (m_DamageFlashCoroutine != null)
            {
                StopCoroutine(m_DamageFlashCoroutine);
            }

            m_DamageFlashCoroutine =
                StartCoroutine(DamageFlashCoroutine());
        }
        else
        {
            Debug.LogWarning(
                "Falta asignar Damage Flash Image en el Inspector."
            );
        }
    }

    void UpdateLifeUI()
    {
        if (m_LifeText == null)
        {
            Debug.LogError("Life Text no está asignado.", this);
            return;
        }

        m_LifeText.text =
            "Life Points\n" + m_Life + "/" + m_MaxLife;
    }

    System.Collections.IEnumerator DamageFlashCoroutine()
    {
        float l_Duration = Mathf.Max(0.01f, m_DamageFlashDuration);
        float l_Elapsed = 0f;

        while (l_Elapsed < l_Duration)
        {
            l_Elapsed += Time.deltaTime;

            float l_Alpha = Mathf.Lerp(
                m_DamageFlashAlpha,
                0f,
                Mathf.Clamp01(l_Elapsed / l_Duration)
            );

            m_DamageFlashImage.color = new Color(
                1f, 0f, 0f, l_Alpha
            );

            yield return null;
        }
        m_DamageFlashImage.color = new Color(1f, 0f, 0f, 0f);

        m_DamageFlashCoroutine = null;
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
        }
        if (other.CompareTag("DeadZone"))
        {
            Kill();
        }
        if (other.CompareTag("Key"))
        {
            m_Key = true;
            other.gameObject.SetActive(false); 
        }
    }
    void Kill()
    {
        //yield return new WaitForSeconds(1.5f);
        //GameController.GetGameController().Restart();
        StartCoroutine(GameController.GetGameController().GetFade().FadeIn(() => {
            GameController.GetGameController().GetFade().gameObject.SetActive(false);
            GameController.GetGameController().Restart();
        }));
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
        GameController l_GameController =
            GameController.GetGameController();

        if (l_GameController == null ||
            l_GameController.GetIngameUI() == null ||
            m_Camera == null)
        {
            return;
        }

        LifeBarElementUI l_LifeBarElementUI =
            l_GameController.GetIngameUI().m_LifeBarElementUI;

        if (l_LifeBarElementUI == null)
        {
            return;
        }

        RectTransform l_ParentRect =
            l_LifeBarElementUI.transform.parent
                as RectTransform;

        Canvas l_Canvas =
            l_LifeBarElementUI.GetComponentInParent<Canvas>();

        if (l_ParentRect == null || l_Canvas == null)
        {
            return;
        }

        // En Screen Space Overlay no se necesita una cámara de UI.
        Camera l_UICamera =
            l_Canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : (l_Canvas.worldCamera != null
                    ? l_Canvas.worldCamera
                    : m_Camera);

        // Ocultar la barra hasta encontrar un enemigo válido.
        l_LifeBarElementUI.Hide();

        List<DroneEnemy> l_Enemies =
            l_GameController.GetEnemies();

        foreach (DroneEnemy l_Enemy in l_Enemies)
        {
            if (l_Enemy == null ||
                !l_Enemy.gameObject.activeInHierarchy ||
                l_Enemy.GetLife() <= 0 ||
                l_Enemy.m_LifeBarPosition == null ||
                l_Enemy.m_MaxLife <= 0)
            {
                continue;
            }

            // Posición del ancla del enemigo en pantalla.
            Vector3 l_ScreenPosition =
                m_Camera.WorldToScreenPoint(
                    l_Enemy.m_LifeBarPosition.position
                );

            // Ignorar enemigos detrás de la cámara o fuera de pantalla.
            if (l_ScreenPosition.z <= 0f ||
                l_ScreenPosition.x < 0f ||
                l_ScreenPosition.x > Screen.width ||
                l_ScreenPosition.y < 0f ||
                l_ScreenPosition.y > Screen.height)
            {
                continue;
            }

            // Convertir coordenadas de pantalla a coordenadas del Canvas.
            Vector2 l_LocalPosition;

            bool l_PositionIsValid =
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    l_ParentRect,
                    l_ScreenPosition,
                    l_UICamera,
                    out l_LocalPosition
                );

            if (!l_PositionIsValid)
            {
                continue;
            }

            float l_NormalizedLife =
                l_Enemy.GetLife() / (float)l_Enemy.m_MaxLife;

            l_LifeBarElementUI.Show(
                l_LocalPosition,
                l_NormalizedLife
            );

            // Un único elemento de barra reutilizable.
            break;
        }
    }
}
