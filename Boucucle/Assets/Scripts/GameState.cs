using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameState : MonoBehaviour {

    public enum E_GameState
    {
        GM_TITLE,
        GM_PLAY,
        GM_LIGHT_WIN,
        GM_NEM_WIN
    };
	
	static int m_ScoreP1 = 0;
	static int m_ScoreP2 = 0;
	int m_NbPots = 0;

    static bool m_Once = false;
    LightGuy m_LightGuy = null;
    Nemesis m_Nemesis = null;
    Spawner m_Spawner = null;
    E_GameState m_GameState;
    public GameObject m_TitleScreen;
    public GameObject m_LightScreen;
    public GameObject m_DarkScreen;
	public GameObject m_MenuMusic;
	public GameObject m_NemesisVictoryMusic;
	public GameObject m_LightGuyVictoryMusic;
	public Text	      m_ScoreText;

	// Use this for initialization
	void Start () {
        m_Spawner = FindObjectOfType<Spawner>();
        if (!m_Once)
        {
            m_Once = true;
            SetGameState(E_GameState.GM_TITLE);
        }
        else
		{
			if (m_ScoreP1 >= 12 || m_ScoreP2 >= 12)
			{
				m_ScoreP1 = m_ScoreP2 = 0;
			}
	        SetGameState(E_GameState.GM_PLAY);
		}
		UpdateScore();
	}
	
    void SetGameState(E_GameState state)
    {
        m_GameState = state;
        switch (m_GameState)
        {
            case E_GameState.GM_TITLE:
                m_TitleScreen.SetActive(true);
                m_LightScreen.SetActive(false);
                m_DarkScreen.SetActive(false);
				m_MenuMusic.SetActive(true);
				m_NemesisVictoryMusic.SetActive(false);
				m_LightGuyVictoryMusic.SetActive(false);
                break;
            case E_GameState.GM_PLAY:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(false);
                m_DarkScreen.SetActive(false);
				m_MenuMusic.SetActive(false);
				m_NemesisVictoryMusic.SetActive(false);
				m_LightGuyVictoryMusic.SetActive(false);
                m_Spawner.Spawn();
                m_LightGuy = FindObjectOfType<LightGuy>();
                m_Nemesis = FindObjectOfType<Nemesis>();
                break;
            case E_GameState.GM_LIGHT_WIN:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(true);
                m_DarkScreen.SetActive(false);
				m_LightGuyVictoryMusic.SetActive(true);
	            m_LightGuy.enabled = false;
	            m_Nemesis.enabled = false;
                break;
            case E_GameState.GM_NEM_WIN:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(false);
				m_DarkScreen.SetActive(true);
				m_LightGuy.enabled = false;
				m_Nemesis.enabled = false;
				m_NemesisVictoryMusic.SetActive(true);
                break;
        }
    }

    void Update()
    {
        switch (m_GameState)
        {
            case E_GameState.GM_TITLE:
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown("space"))
                    SetGameState(E_GameState.GM_PLAY);
                break;
			case E_GameState.GM_PLAY:
				PotDeFleur[] potsdeFleur = FindObjectsOfType<PotDeFleur>();
				int currentNbPots = potsdeFleur.Length;
				if (currentNbPots < m_NbPots)
				{
					if (currentNbPots >= 2)
						m_ScoreP1 += 1;
					else if (currentNbPots >= 1)
						m_ScoreP1 += 2;
					else
						m_ScoreP1 += 3;
					UpdateScore();
				}
				m_NbPots = currentNbPots;
				if (m_LightGuy.IsDead())
				{
					SetGameState(E_GameState.GM_NEM_WIN);
					m_ScoreP2 += 3;
					UpdateScore();
				}
				else if (potsdeFleur.Length == 0)				
					SetGameState(E_GameState.GM_LIGHT_WIN);
				break;
            case E_GameState.GM_LIGHT_WIN:
            case E_GameState.GM_NEM_WIN:
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown("space"))
                    Application.LoadLevel(0);
                break;
        }
    }

	void UpdateScore()
	{
		m_ScoreText.text = "" + m_ScoreP1 + " | " + m_ScoreP2;
	}
}
