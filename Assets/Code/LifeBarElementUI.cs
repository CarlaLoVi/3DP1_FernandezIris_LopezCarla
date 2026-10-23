using UnityEngine;
using UnityEngine.UI;

public class LifeBarElementUI : MonoBehaviour
{
    public Image m_LifeBarImageUI;
    RectTransform m_RectTransform;

    public void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }
    public void Show (Vector2 Position, float NormalizedLife)
    {
        gameObject.SetActive(true);
        m_RectTransform.anchoredPosition = Position;
        m_LifeBarImageUI.fillAmount = NormalizedLife;

    }

    public void Hide()
    {
        gameObject.SetActive (false);
    }

}
