using UnityEngine;

public class HitCollider : MonoBehaviour
{
    public enum THitColliderType
    {
        HEAD=0,
        BODY,
        HELIX

    }

    public THitColliderType m_Type;
    public DroneEnemy m_DroneEnemy;

    public void Hit()
    {
        m_DroneEnemy.Hit(m_Type);
    }
}
