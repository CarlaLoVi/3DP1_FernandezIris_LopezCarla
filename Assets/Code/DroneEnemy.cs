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


    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        m_Life = m_MaxLife;
        SetIdleState();
        GameController.GetGameController().AddEnemy(this);
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
    void SetAlertState()
    {
        m_CurrentState = TState.ALERT;
    }
    void SetHitState()
    {
        m_CurrentState = TState.HIT;
    }
    void SetAttackState()
    {
        m_CurrentState = TState.ATTACK;
    }
    void SetPatrolState()
    {
        m_CurrentState = TState.PATROL;
    }
    void SetDieState()
    {
        m_CurrentState = TState.DIE;
    }
    void UpdateIdleState()
    {
    }
    void UpdateAlertState()
    {
    }
    void UpdateHitState()
    {
    }
    void UpdateAttackState()
    {
    }
    void UpdatePatrolState()
    {
    }
    void UpdateDieState()
    {
    }
    void UpdateChaseState()
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
