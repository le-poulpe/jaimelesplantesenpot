﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameState : MonoBehaviour {

    public enum E_GameState
    {
        GM_TITLE,
        GM_PLAY,
        GM_LIGHT_WIN,
		GM_LIGHT_ROUNDWIN,
        GM_NEM_WIN,
		GM_NEM_ROUNDWIN
    };
	
	static bool m_Once = false;
	static int m_ScoreP1 = 0;
	static int m_ScoreP2 = 0;
	static private int[] m_ArenaOrder;
	static private int	  m_CurrentArenaIndex;

	private int m_NbPots;
	private int m_BaseNbPots;
	
	public float m_EndGameTimer = 1.2f;

    LightGuy m_LightGuy = null;
    Nemesis m_Nemesis = null;
    Spawner m_Spawner = null;
    E_GameState m_GameState;
	public GameObject m_TitleScreen;
    public GameObject m_LightScreen;
    public GameObject m_LightRoundScreen;
    public GameObject m_DarkScreen;
    public GameObject m_DarkRoundScreen;
	public GameObject m_MenuMusic;
	public GameObject m_NemesisVictoryMusic;
	public GameObject m_NemesisRoundVictoryMusic;
	public GameObject m_LightGuyVictoryMusic;
	public GameObject m_LightGuyRoundVictoryMusic;
	public Text	      m_ScoreText;
	public int	      m_TargetScore = 12;
	public int 		  m_KillLightGuyScore = 3;
	public int[]	  m_PotScores;
	public GameObject[] m_Arenas;


	void ShuffleArenas()
	{
		for (int i = m_ArenaOrder.Length-1; i > 0 ; --i)
		{
			int j = Random.Range(0, i);
			int tmp = m_ArenaOrder[j];
			m_ArenaOrder[j] = m_ArenaOrder[i];
			m_ArenaOrder[i] = tmp;
		}
	}
	void SelectArena(int index)
	{
		int randomIndex = m_ArenaOrder[index];
		for (int i = 0; i < m_Arenas.Length; ++i)
		{
			m_Arenas[i].SetActive(i == randomIndex);
		}
		m_Spawner = m_Arenas[randomIndex].GetComponentInChildren<Spawner>();
	}

	// Use this for initialization
	void Start () {
        if (!m_Once)
        {
            m_Once = true;
			
			//init arenas
			{
				m_CurrentArenaIndex = 0;
				m_ArenaOrder = new int[m_Arenas.Length];
				for (int i = 0; i < m_ArenaOrder.Length; ++i)
					m_ArenaOrder[i] = i;
				ShuffleArenas();
				SelectArena(m_CurrentArenaIndex);
			}

            SetGameState(E_GameState.GM_TITLE);
        }
        else
		{
			if (m_ScoreP1 >= m_TargetScore || m_ScoreP2 >= m_TargetScore)
			{
				m_ScoreP1 = m_ScoreP2 = 0;

				// new arena !
				{
					++m_CurrentArenaIndex;
					if (m_CurrentArenaIndex >= m_Arenas.Length)
					{
						m_CurrentArenaIndex = 0;
						ShuffleArenas();
					}
				}
			}
			SelectArena(m_CurrentArenaIndex);
			SetGameState(E_GameState.GM_PLAY);
		}
		UpdateScore();

		//retrieve number of flower pots in arena
		m_BaseNbPots = m_NbPots = m_Spawner.m_NbPotDeFleurs;
	}
	
    void SetGameState(E_GameState state)
    {
        m_GameState = state;
        switch (m_GameState)
        {
            case E_GameState.GM_TITLE:
                m_TitleScreen.SetActive(true);
                m_LightScreen.SetActive(false);
				m_LightRoundScreen.SetActive(false);
                m_DarkScreen.SetActive(false);
				m_DarkRoundScreen.SetActive(false);
				m_MenuMusic.SetActive(true);
				m_NemesisVictoryMusic.SetActive(false);
				m_LightGuyVictoryMusic.SetActive(false);
				m_NemesisRoundVictoryMusic.SetActive(false);
				m_LightGuyRoundVictoryMusic.SetActive(false);
                break;
            case E_GameState.GM_PLAY:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(false);
				m_LightRoundScreen.SetActive(false);
                m_DarkScreen.SetActive(false);
				m_DarkRoundScreen.SetActive(false);
				m_MenuMusic.SetActive(false);
				m_NemesisVictoryMusic.SetActive(false);
				m_NemesisRoundVictoryMusic.SetActive(false);
				m_LightGuyVictoryMusic.SetActive(false);
				m_LightGuyRoundVictoryMusic.SetActive(false);
                m_Spawner.Spawn();
                m_LightGuy = FindObjectOfType<LightGuy>();
                m_Nemesis = FindObjectOfType<Nemesis>();
                break;
            case E_GameState.GM_LIGHT_ROUNDWIN:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(false);
				m_LightRoundScreen.SetActive(true);
                m_DarkScreen.SetActive(false);
				m_DarkRoundScreen.SetActive(false);
	            m_LightGuy.enabled = false;
	            m_Nemesis.enabled = false;
				m_LightGuyRoundVictoryMusic.SetActive(true);
				break;
			case E_GameState.GM_LIGHT_WIN:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(true);
				m_LightRoundScreen.SetActive(false);
                m_DarkScreen.SetActive(false);
				m_DarkRoundScreen.SetActive(false);
	            m_LightGuy.enabled = false;
	            m_Nemesis.enabled = false;
				m_LightGuyVictoryMusic.SetActive(true);
                break;
		    case E_GameState.GM_NEM_ROUNDWIN:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(false);
				m_LightRoundScreen.SetActive(false);
                m_DarkScreen.SetActive(false);
				m_DarkRoundScreen.SetActive(true);
	            m_LightGuy.enabled = false;
	            m_Nemesis.enabled = false;
				m_NemesisRoundVictoryMusic.SetActive(true);
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
				else if (Input.GetKeyDown(KeyCode.Escape))
				{	
						Application.Quit();
				}
                break;
			case E_GameState.GM_PLAY:
				PotDeFleur[] potsdeFleur = FindObjectsOfType<PotDeFleur>();
				int currentNbPots = potsdeFleur.Length;
				if (currentNbPots < m_NbPots)
				{
					m_NbPots = currentNbPots;
					m_ScoreP1 += m_PotScores[m_BaseNbPots-1-m_NbPots];
					UpdateScore();
				}
				if (m_LightGuy.IsDead() && m_ScoreP2 < m_TargetScore)
				{
					SetGameState(E_GameState.GM_NEM_ROUNDWIN);
					m_ScoreP2 += m_KillLightGuyScore;
					UpdateScore();
				}
				if (m_ScoreP2 >= m_TargetScore)
				{
					SetGameState(E_GameState.GM_NEM_WIN);
				}				
				else if (potsdeFleur.Length == 0 && m_ScoreP1 < m_TargetScore)
				{
					SetGameState(E_GameState.GM_LIGHT_ROUNDWIN);
				}
				if (m_ScoreP1 >= m_TargetScore)
				{
					SetGameState(E_GameState.GM_LIGHT_WIN);
				}
			
				break;
			case E_GameState.GM_LIGHT_ROUNDWIN:
            case E_GameState.GM_NEM_ROUNDWIN:
				m_EndGameTimer -= 1 * Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) && m_EndGameTimer < 0 || Input.GetKeyDown(KeyCode.Joystick2Button0) && m_EndGameTimer < 0 || Input.GetKeyDown("space") && m_EndGameTimer < 0)
					Application.LoadLevel(0);
					
				break;
            case E_GameState.GM_LIGHT_WIN:
            case E_GameState.GM_NEM_WIN:
				m_EndGameTimer -= 1 * Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) && m_EndGameTimer < 0 || Input.GetKeyDown(KeyCode.Joystick2Button0) && m_EndGameTimer < 0 || Input.GetKeyDown("space") && m_EndGameTimer < 0)
                    Application.LoadLevel(0);
                break;
        }
    }

	void UpdateScore()
	{
		m_ScoreText.text = "" + m_ScoreP1 + " | " + m_ScoreP2;
	}
}
