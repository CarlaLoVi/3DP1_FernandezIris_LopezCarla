using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform m_DestroyObjects;
    static GameController m_GameCOntroller = null;
    public KeyCode m_RestartKeyCode = KeyCode.F5;
    FPSController m_Player;

    private void Awake()
    {
        if (m_GameCOntroller == null)
        {
            m_GameCOntroller = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }

    }

    static public GameController GetGameController()
    {
        return m_GameCOntroller;

    }

    public void SetPlayer(FPSController Player)
    {
        m_Player = Player;
    }

    public void Restart()
    {
        Debug.Log("RestartGame");
        for (int i = 0; i < m_DestroyObjects.childCount; ++i)
        {
            GameObject.Destroy(m_DestroyObjects.GetChild(i).gameObject);
            m_Player.Restart();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_RestartKeyCode))
        {
            Restart();
        }
    }
}
