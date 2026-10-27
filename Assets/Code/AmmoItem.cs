using UnityEngine;

public class AmmoItem : Item
{
    public int m_AmmoPoints = 20;

    public override bool CanPick()
    {
        return true;
    }

    public override void Pick()
    {
        base.Pick();
    }
}
