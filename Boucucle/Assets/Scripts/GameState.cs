using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

    public enum E_GameState
    {
        GM_TITLE,
        GM_PLAY,
        GM_LIGHT_WIN,
        GM_NEM_WIN
    };

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

	// Use this for initialization
	void Start () {
        m_Spawner = FindObjectOfType<Spawner>();
        if (!m_Once)
        {
            m_Once = true;
            SetGameState(E_GameState.GM_TITLE);
        }
        else
            SetGameState(E_GameState.GM_PLAY);
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
				if (m_LightGuy != null && m_Nemesis != null) // WHY should I check this ?
				{
					if (m_LightGuy.IsDead())
						SetGameState(E_GameState.GM_NEM_WIN);
					else if (m_Nemesis.IsDead())
						SetGameState(E_GameState.GM_LIGHT_WIN);
					else
					{
						PotDeFleur[] potsdeFleur = FindObjectsOfType<PotDeFleur>();
						if (potsdeFleur.Length == 0)				
							SetGameState(E_GameState.GM_LIGHT_WIN);
					}
				}
				break;
            case E_GameState.GM_LIGHT_WIN:
            case E_GameState.GM_NEM_WIN:
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown("space"))
                    Application.LoadLevel(0);
                break;
        }
    }
}
