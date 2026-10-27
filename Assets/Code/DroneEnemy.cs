using NUnit.Framework;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class DroneEnemy : MonoBehaviour
{
    enum TState
    {
        IDLE = 0,
        PATROL,
        ALERT,
        CHASE,
        ATTACK,
        HIT,
        DIE
    }

    NavMeshAgent m_NavMeshAgent;
    TState m_CurrentState = TState.IDLE;

    [Header("Patrol")]
    public List<Transform> m_Waypoints;
    int m_CurrentPatrolId;


    [Header("Distances")]
    public float m_MinDistanceToAttack = 5.0f;
    public float m_MaxEarsDistance = 10.0f;
    public float m_MaxChaseDistance = 10.0f;
    public float m_ShootMaxDistance = 5.0f;

    [Header("Life")]
    public int m_MaxLife = 50;
    int m_Life = 50;

    [Header("Attack")]
    public int m_Damage = 10;
    public float m_AttackTime = 2.0f;
    private float m_CurrentAttackTime = 0.0f;

    [Header("Alert")]
    public float m_AlertAngle = 360.0f;
    private float m_AcumulatedAlertAngle = 0.0f;

    [Header("Sight")]
    public float m_SightAngle = 60.0f;
    public LayerMask m_SightLayerMask;
    public float m_EyesHeight = 1.8f;
    public float m_SightDistance = 10.0f;

    [Header("LifeBar")]
    public Transform m_LifeBarPosition;
    public LifeBarElementUI m_LifeBarElementUI;
    public float m_ShowLifeBarDistance = 15.0f;
    bool m_LifeBarVisible;

    [Header("Dead & Fade")]
    public List<MeshRenderer> m_MeshRenderers;
    public float m_DeadTime = 1.5f;
    private float m_CurrentTime;
    private bool m_Die;

    [Header("Hits")]
    public int m_HelixHitColliderAmount = 10;
    public int m_HeadHitColliderAmount = 50;
    public int m_BodyHitColliderAmount = 20;

    [Header("Sound")]
    public AudioSource m_AudioSource;
    public AudioClip m_ShootSound;


    private Transform m_PlayerTransform;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        m_Life = m_MaxLife;
        m_Die = false;
        m_LifeBarVisible = true;

        InitFade();

        if (GameController.GetGameController() != null)
        {
            GameController.GetGameController().AddEnemy(this);
        }

        FindPlayer();
        if (m_Waypoints != null && m_Waypoints.Count > 0)
        {
            SetPatrolState();
        }
        else
        {
            SetIdleState();
        }
        
    }

    private void Update()
    {
        switch (m_CurrentState)
        {
            case TState.IDLE:
                UpdateIdleState();
                break;
            case TState.ALERT:
                UpdateAlertState();
                break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;
            case TState.CHASE:
                UpdateChaseState();
                break;
            case TState.HIT:
                UpdateHitState();
                break;
            case TState.DIE:
                UpdateDieState();
                break;
        }

        if (m_PlayerTransform != null)
        {
            float l_PlayerDistance = Vector3.Distance(m_PlayerTransform.position, transform.position);
            m_LifeBarVisible = l_PlayerDistance < m_ShowLifeBarDistance;
        }

        UpdateLifeBarUI();
    }

    public void InitFade()
    {
        if (m_MeshRenderers == null) return;

        foreach (MeshRenderer l_MeshRender in m_MeshRenderers)
        {
            if (l_MeshRender != null && l_MeshRender.sharedMaterial != null)
            {
                l_MeshRender.material = Material.Instantiate(l_MeshRender.sharedMaterial);
            }
        }
    }

    public void SetFadeValue(float Pct)
    {
        if (m_MeshRenderers == null) return;

        foreach (MeshRenderer l_MeshRender in m_MeshRenderers)
        {
            if (l_MeshRender != null && l_MeshRender.material != null)
            {
                l_MeshRender.material.SetFloat("_Cutoff", Pct);
            }
        }
    }



    //Màquines d'estat
    void SetIdleState()
    {
        m_CurrentState=TState.IDLE;
        SetFadeValue(0.0f);
    }

    void UpdateIdleState()
    {
        SetPatrolState();
    }


    void SetAlertState()
    {
        m_CurrentState = TState.ALERT;
        m_AcumulatedAlertAngle = 0.0f;

        if (m_NavMeshAgent != null)
        {
            m_NavMeshAgent.isStopped = true;
        }
    }

    void UpdateAlertState()
    {
        float l_Rotation = m_AlertAngle * Time.deltaTime;
        float l_RemainingAngle = 360.0f - m_AcumulatedAlertAngle;

        if (l_RemainingAngle > 0.0f)
        {
            float l_ApplyRotation = Mathf.Min(l_Rotation, l_RemainingAngle);
            transform.Rotate(Vector3.up, l_ApplyRotation);
            m_AcumulatedAlertAngle += l_ApplyRotation;
        }

        if (SeesPlayer())
        {
            if (m_NavMeshAgent != null) m_NavMeshAgent.isStopped = false;
            float l_PlayerDistance = Vector3.Distance(GetPlayerPosition(), transform.position);

            if (l_PlayerDistance > m_MinDistanceToAttack)
                SetChaseState();
            else
                SetAttackState();

            return;
        }

        if (m_AcumulatedAlertAngle >= 360.0f)
        {
            if (m_NavMeshAgent != null) m_NavMeshAgent.isStopped = false;
            SetPatrolState();
        }
    }

    void SetHitState()
    {
        m_CurrentState = TState.HIT;
    }

    void UpdateHitState()
    {
        SetAlertState();
    }

    void SetAttackState()
    {
        m_CurrentState = TState.ATTACK;
        if (m_NavMeshAgent != null) m_NavMeshAgent.isStopped = true;
    }

    void UpdateAttackState()
    {
        Vector3 l_PlayerPosition = GetPlayerPosition();
        float l_Distance = Vector3.Distance(l_PlayerPosition, transform.position);

        if (l_Distance < m_ShootMaxDistance)
        {
            // per a que apunti al jugador en la rotació horitzontal
            Vector3 l_TargetPosition = new Vector3(l_PlayerPosition.x, transform.position.y, l_PlayerPosition.z);
            Vector3 l_Direction = (l_TargetPosition - transform.position).normalized;
            if (l_Direction != Vector3.zero)
            {
                transform.forward = l_Direction;
            }

            // Temporizador de ataque
            m_CurrentAttackTime += Time.deltaTime;
            if (m_CurrentAttackTime >= m_AttackTime)
            {
                PlayShootSound();
                ApplyDamageToPlayer();
                m_CurrentAttackTime = 0.0f;
            }
        }
        else
        {
            SetChaseState();
        }
    }

    void SetPatrolState()
    {
        m_CurrentState = TState.PATROL;
        if (m_NavMeshAgent != null)
        {
            m_NavMeshAgent.isStopped = false;
        }

        if (m_Waypoints != null && m_Waypoints.Count > 0)
        {
            m_NavMeshAgent.SetDestination(m_Waypoints[m_CurrentPatrolId].position);
        }
    }

    void UpdatePatrolState()
    {
        if (m_Waypoints == null || m_Waypoints.Count == 0)
        {
            return;
        }

        if (!m_NavMeshAgent.pathPending && m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
        {
            MoveToNextPatrolPoint();
        }

        if (SeesPlayer())
        {
            SetChaseState();
        }
        else if (HearsPlayer())
        {
            SetChaseState();
        }

    }

    void SetChaseState()
    {
        m_CurrentState = TState.CHASE;
        if (m_NavMeshAgent != null) m_NavMeshAgent.isStopped = false;
    }

    void UpdateChaseState()
    {
        Vector3 l_PlayerPos = GetPlayerPosition();
        float l_Distance = Vector3.Distance(l_PlayerPos, transform.position);

        if (l_Distance > m_MaxChaseDistance)
        {
            SetPatrolState();
            return;
        }

        SetNextChasePosition();

        if (l_Distance <= m_MinDistanceToAttack)
        {
            SetAttackState();
        }

    }

    void SetDieState()
    {
        m_CurrentState = TState.DIE;
        if (m_NavMeshAgent != null) m_NavMeshAgent.isStopped = true;
        m_CurrentTime = 0f;

        if (m_LifeBarElementUI != null)
        {
            m_LifeBarElementUI.Hide();
        }
    }

    void UpdateDieState()
    {
        m_CurrentTime += Time.deltaTime;
        float l_Pct = Mathf.Min(1.0f, m_CurrentTime / m_DeadTime);
        SetFadeValue(l_Pct);

        if (l_Pct >= 1.0f && !m_Die)
        {
            m_Die = true;
            gameObject.SetActive(false);
        }
    }



    public void MoveToNextPatrolPoint()
    {
        if (m_Waypoints == null || m_Waypoints.Count == 0) return;

        m_CurrentPatrolId = (m_CurrentPatrolId + 1) % m_Waypoints.Count;
        m_NavMeshAgent.SetDestination(m_Waypoints[m_CurrentPatrolId].position);
    }

    public void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GetPlayerPosition();
        Vector3 l_Direction = (l_PlayerPosition - transform.position).normalized;
        Vector3 l_Position = l_PlayerPosition - l_Direction * m_MinDistanceToAttack;
        m_NavMeshAgent.SetDestination(l_Position);
    }



    public bool SeesPlayer()
    {
        if (m_PlayerTransform == null && !FindPlayer()) return false;

        Vector3 l_EyePosition = transform.position + Vector3.up * m_EyesHeight;
        Vector3 l_PlayerTargetPos = m_PlayerTransform.position + Vector3.up * 1.0f; // Apunta al centro del cuerpo
        Vector3 l_Direction = (l_PlayerTargetPos - l_EyePosition).normalized;
        float l_Distance = Vector3.Distance(l_EyePosition, l_PlayerTargetPos);

        if (l_Distance > m_SightDistance) return false;

        // Comprobación de cono de visión
        if (Vector3.Angle(transform.forward, l_Direction) <= m_SightAngle * 0.5f)
        {
            // Dibujar línea de depuración en la ventana Scene (Verde = Ve al player)
            Debug.DrawLine(l_EyePosition, l_PlayerTargetPos, Color.green);

            RaycastHit[] l_Hits = Physics.RaycastAll(l_EyePosition, l_Direction, m_SightDistance);
            System.Array.Sort(l_Hits, (x, y) => x.distance.CompareTo(y.distance));

            foreach (RaycastHit l_Hit in l_Hits)
            {
                // Ignorar piezas del propio dron
                if (l_Hit.transform == transform || l_Hit.transform.IsChildOf(transform)) continue;

                // Detectar si impacta en el Jugador
                if (l_Hit.collider.CompareTag("Player") || l_Hit.transform.IsChildOf(m_PlayerTransform))
                {
                    return true;
                }

                // Si impacta con pared o terreno antes que con el jugador -> No lo ve
                break;
            }
        }
        else
        {
            Debug.DrawLine(l_EyePosition, l_EyePosition + transform.forward * m_SightDistance, Color.red);
        }

        return false;
    }


    public bool HearsPlayer()
    {
        if (m_PlayerTransform == null && !FindPlayer()) return false;

        float l_Distance = Vector3.Distance(m_PlayerTransform.position, transform.position);
        return l_Distance < m_MaxEarsDistance;
    }


    public bool FindPlayer()
    {
        GameObject l_Player = GameObject.FindGameObjectWithTag("Player");
        if (l_Player != null)
        {
            m_PlayerTransform = l_Player.transform;
            return true;
        }
        return false;
    }

    public Vector3 GetPlayerPosition()
    {
        if (m_PlayerTransform == null) FindPlayer();
        return m_PlayerTransform != null ? m_PlayerTransform.position : transform.position;
    }



    void ApplyDamageToPlayer()
    {
        FPSController l_PlayerController = null;

        // Primero, buscar en el objeto del jugador.
        if (m_PlayerTransform != null)
        {
            l_PlayerController =
                m_PlayerTransform.GetComponent<FPSController>();

            // Si no está ahí, buscar en los padres.
            if (l_PlayerController == null)
            {
                l_PlayerController =
                    m_PlayerTransform.GetComponentInParent<FPSController>();
            }

            // Si tampoco está, buscar en los hijos.
            if (l_PlayerController == null)
            {
                l_PlayerController =
                    m_PlayerTransform.GetComponentInChildren<FPSController>();
            }
        }

        // Último recurso: buscar el controlador en la escena.
        if (l_PlayerController == null)
        {
            l_PlayerController =
                FindAnyObjectByType<FPSController>();
        }

        if (l_PlayerController == null)
        {
            Debug.LogError(
                "No se encuentra ningún FPSController activo en la escena."
            );
            return;
        }

        // Guardar la referencia correcta del jugador.
        m_PlayerTransform = l_PlayerController.transform;

        // Aplicar el daño y activar el efecto rojo.
        l_PlayerController.TakeDamage(m_Damage);

        Debug.Log("El dron ha hecho " + m_Damage + " de daño al jugador.");
    }

    void PlayShootSound()
    {
        if (m_AudioSource != null && m_ShootSound != null)
        {
            m_AudioSource.PlayOneShot(m_ShootSound);
        }
    }

    public void UpdateLifeBarUI()
    {
        if (m_LifeBarElementUI == null && IngameUI.GetIngameUI() != null)
        {
            m_LifeBarElementUI = IngameUI.GetIngameUI().m_LifeBarElementUI;
        }

        if (m_LifeBarElementUI == null || m_LifeBarPosition == null || Camera.main == null)
        {
            return;
        }

        if (!m_Die && m_LifeBarVisible)
        {
            Vector3 l_ViewportPos = Camera.main.WorldToViewportPoint(m_LifeBarPosition.position);

            if (l_ViewportPos.z > 0)
            {
                RectTransform l_CanvasRect = m_LifeBarElementUI.transform.parent.GetComponent<RectTransform>();
                Vector2 l_CanvasPosition = new Vector2(
                    (l_ViewportPos.x - 0.5f) * l_CanvasRect.rect.width,
                    (l_ViewportPos.y - 0.5f) * l_CanvasRect.rect.height
                );

                float l_NormalizedLife = (float)m_Life / m_MaxLife;
                m_LifeBarElementUI.Show(l_CanvasPosition, l_NormalizedLife);
                return;
            }
        }

        m_LifeBarElementUI.Hide();
    }



    public void Hit(HitCollider.THitColliderType HitColliderType)
    {
        int l_LifePoints = 0;
        switch (HitColliderType)
        {
            case HitCollider.THitColliderType.BODY:
                l_LifePoints = m_BodyHitColliderAmount;
                break;
            case HitCollider.THitColliderType.HEAD:
                l_LifePoints = m_HeadHitColliderAmount;
                break;
            case HitCollider.THitColliderType.HELIX:
                l_LifePoints = m_HelixHitColliderAmount;
                break;
            default:
                Debug.LogError("HitColliderType '" + HitColliderType);
                break;
        }

        m_Life -= l_LifePoints;
        if (m_Life <= 0)
        {
            SetDieState();
        }
        else
        {
            SetHitState();
        }

        Debug.Log("hit current life " +  m_Life);
    }

    public int GetLife()
    {
        return m_Life;
    }

}
