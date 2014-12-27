using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {
	
	public Vector2[] m_LightGuySpawnPoints;
	public Vector2[] m_NemesisSpawnPoints;
	public Vector2[] m_PotDeFleurSpawnPoints;
	public GameObject m_LightGuyPrefab;
	public GameObject m_NemesisPrefab;
	public GameObject m_PotDeFleurPrefab;

	public int m_NbPotDeFleurs = 3;
	public float m_MinPotDeFleursDistance = 5.0f;
	public bool m_ShowPots = false;
	
    public void Spawn()
    {
		// spawn a light guy
        Vector2 spawnLight = m_LightGuySpawnPoints[Random.Range(0, m_LightGuySpawnPoints.Length)];
		GameObject.Instantiate(m_LightGuyPrefab, transform.position + new Vector3(spawnLight.x, spawnLight.y), Quaternion.identity);

		//spawn a nemesis
		Vector2 spawnNemesis = m_NemesisSpawnPoints[Random.Range(0, m_NemesisSpawnPoints.Length)];
		GameObject.Instantiate(m_NemesisPrefab, transform.position + new Vector3(spawnNemesis.x, spawnNemesis.y), Quaternion.identity);

		//spawn flower pots
		{
			if (m_PotDeFleurSpawnPoints.Length < m_NbPotDeFleurs)
				m_NbPotDeFleurs = m_PotDeFleurSpawnPoints.Length;

			// Fisher-Yates on PotDeFleur spawnpoints
			int[] indexes = new int[m_PotDeFleurSpawnPoints.Length];
			for (int i = 0; i < indexes.Length; ++i)
				indexes[i] = i;
			for (int i = indexes.Length-1; i > 0 ; --i)
			{
				int j = Random.Range(0, i);
				int tmp = indexes[j];
				indexes[j] = indexes[i];
				indexes[i] = tmp;
			}

			List<GameObject> potDeFleurs = new List<GameObject>();

			// spawn all flower pots at random position, at a minimum distance from each other
			for (int i = 0, j = 0; i < indexes.Length && j < m_NbPotDeFleurs; ++i)
			{
				Vector2 pos = m_PotDeFleurSpawnPoints[indexes[i]];
				bool canSpawn = true;
				for (int k = 0; k < potDeFleurs.Count && canSpawn; ++k)
				{
					Vector2 delta = potDeFleurs[k].transform.position - transform.position;
					delta = delta - pos;
					if (delta.magnitude <= m_MinPotDeFleursDistance)
						canSpawn = false;
				}
				if (canSpawn)
				{
					GameObject flowerPot = GameObject.Instantiate(m_PotDeFleurPrefab, transform.position + new Vector3(pos.x, pos.y), Quaternion.identity) as GameObject;
					potDeFleurs.Add(flowerPot);
					++j;
				}
			}

			if (potDeFleurs.Count < m_NbPotDeFleurs)
			{
				Debug.LogError("Flower pot spawn points are not far enough apart !");
			}

			foreach (GameObject pot in potDeFleurs)
			{
				pot.GetComponent<PotDeFleur>().setVisibility(m_ShowPots);
			}
		}
    }
}
