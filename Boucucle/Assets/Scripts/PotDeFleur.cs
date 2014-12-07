﻿using UnityEngine;
using System.Collections;

public class PotDeFleur : MonoBehaviour {

    private LightGuy[] m_LightGuys;

	public Light m_Light;

    public float m_LightTriggerDistance;
    public Vector2 m_LightMinMaxRange;
    public Vector2 m_LightMinMaxIntensity;

	// Use this for initialization
	void Start () {
		if (m_Light == null)
		{
			Debug.LogError("no light set on the pot de fleur !");
		}
		else
		{
			m_Light.intensity = 0;
			m_Light.range = 0;
		}

        m_LightGuys = FindObjectsOfType(typeof(LightGuy)) as LightGuy[];
	}
	
	// Update is called once per frame
	void Update () {

        foreach(LightGuy lightguy in m_LightGuys)
        {
            Vector2 vec = lightguy.transform.position - this.transform.position;

            float dist = vec.magnitude;
            if (dist < m_LightTriggerDistance)
            {
                float t = (m_LightTriggerDistance - dist) / m_LightTriggerDistance;
                m_Light.range = Mathf.Lerp(m_LightMinMaxRange.x, m_LightMinMaxRange.y, t);
                m_Light.intensity = Mathf.Lerp(m_LightMinMaxIntensity.x, m_LightMinMaxIntensity.y, t);
            }
        }
	}
}