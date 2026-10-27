using UnityEngine;

public class AmmoItem : Item
{
    public int m_AmmoPoints = 20;

    public override bool CanPick()
    {
        FPSController l_Player = GetPlayer();

        return l_Player != null &&
               l_Player.CanReceiveAmmo(m_AmmoPoints);
    }

    public override void Pick()
    {
        FPSController l_Player = GetPlayer();

        if (l_Player != null && l_Player.AddAmmo(m_AmmoPoints))
        {
            base.Pick();
        }
    }
}
