﻿using UnityEngine;
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
    LightGuy[] m_LightGuys;
    Nemesis[] m_Nemesis;
    Spawner m_Spawner;
    E_GameState m_GameState;
    public GameObject m_TitleScreen;
    public GameObject m_LightScreen;
    public GameObject m_DarkScreen;

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
	
    public void SetGameState(E_GameState state)
    {
        m_GameState = state;
        switch (m_GameState)
        {
            case E_GameState.GM_TITLE:
                m_TitleScreen.SetActive(true);
                m_LightScreen.SetActive(false);
                m_DarkScreen.SetActive(false);
                break;
            case E_GameState.GM_PLAY:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(false);
                m_DarkScreen.SetActive(false);
                m_Spawner.Spawn();
                m_LightGuys = FindObjectsOfType<LightGuy>();
                m_Nemesis = FindObjectsOfType<Nemesis>();
                break;
            case E_GameState.GM_LIGHT_WIN:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(true);
                m_DarkScreen.SetActive(false);
                foreach (LightGuy guy in m_LightGuys)
                    guy.enabled = false;
                foreach (Nemesis nem in m_Nemesis)
                    nem.enabled = false;
                break;
            case E_GameState.GM_NEM_WIN:
                m_TitleScreen.SetActive(false);
                m_LightScreen.SetActive(false);
                m_DarkScreen.SetActive(true);
                foreach (LightGuy guy in m_LightGuys)
                    guy.enabled = false;
                foreach (Nemesis nem in m_Nemesis)
                    nem.enabled = false;
                break;
        }
    }

    void Update()
    {
        switch (m_GameState)
        {
            case E_GameState.GM_TITLE:
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0))
                    SetGameState(E_GameState.GM_PLAY);
                break;
            case E_GameState.GM_LIGHT_WIN:
            case E_GameState.GM_NEM_WIN:
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0))
                    Application.LoadLevel(0);
                break;
        }
    }
}
