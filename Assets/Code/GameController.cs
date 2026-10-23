using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    public Transform m_DestroyObjects;
    static GameController m_GameCOntroller = null;
    public KeyCode m_RestartKeyCode = KeyCode.F5;
    FPSController m_Player;
    IngameUI m_IngameUI;
    List<DroneEnemy> m_Enemies = new List<DroneEnemy>();
    Fade m_Fade;

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
            StartCoroutine(m_Fade.FadeOut());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_RestartKeyCode))
        {
            Restart();
        }
    }

    public void SetIngameUI(IngameUI _IngameUI)
    {
        m_IngameUI = _IngameUI;
    }

    public IngameUI GetIngameUI()
    {
        return m_IngameUI;
    }

    public void AddEnemy(DroneEnemy Enemy)
    {
        m_Enemies.Add(Enemy);
    }

    public List<DroneEnemy> GetEnemies()
    {
        return m_Enemies;
    }
    public void SetFade(Fade _Fade)
    {
        m_Fade = _Fade;
    }
    public Fade GetFade()
    {
        return m_Fade;
    }
}
