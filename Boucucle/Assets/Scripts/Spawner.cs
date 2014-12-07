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
        
        Vector2 spawnLight = m_LightGuySpawnPoints[Random.Range(0, m_LightGuySpawnPoints.Length)];
        Vector2 spawnNemesis = m_NemesisSpawnPoints[Random.Range(0, m_NemesisSpawnPoints.Length)];
        Vector2 spawnPotDeFleur =  m_PotDeFleurSpawnPoints[Random.Range(0, m_PotDeFleurSpawnPoints.Length)];
        

		GameObject.Instantiate(m_LightGuyPrefab, transform.position + new Vector3(spawnLight.x, spawnLight.y), Quaternion.identity);
        GameObject.Instantiate(m_NemesisPrefab, transform.position + new Vector3(spawnNemesis.x, spawnNemesis.y), Quaternion.identity);
        GameObject.Instantiate(m_PotDeFleurPrefab, transform.position + new Vector3(spawnPotDeFleur.x, spawnPotDeFleur.y), Quaternion.identity);
	}
}
