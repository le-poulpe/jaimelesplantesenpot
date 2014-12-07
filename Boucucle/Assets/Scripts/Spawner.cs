using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	
	public Vector2[] m_LightGuySpawnPoints;
	public Vector2[] m_NemesisSpawnPoints;
	public Vector2[] m_PotDeFleurSpawnPoints;
	public GameObject m_LightGuyPrefab;
	public GameObject m_NemesisPrefab;
	public GameObject m_PotDeFleurPrefab;

	// Use this for initialization
	void Start () {

		GameObject.Instantiate(m_LightGuyPrefab, m_LightGuySpawnPoints[Random.Range(0, m_LightGuySpawnPoints.Length)], Quaternion.identity);
        GameObject.Instantiate(m_NemesisPrefab, m_NemesisSpawnPoints[Random.Range(0, m_NemesisSpawnPoints.Length)], Quaternion.identity);
        GameObject.Instantiate(m_PotDeFleurPrefab, m_PotDeFleurSpawnPoints[Random.Range(0, m_PotDeFleurSpawnPoints.Length)], Quaternion.identity);
	}
}
