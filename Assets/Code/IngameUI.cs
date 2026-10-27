using UnityEngine;

public class IngameUI : MonoBehaviour
{
    public LifeBarElementUI m_LifeBarElementUI;
    private static IngameUI m_IngameUI;

    private void Awake()
    {
        m_IngameUI = this;
    }

    private void Start()
    {
        GameController.GetGameController().SetIngameUI(this);
    }

    public static IngameUI GetIngameUI()
    {
        return m_IngameUI;
    }
}
