using UnityEngine;

public class ShieldItem : Item
{
    public int m_ShieldPoints = 25;

    public override bool CanPick()
    {
        FPSController l_Player = GetPlayer();

        return l_Player != null && l_Player.CanReceiveShield(m_ShieldPoints);
    }

    public override void Pick()
    {
        FPSController l_Player = GetPlayer();

        if (l_Player != null && l_Player.AddShield(m_ShieldPoints))
        {
            base.Pick();
        }
    }
}