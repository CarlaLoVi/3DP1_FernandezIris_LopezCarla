using TMPro;
using UnityEngine;

public class ShootingGallery : MonoBehaviour
{
    [Header("Partida Ajustes")]
    public float m_Duration = 30.0f;
    public int m_GoalScore = 50;
    public KeyCode m_ReplayKeyCode = KeyCode.Return;


    [Header("HUD")]
    public GameObject m_HUDPanel;
    public TMP_Text m_ScoreText;
    public TMP_Text m_TimeText;

    [Header("Targets")]
    public Transform m_TargetsParent;

    int m_Score = 0;
    float m_TimeRemaining;

    bool m_PlayerInside = false;
    bool m_IsRunning = false;
    bool m_HasFinished = false;


    private void Start()
    {
        m_TimeRemaining = m_Duration;

        if (m_HUDPanel != null)
        {
            m_HUDPanel.SetActive(false);
        }

        UpdateHUD();
    }

    private void Update()
    {
        if (m_IsRunning)
        {
            m_TimeRemaining -= Time.deltaTime;

            if (m_TimeRemaining <= 0.0f)
            {
                m_TimeRemaining = 0.0f;
                FinishGame(false);
            }

            UpdateHUD();
        }
        else if (m_PlayerInside && m_HasFinished && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            RestartGame();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        FPSController l_Player = other.GetComponentInParent<FPSController>();

        if (l_Player == null)
        {
            return;
        }

        m_PlayerInside = true;

        if (m_HUDPanel != null)
        {
            m_HUDPanel.SetActive(true);
        }

        UpdateHUD();
    }

    private void OnTriggerExit(Collider other)
    {
        FPSController l_Player = other.GetComponentInParent<FPSController>();

        if (l_Player == null)
        {
            return;
        }

        m_PlayerInside = false;

        if (m_HUDPanel != null)
        {
            m_HUDPanel.SetActive(false);
        }
    }

    public void AddPoints(int Points)
    {
        if (!m_PlayerInside || m_HasFinished)
        {
            return;
        }

        if (!m_IsRunning)
        {
            m_IsRunning = true;
            m_TimeRemaining = m_Duration;

        }

        m_Score += Points;
        UpdateHUD();

        if (m_Score >= m_GoalScore)
        {
            FinishGame(true);
        }

    }

    public void UpdateHUD()
    {
        if (m_ScoreText != null)
        {
            m_ScoreText.text = "Score: " + m_Score + " / " + m_GoalScore;
        }

        if (m_TimeText != null)
        {
            m_TimeText.text ="Tiempo: " + Mathf.CeilToInt(m_TimeRemaining) + " s";
        }
    }

    public void FinishGame(bool GoalReached)
    {
        m_IsRunning = false;
        m_HasFinished = true;

        SetTargetsActive(false);

    }

    void RestartGame()
    {
        m_Score = 0;
        m_TimeRemaining = m_Duration;
        m_IsRunning = false;
        m_HasFinished = false;

        SetTargetsActive(true);

        if (m_TargetsParent != null)
        {
            ShootingGalleryTarget[] l_Targets = m_TargetsParent.GetComponentsInChildren <ShootingGalleryTarget>(true);

            foreach (ShootingGalleryTarget l_Target in l_Targets)
            {
                l_Target.ResetTarget();
            }
        }

        UpdateHUD();

    }

    public void SetTargetsActive(bool Active)
    {
        if (m_TargetsParent == null)
        {
            return;
        }

        for (int i = 0; i < m_TargetsParent.childCount; i++)
        {
            m_TargetsParent.GetChild(i).gameObject.SetActive(Active);
        }
    }



}
