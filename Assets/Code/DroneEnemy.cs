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
    public List<Transform> m_Waypoints;
    int m_CurrentPatrolId;

    public float m_ChaseDistance = 5.0f;

    public int m_MaxLife = 50;
    int m_Life;
    public Transform m_LifeBarPosition;


    [Header("Hits")]
    public int m_HelixHitColliderAmount = 10;
    public int m_HeadHitColliderAmount = 50;
    public int m_BodyHitColliderAmount = 20;

    private Transform m_PlayerTransform;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        m_Life = m_MaxLife;

        GameController.GetGameController().AddEnemy(this);

        if(m_Waypoints != null && m_Waypoints.Count > 0)
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
            case TState.DIE:
                UpdateDieState();
                break;
        }
    }

    void SetIdleState()
    {
        m_CurrentState=TState.IDLE;
    }

    void UpdateIdleState()
    {
        if(m_Waypoints != null & m_Waypoints.Count > 0)
        {
            SetPatrolState();
        }
        
    }


    void SetAlertState()
    {
        m_CurrentState = TState.ALERT;
    }

    void UpdateAlertState()
    {
    }

    void SetHitState()
    {
        m_CurrentState = TState.HIT;
    }

    void UpdateHitState()
    {

    }

    void SetAttackState()
    {
        m_CurrentState = TState.ATTACK;
    }

    void UpdateAttackState()
    {

    }

    void SetPatrolState()
    {
        m_CurrentState = TState.PATROL;

        if(m_Waypoints != null && m_Waypoints.Count > 0)
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
            m_CurrentPatrolId = (m_CurrentPatrolId + 1) % m_Waypoints.Count;
            m_NavMeshAgent.SetDestination (m_Waypoints [m_CurrentPatrolId].position);
        }

    }


    void UpdateChaseState()
    {
    }

    void SetDieState()
    {
        m_CurrentState = TState.DIE;
        Destroy(gameObject);
    }

    void UpdateDieState()
    {

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
