using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightGuy : MonoBehaviour {

    private Rigidbody2D m_RigidBody;
    private bool m_CanJump = false;
    private Nemesis m_AttackingNemesis = null;
    private List<GameObject> m_CollidingStuff;
    private float m_Energy;

    public float m_JumpImpulse = 5;
    public float m_MoveSpeed = 1;
    public float m_MaxEnergy = 100;
    public float m_EnergyLossPerSecond = 0.1f;
    public float m_BlastSuckPerSecond = 15;

    public Light m_AuraLight = null;
    public Light m_BlastLight = null;
    public float m_MinAuraIntensity = 0;
    public float m_MaxAuraIntensity = 2;

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_CanJump = false;
        m_CollidingStuff = new List<GameObject>();
        m_Energy = m_MaxEnergy;

        if (m_RigidBody == null)
        {
            Debug.LogError("No rigidbody 2D attached to lightguy !");
        }
        if (m_AuraLight == null)
        {
            Debug.LogError("No aura light attached to lightguy !");
        }
        if (m_BlastLight == null)
        {
            Debug.LogError("No blast light 2D attached to lightguy !");
        }
        else
            m_BlastLight.enabled = false;
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        Nemesis nemesis = coll.gameObject.GetComponent<Nemesis>();
        if (nemesis != null)
            m_AttackingNemesis = nemesis;

        if (coll.collider.bounds.max.y < this.collider2D.bounds.min.y)
        {
            m_CollidingStuff.Add(coll.gameObject);
            m_CanJump = true;
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        Nemesis nemesis = coll.gameObject.GetComponent<Nemesis>();
        if (nemesis == m_AttackingNemesis)
            m_AttackingNemesis = null;

        m_CollidingStuff.Remove(coll.gameObject);
        m_CanJump = m_CollidingStuff.Count > 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_Energy > 0)
        {
            // update controls
            float axisValue = Input.GetAxis("HorizontalP1Joy");
            m_RigidBody.AddForce(new Vector2(axisValue * m_MoveSpeed, 0), ForceMode2D.Impulse);

            // Jump
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) && m_CanJump)
            {
                m_CanJump = false;
                m_RigidBody.AddForce(new Vector2(0, m_JumpImpulse), ForceMode2D.Impulse);
            }

            //Shoot
            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                // TODO
                Debug.Log("Piouuu");
            }

            //Blast
            if (Input.GetAxis("BlastP1Joy") < 0) // xbox left trigger
            {
                m_BlastLight.enabled = true;
            }
            else
            {
                m_BlastLight.enabled = false;
            }


            // update light
            if (m_AuraLight != null)
                m_AuraLight.intensity = Mathf.Lerp(m_MinAuraIntensity, m_MaxAuraIntensity, m_Energy / m_MaxEnergy);

            // die slowly
            m_Energy -= m_EnergyLossPerSecond * Time.deltaTime;

            // die less slowly if in contact with nemesis
            if (m_AttackingNemesis != null)
                m_Energy -= m_AttackingNemesis.m_EnergySuckPerSecond * Time.deltaTime;

            // die less slowly if blasting
            if (m_BlastLight.enabled)
                m_Energy -= m_BlastSuckPerSecond * Time.deltaTime;
        }
        else if (m_BlastLight.enabled)
            m_BlastLight.enabled = false;
	}
}
