using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public Vector2[] m_SpawnPoints;
	public GameObject m_LightGuyPrefab;
	public GameObject m_NemesisPrefab;
	public GameObject m_PotDeFleurPrefab;

	// Use this for initialization
	void Start () {

        if (m_SpawnPoints.Length < 3)
        {
            Debug.LogError("Need at least 3 spawn points !");
        }

        // Fisher Yates
		for (int i = 0; i < m_SpawnPoints.Length; ++i)
        {
            int rand = Random.Range(i, m_SpawnPoints.Length);
            Vector2 temp = m_SpawnPoints[i];
            m_SpawnPoints[i] = m_SpawnPoints[rand];
            m_SpawnPoints[rand] = temp;
        }

        GameObject.Instantiate(m_LightGuyPrefab, m_SpawnPoints[0], Quaternion.identity);
        GameObject.Instantiate(m_NemesisPrefab, m_SpawnPoints[1], Quaternion.identity);
        GameObject.Instantiate(m_PotDeFleurPrefab, m_SpawnPoints[2], Quaternion.identity);
	}
}
