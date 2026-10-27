using UnityEngine;

public class HealthItem : Item
{
    public int m_HealthPoints = 25;

    public override bool CanPick()
    {
        FPSController l_Player = GetPlayer();

        return l_Player != null && l_Player.CanReceiveHealth(m_HealthPoints);
    }

    public override void Pick()
    {
        FPSController l_Player = GetPlayer();

        if (l_Player != null && l_Player.AddLife(m_HealthPoints))
        {
            base.Pick();
        }
    }
}