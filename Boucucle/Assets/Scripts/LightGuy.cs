using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightGuy : MonoBehaviour {

    private Rigidbody2D m_RigidBody;
    private Collider2D m_Collider;
    private bool m_CanJump = false;
    private Nemesis m_Nemesis;
    private bool m_AttackingNemesis = false;
    private float m_Energy;
    private bool m_IsBlasting = false;
    private bool m_IsShooting = false;
    private float m_ShootAngle = 0f;

    public GameObject m_Cursor;

    public float m_JumpImpulse = 5;
    public float m_MoveSpeed = 1;
    public float m_MaxEnergy = 100;
    public float m_EnergyLossPerSecond = 0.1f;
    public float m_BlastSuckPerSecond = 15;
    public float m_ShootSuckPerSecond = 10;
    public float m_RotationSpeed = 1000f;

    public Light m_AuraLight = null;
    public Light m_BlastLight = null;
    public GameObject m_Shoot = null;
    public float m_MinAuraIntensity = 0;
    public float m_MaxAuraIntensity = 2;

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_Collider = GetComponentInChildren<Collider2D>();
        m_CanJump = false;
        m_IsShooting = false;
        m_Energy = m_MaxEnergy;
        m_Nemesis = FindObjectOfType<Nemesis>();

        if (m_Nemesis == null)
        {
            Debug.LogError("No nemesis in scene !");
        }
        if (m_Collider == null)
        {
            Debug.LogError("No rigidbody 2D attached to lightguy !");
        }
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
        {
            m_BlastLight.gameObject.SetActive(false);
            m_IsBlasting = false;
        }
        if (m_Shoot == null)
        {
            Debug.LogError("No shoot attached to lightguy !");
        }
        else
        {
            m_Shoot.gameObject.SetActive(false);
            m_IsShooting = false;
        }

	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        Nemesis nemesis = coll.gameObject.GetComponent<Nemesis>();
        if (nemesis != null)
            m_AttackingNemesis = true;
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        Nemesis nemesis = coll.gameObject.GetComponent<Nemesis>();
        if (nemesis != null)
            m_AttackingNemesis = false;
    }
	
	// Update is called once per frame
	void Update ()
    {

        m_CanJump = Physics2D.OverlapCircleAll(m_Collider.transform.position + new Vector3(0, - m_Collider.bounds.extents.y, 0), 0.2f).Length > 1; // will collide at least with self
        if (m_Energy > 0)
        {
            // update controls
            float axisValueX = Input.GetAxis("HorizontalP1Joy");
            float axisValueY = Input.GetAxis("VerticalP1Joy");

            if (axisValueX != 0 || axisValueY != 0)
            {
                m_ShootAngle = Mathf.Atan2(-axisValueY, axisValueX) * 180 / Mathf.PI;
                m_Collider.transform.Rotate(new Vector3(0, 0, 1), axisValueX * -m_RotationSpeed * Time.deltaTime);
                
            }

            m_RigidBody.AddForce(new Vector2(axisValueX * m_MoveSpeed, 0), ForceMode2D.Impulse);

            if (m_Cursor != null)
            {
                if (axisValueX != 0 || axisValueY != 0)
                {
                    m_Cursor.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, m_ShootAngle));
                }
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) && m_CanJump)
            {
                m_CanJump = false;
                m_RigidBody.AddForce(new Vector2(0, m_JumpImpulse), ForceMode2D.Impulse);
            }

            //Blast
            if (Input.GetAxis("BlastP1Joy") < 0) // xbox left trigger
            {
                m_BlastLight.gameObject.SetActive(true);
                m_IsBlasting = true;
            }
            else
            {
                m_BlastLight.gameObject.SetActive(false);
                m_IsBlasting = false;
            }

            //Shoot
            if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                m_Shoot.gameObject.SetActive(true);
                m_IsShooting = true;
            }
            else if (Input.GetKeyUp(KeyCode.Joystick1Button2))
            {
                m_Shoot.gameObject.SetActive(false);
                m_IsShooting = false;
            }


            // update light
            if (m_AuraLight != null)
                m_AuraLight.intensity = Mathf.Lerp(m_MinAuraIntensity, m_MaxAuraIntensity, m_Energy / m_MaxEnergy);

            // die slowly
            m_Energy -= m_EnergyLossPerSecond * Time.deltaTime;

            // die less slowly if in contact with nemesis
            if (m_AttackingNemesis)
            {
                float loss = m_Nemesis.m_EnergySuckPerSecond * Time.deltaTime;
                m_Energy -= loss;
                m_Nemesis.Heal(loss);
            }

            // die less slowly if blasting
            if (m_IsBlasting)
            {
                float loss = m_BlastSuckPerSecond * Time.deltaTime;
                m_Energy -= loss;

                //nemesis sucks light if touched by blast
                Vector2 dir = m_Nemesis.transform.position - this.transform.position;
                dir.Normalize();
                RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, dir);
                bool nemesisTouched = false;
                for (int i = 0; i < hits.Length; ++i )
                {
                    if (hits[i].collider == m_Collider)
                        continue;

                    if (hits[i].collider.gameObject.GetComponent<Nemesis>() != null)
                        nemesisTouched = true;
                    break;
                }
                if (nemesisTouched)
                    m_Nemesis.Heal(loss * 0.5f);
            }

            // die less slowly if shooting
            if (m_IsShooting)
            {
                float loss = m_ShootSuckPerSecond * Time.deltaTime;
                m_Energy -= loss;
                
                if (axisValueX != 0 || axisValueY != 0)
                {
                    m_Shoot.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, m_ShootAngle));
                }
                LaserBeam beam = m_Shoot.GetComponentInChildren<LaserBeam>();
                if (beam != null)
                {
                    GameObject go = beam.GetHitObject();
                    if (go != null)
                    {
                        Nemesis nemesis = go.GetComponent<Nemesis>();
                        if (nemesis != null)
                        {
                            nemesis.Stun();
                            nemesis.Heal(loss);
                        }
                    }
                }
            }
        }
        else if (m_IsBlasting)
        {
            m_BlastLight.gameObject.SetActive(false);
            m_IsBlasting = false;
        }
        else if (m_IsShooting)
        {
            m_Shoot.gameObject.SetActive(false);
            m_IsShooting = false;
        }
	}
}
