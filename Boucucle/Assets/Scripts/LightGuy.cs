using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightGuy : MonoBehaviour {

    private Rigidbody2D m_RigidBody;
    private Collider2D m_Collider;
    private bool m_CanJump = false;
    private Nemesis m_Nemesis;
    private float m_Energy;
    private bool m_IsBlasting = false;
    private bool m_IsShooting = false;
    private float m_ShootAngle = 0f;
    private bool m_IsOnLadder = false;

    public GameObject m_Cursor;
	private bool m_AttackingNemesis = false;

    public float m_JumpImpulse = 5;
    public float m_MoveSpeed = 1;
    public float m_MaxEnergy = 100;
    public float m_EnergyLossPerSecond = 0.1f;
    public float m_BlastSuckPerSecond = 15;
	public float m_BlastStunRange = 3.0f;
    public float m_ShootSuckPerSecond = 10;
    public float m_RotationSpeed = 400f;
    public float m_LadderClimbSpeed = 1;

    public Light m_AuraLight = null;
    public Light m_BlastLight = null;
	public GameObject m_GotPotFeedback = null;
	public GameObject m_DyingLightGuyFeedback = null;
    public GameObject m_Shoot = null;
    public float m_MinAuraIntensity = 0;
    public float m_MaxAuraIntensity = 2;

	public bool IsDead()
	{
		return m_Energy <= 0;
	}

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_Collider = GetComponentInChildren<Collider2D>();
        m_CanJump = false;
        m_IsShooting = false;
        m_Energy = m_MaxEnergy;
        m_Nemesis = FindObjectOfType<Nemesis>();
		m_GotPotFeedback.gameObject.SetActive(false);

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
		if (m_GotPotFeedback == null)
        {
            Debug.LogError("No pot feedback object attached to lightguy !");
        }
		if (m_DyingLightGuyFeedback == null)
        {
            Debug.LogError("No low energy feedback object attached to lightguy !");
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
        Nemesis nemesis = coll.gameObject.GetComponentInParent<Nemesis>();
        if (nemesis != null)
            m_AttackingNemesis = true;
        else if (coll.gameObject.GetComponentInParent<PotDeFleur>() != null)
        {
			Object.Destroy(coll.gameObject);		
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        Nemesis nemesis = coll.gameObject.GetComponentInParent<Nemesis>();
        if (nemesis != null)
            m_AttackingNemesis = false;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        m_CanJump = false;
        Collider2D[] jumpColliders = Physics2D.OverlapAreaAll(m_Collider.transform.position + new Vector3(-0.2f, -m_Collider.bounds.extents.y - 0.1f, 0),
                                             m_Collider.transform.position + new Vector3( 0.2f, -m_Collider.bounds.extents.y + 0.1f, 0));
        foreach (Collider2D col in jumpColliders)
        {
            if (col != m_Collider && !col.isTrigger)
            {
                m_CanJump = true;
                break;
            }
        }
		
		if (m_Energy > 1 && m_Energy < 13.5)
		{
		m_DyingLightGuyFeedback.gameObject.SetActive(true);
		}
		else
		{
		m_DyingLightGuyFeedback.gameObject.SetActive(false);
		}
		
        if (m_Energy > 0)
        {
            // update controls
            float axisValueX = Input.GetAxis("HorizontalP1Joy");
			if (axisValueX == 0)
				axisValueX = Input.GetAxis("HorizontalP1Keyboard");
			float axisValueY = Input.GetAxis("VerticalP1Joy");
			if (axisValueY == 0)
				axisValueY = Input.GetAxis("VerticalP1Keyboard");

            // check ladder
            bool collidesLadder = false;
            Collider2D[] ladderColliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x-0.2f, transform.position.y - 0.2f),
			                                                        new Vector2(transform.position.x+0.2f, transform.position.y + 0.2f));
            GameObject ladder = null;

            for (int i = 0; i < ladderColliders.Length; ++i)
            {
                if (ladderColliders[i].gameObject.tag.Equals("Ladder"))
                {
                    collidesLadder = true;
                    ladder = ladderColliders[i].gameObject;
                    break;
                }
            }
            if (collidesLadder && !m_IsOnLadder)
            {
                if (axisValueY < -0.5 && Mathf.Abs(axisValueX) < 0.5 && !m_IsBlasting && !m_IsShooting)
                {
                    //up near a ladder : get on ladder
                    m_IsOnLadder = true;
                    m_RigidBody.isKinematic = true;
					transform.position = new Vector3(ladder.transform.position.x, transform.position.y, transform.position.z);
                }
            }
            else if (!collidesLadder && m_IsOnLadder)
            {
                m_RigidBody.isKinematic = false;
                m_IsOnLadder = false;
            }

            if (m_IsOnLadder)
            {
                // Jump
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown("space"))
                {
                    m_IsOnLadder = false;
                    m_RigidBody.isKinematic = false;
                }
                else
                {
                    Vector3 toAdd = new Vector3(0, -axisValueY * Time.deltaTime * m_LadderClimbSpeed, 0);
                    transform.position = transform.position + toAdd;
                }
            }
            else
            {
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
                if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown("space") && m_CanJump)
                {
                    m_CanJump = false;
                    m_RigidBody.AddForce(new Vector2(0, m_JumpImpulse), ForceMode2D.Impulse);
                }

                //Blast
				if (Input.GetAxis("BlastP1Joy") < 0 || Input.GetAxis("BlastP1Keyboard") < 0) // xbox left trigger
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
                if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown("left shift"))
                {
                    m_Shoot.gameObject.SetActive(true);
                    m_IsShooting = true;
                }
                else if (Input.GetKeyUp(KeyCode.Joystick1Button2) || Input.GetKeyUp("left shift"))
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
                    for (int i = 0; i < hits.Length; ++i)
                    {
                        if (hits[i].collider == m_Collider)
                            continue;
							
						if (hits[i].collider)
					
                        if (hits[i].collider.gameObject.GetComponentInParent<Nemesis>() != null)
							{
								nemesisTouched = true;
							}
                        break;
                    }
                    /*if (nemesisTouched)
                        m_Nemesis.Heal(loss * 0.5f);
						m_Nemesis.StunByBlast();*/
                }

                // die less slowly if shooting
                if (m_IsShooting)
                {
                    float loss = m_ShootSuckPerSecond * Time.deltaTime;
                    m_Energy -= loss;
                    m_Shoot.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, m_ShootAngle));
                    LaserBeam beam = m_Shoot.GetComponentInChildren<LaserBeam>();
                    if (beam != null)
                    {
                        GameObject go = beam.GetHitObject();
                        if (go != null)
                        {
                            Nemesis nemesis = go.GetComponentInParent<Nemesis>();
                            if (nemesis != null)
                            {
                                nemesis.Stun();
                                nemesis.Heal(loss);													
                            }
                        }
                    }
                }
            }
        }
	}
}
