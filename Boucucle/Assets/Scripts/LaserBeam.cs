
//This is free to use and no attribution is required
//No warranty is implied or given
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]

public class LaserBeam : MonoBehaviour
{

    public float laserWidth = 1.0f;
    public float noise = 0.0f;
    public float maxLength = 50.0f;
    public Color color = Color.red;
    public float texLength = 0.1f;

    LineRenderer lineRenderer;
    int length;
    Vector3[] position;
    //Cache any transforms here
    Transform endEffectTransform;
    //The particle system, in this case sparks which will be created by the Laser
    public ParticleSystem endEffect;
    Vector3 offset;
    GameObject m_HitObject;


    // Use this for initialization
    void Start()
    {
		InitLaser();
    }

	void InitLaser()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetWidth(laserWidth, laserWidth);
		offset = new Vector3(0, 0, 0);
		endEffect = GetComponentInChildren<ParticleSystem>();
		if (endEffect)
			endEffectTransform = endEffect.transform;
		m_HitObject = null;
		RenderLaser();
	}

	void OnEnable()
	{
		InitLaser();
	}

    // Update is called once per frame
    void Update()
    {
        RenderLaser();
	}

	public Vector2 GetDir()
	{
		return new Vector2 (transform.right.x, transform.right.y);
	}

    void RenderLaser()
    {

        //Shoot our laserbeam forwards!
        UpdateLength();

        lineRenderer.SetColors(color, color);
		//Move through the Array
        for (int i = 0; i < length; i++)
        {
            //Set the position here to the current location and project it in the forward direction of the object it is attached to
            offset.x = transform.position.x + i * transform.right.x * texLength + Random.Range(-noise, noise);
            offset.y = transform.position.y + i * transform.right.y * texLength + Random.Range(-noise, noise);
            position[i] = offset;

            lineRenderer.SetPosition(i, position[i]);

        }



    }

    void UpdateLength()
    {
        //Raycast from the location of the cube forwards
        m_HitObject = null;
        RaycastHit2D[] hit;
        hit = Physics2D.RaycastAll(transform.position, transform.right, maxLength);
        int i = 0;
        while (i < hit.Length)
        {
            //Check to make sure we aren't hitting triggers but colliders
            if (!hit[i].collider.isTrigger)
            {
                Vector2 point = new Vector2(hit[i].point.x, hit[i].point.y);
                point -= new Vector2(transform.position.x, transform.position.y);
                length = (int)Mathf.Round(point.magnitude / texLength) + 2;
                m_HitObject = hit[i].collider.gameObject;
                position = new Vector3[length];
                //Move our End Effect particle system to the hit point and start playing it
                if (endEffect)
                {
                    endEffectTransform.position = hit[i].point;
                    if (!endEffect.isPlaying)
                        endEffect.Play();
                }
                lineRenderer.SetVertexCount(length);
                return;
            }
            i++;
        }
        //If we're not hitting anything, don't play the particle effects
        if (endEffect)
        {
            if (endEffect.isPlaying)
                endEffect.Stop();
        }
        length = (int)Mathf.Round(maxLength / texLength);
        position = new Vector3[length];
        lineRenderer.SetVertexCount(length);


    }

    public GameObject GetHitObject() { return m_HitObject; }
}