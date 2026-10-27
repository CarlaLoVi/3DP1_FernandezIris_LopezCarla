using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HudController : MonoBehaviour
{
    [Header("Player")]
    public PlayerController m_PlayerController;

    [Header("Life")]
    public Image m_LifeImage;
    public Gradient m_LifeGradient;
    public TextMeshProUGUI m_LifeText;

    [Header("Shield")]
    public Image m_ShieldImage;
    public Gradient m_ShieldGradient;
    public TextMeshProUGUI m_ShieldText;

    [Header("Ammo")]
    public TextMeshProUGUI m_AmmoText;

    [Header("Score")]
    public bool m_ShowScore = false;
    public TextMeshProUGUI m_ScoreText;

    void Start()
    {
        if (m_PlayerController == null)
        {
            m_PlayerController = GameManager.GetGameManager().GetPlayer();
        }

        m_PlayerController.OnLifeChanged += UpdateLife;
        m_PlayerController.OnShieldChanged += UpdateShield;
        m_PlayerController.OnAmmoChanged += UpdateAmmo;
        m_PlayerController.OnScoreChanged += UpdateScore;

        UpdateLife(m_PlayerController.m_Life, m_PlayerController.m_MaxLife);
        UpdateShield(m_PlayerController.m_Shield, m_PlayerController.m_MaxShield);
        UpdateAmmo(m_PlayerController.m_Ammo, m_PlayerController.m_TotalAmmo);
        UpdateScore(m_PlayerController.m_Score, m_PlayerController.m_ScoreToWin);

        m_ScoreText.gameObject.SetActive(m_PlayerController.m_ShowScore);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (m_ShowScore != m_PlayerController.m_ShowScore)
        {
            m_ShowScore = m_PlayerController.m_ShowScore;
            m_ScoreText.gameObject.SetActive(m_ShowScore);
        }
    }

    void UpdateLife(float life, float lifeMax)
    {
        float pct = Mathf.Clamp01(life / lifeMax);
        m_LifeImage.fillAmount = pct;

        if (m_LifeGradient != null)
        {
            m_LifeImage.color = m_LifeGradient.Evaluate(pct);
        }
        m_LifeText.text = life.ToString() + " / " + lifeMax.ToString();
    }

    void UpdateShield(float shield, float shieldMax)
    {
        float pct = Mathf.Clamp01(shield / shieldMax);
        m_ShieldImage.fillAmount = pct;
        if (m_ShieldGradient != null)
        {
            m_ShieldImage.color = m_ShieldGradient.Evaluate(pct);
        }
        m_ShieldText.text = shield.ToString() + " / " + shieldMax.ToString();
    }

    void UpdateAmmo(int ammo, int totalAmmo)
    {
        m_AmmoText.text = ammo.ToString() + " / " + totalAmmo.ToString();
    }

    void UpdateScore(int Score, int ScoreRequired)
    {
        m_ScoreText.text = Score.ToString() + " / " + ScoreRequired.ToString();
    }

    public void ShowScoreHUD()
    {
        m_ScoreText.gameObject.SetActive(true);
    }

    public void HideScoreHUD()
    {
        m_ScoreText.gameObject.SetActive(false);
    }
}
