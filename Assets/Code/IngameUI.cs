using UnityEngine;

public class IngameUI : MonoBehaviour
{
    public LifeBarElementUI m_LifeBarElementUI;

    private void Start()
    {
        GameController.GetGameController().SetIngameUI(this);
    }
}
