using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightGuy : MonoBehaviour {

    private Rigidbody2D m_RigidBody;
    private Collider2D m_Collider;
    private bool m_IsOnGround = false; // DO NOT SET DIRECTLY ! use SetOnGround
	private bool m_CanJump = false; // like m_IsOnGround, but more tolerant
    private Nemesis m_Nemesis;
    private float m_Energy;
    private bool m_IsBlasting = false;
    private bool m_IsShooting = false;
    private float m_ShootAngle = 0f;
    private bool m_IsOnLadder = false;
	private bool m_JumpButtonPressed = false; // used to communicate between Update and FixedUpdate
	private float m_Gravity;
	
	private float m_AxisValueX;
	private float m_AxisValueY;

    public GameObject m_Cursor;
	private bool m_AttackingNemesis = false;
	
	public float m_JumpImpulse = 35;
	public float m_JumpImpulseX = 5;
	public float m_LadderJumpImpulse = 12;
	public float m_LadderJumpImpulseX = 20;
    public float m_MoveSpeed = 1;
    public float m_MaxEnergy = 100;
    public float m_EnergyLossPerSecond = 0.1f;
    public float m_BlastSuckPerSecond = 15;
	public float m_BlastStunRange = 3.0f;
    public float m_ShootSuckPerSecond = 10;
    public float m_LadderClimbSpeed = 1;
	public float m_BlastStunDistance = 1.25f;
	public float m_DyingFeedbackPitch = 0.45f;
	public float m_DyingFeedbackVolume = 1;
	public float m_FlowerPotHeal = 1;
	

    public Light m_AuraLight = null;
    public Light m_BlastLight = null;
	public AudioSource m_DyingLightGuyFeedback = null;
	public AudioSource m_DisappearSound = null;
    public GameObject m_Shoot = null;
    public float m_MinAuraIntensity = 0;
    public float m_MaxAuraIntensity = 2;

	Color m_BlastStartColor;
	public float m_BlastAttenuationFactor = 5;

	public bool IsDead()
	{
		return m_Energy <= 0;
	}

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_Collider = GetComponentInChildren<Collider2D>();
        m_IsShooting = false;
        m_Energy = m_MaxEnergy;
        m_Nemesis = FindObjectOfType<Nemesis>();
		m_DyingLightGuyFeedback.gameObject.SetActive(false);
		m_DisappearSound.gameObject.SetActive(true);

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

		m_AxisValueX = m_AxisValueY = 0;
		m_BlastStartColor = m_BlastLight.color;
		m_Gravity = m_RigidBody.gravityScale;
		SetOnGround(false);

	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        Nemesis nemesis = coll.gameObject.GetComponentInParent<Nemesis>();
        if (nemesis != null)
            m_AttackingNemesis = true;
        else if (coll.gameObject.GetComponentInParent<PotDeFleur>() != null)
        {
			Object.Destroy(coll.gameObject);
			m_Energy = m_Energy + m_FlowerPotHeal;		//Small heal when LG get his hands on a flower pot 
			m_DisappearSound.Play();
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        Nemesis nemesis = coll.gameObject.GetComponentInParent<Nemesis>();
        if (nemesis != null)
            m_AttackingNemesis = false;
    }

	void FixedUpdate()
	{
		// only physics-related stuff here
		bool jumpPressedEvent = false;
		if (m_JumpButtonPressed)
		{
			m_JumpButtonPressed = false;
			jumpPressedEvent = true;
		}
		
		//check if on ground
		SetOnGround(false);
		{
			Collider2D[] groundColliders = Physics2D.OverlapAreaAll(m_Collider.bounds.center + new Vector3(-0.2f, -m_Collider.bounds.extents.y - 0.1f, 0),
			                                                        m_Collider.bounds.center + new Vector3( 0.2f, 0, 0));
	        foreach (Collider2D col in groundColliders)
	        {
	            if (col != m_Collider && !col.isTrigger)
	            {
					SetOnGround(true);
					break;
	            }
	        }
		}

		// check if can jump. Need to separate canJump and isOnGround to be more tolerant because gravity is disabled when on ground.
		m_CanJump = IsOnGround();
		if (!m_CanJump)
		{
			// same check as above, but more tolerant
			Collider2D[] groundColliders = Physics2D.OverlapAreaAll(m_Collider.bounds.center + new Vector3(-0.4f, -m_Collider.bounds.extents.y - 0.4f, 0),
			                                                        m_Collider.bounds.center + new Vector3( 0.4f, 0, 0));
			foreach (Collider2D col in groundColliders)
			{
				if (col != m_Collider && !col.isTrigger)
				{
					m_CanJump = true;
					break;
				}
			}
		}

		if (!IsDead())
		{
			if (m_IsOnLadder)
			{
				// Jump
				if (jumpPressedEvent)
				{
					m_IsOnLadder = false;
					m_RigidBody.isKinematic = false;
					m_RigidBody.AddForce(new Vector2((m_LadderJumpImpulseX + m_LadderJumpImpulse) * m_AxisValueX, -m_AxisValueY * m_LadderJumpImpulse), ForceMode2D.Impulse);
				}
				else
				{
					Vector3 toAdd = new Vector3(0, -m_AxisValueY * Time.fixedDeltaTime * m_LadderClimbSpeed, 0);
					transform.position = transform.position + toAdd;
				}
			}
			else
			{
				//move left/right
				m_RigidBody.AddForce(new Vector2(m_AxisValueX * m_MoveSpeed, 0), ForceMode2D.Impulse);
				
				// Jump
				if (jumpPressedEvent && m_CanJump)
				{
					m_RigidBody.AddForce(new Vector2(m_AxisValueX * m_JumpImpulseX, m_JumpImpulse), ForceMode2D.Impulse);
				}
			}

			if (m_IsBlasting)
			{
				//nemesis stun if touched by blast
				Vector2 delta = m_Nemesis.transform.position - this.transform.position;
				if (delta.magnitude < m_BlastStunDistance)
				{
					// Raycast to check if touched
					RaycastHit2D[] hit;
					Vector2 dir = m_Nemesis.transform.position - transform.position;
					dir.Normalize();
					hit = Physics2D.RaycastAll(transform.position, dir, m_BlastStunDistance);
					for (int i = 0; i < hit.Length; ++i)
					{
						if (!hit[i].collider.isTrigger)
						{
							if (hit[i].collider == m_Collider)
								continue;
							
							GameObject go = hit[i].collider.gameObject;
							if (go != null)
							{
								Nemesis nemesis = go.GetComponentInParent<Nemesis>();
								if (nemesis != null)
								{
									nemesis.StunByBlast();
									nemesis.Repel(dir, true);
								}
								break;
							}
						}
					}
				}
			}
			
			// die less slowly if shooting
			if (m_IsShooting)
			{
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
							nemesis.Repel(beam.GetDir());									
						}
					}
				}
			}
		}
	}
	
	void Update ()
	{
		// update controls
		m_AxisValueX = Input.GetAxis("HorizontalP1Joy");
		if (m_AxisValueX == 0)
			m_AxisValueX = Input.GetAxis("HorizontalP1Keyboard");
		m_AxisValueY = Input.GetAxis("VerticalP1Joy");
		if (m_AxisValueY == 0)
			m_AxisValueY = Input.GetAxis("VerticalP1Keyboard");

		if (Mathf.Abs(m_AxisValueX) == Mathf.Abs(m_AxisValueY) && Mathf.Abs(m_AxisValueX) == 1)
		{
			// got keyboard input : normalize it
			m_AxisValueX *= 0.70710678118654752440084436210485f;
			m_AxisValueY *= 0.70710678118654752440084436210485f;
		}

		bool jumpJoy = Input.GetKeyDown(KeyCode.Joystick1Button0);
		bool jumpKey = Input.GetKeyDown("space");
		bool jump = jumpJoy || jumpKey;
		if (jump)
			m_JumpButtonPressed = true; // communicate to physics update to jump

		if (m_Energy > 0.1 && m_Energy < m_MaxEnergy * 0.24f && m_AttackingNemesis == false)	//Stress inducing audio when LG is dying
		{
			m_DyingLightGuyFeedback.gameObject.SetActive(true);
			m_DyingLightGuyFeedback.pitch = m_DyingFeedbackPitch - (m_Energy * 0.04f);
			m_DyingLightGuyFeedback.volume = m_DyingFeedbackVolume - (m_Energy * 0.08f);
		}
		else
		{
			m_DyingLightGuyFeedback.gameObject.SetActive(false);
		}
		
        if (!IsDead())
        {
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
                if (m_AxisValueY < -0.5 && Mathf.Abs(m_AxisValueX) < 0.5 && !m_IsBlasting && !m_IsShooting)
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
			
			if (m_AxisValueX != 0 || m_AxisValueY != 0)
				m_ShootAngle = Mathf.Atan2(-m_AxisValueY, m_AxisValueX) * 180 / Mathf.PI;

			if (m_Cursor != null)
				m_Cursor.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, m_ShootAngle));
			
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
				m_Shoot.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, m_ShootAngle)); // yes, necessary here too
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
			
			if (m_IsBlasting)
			{
				float loss = m_BlastSuckPerSecond * Time.fixedDeltaTime;
				m_Energy -= loss;
				
				float t = (m_MaxEnergy - m_Energy) / m_MaxEnergy;
				Color newColor = new Color();
				newColor.r = Mathf.Lerp(m_BlastStartColor.r, m_BlastStartColor.r / m_BlastAttenuationFactor, t);
				newColor.g = Mathf.Lerp(m_BlastStartColor.g, m_BlastStartColor.g / m_BlastAttenuationFactor, t);
				newColor.b = Mathf.Lerp(m_BlastStartColor.b, m_BlastStartColor.b / m_BlastAttenuationFactor, t);
				m_BlastLight.color = newColor;
			}
			
			// die less slowly if shooting
			if (m_IsShooting)
			{
				float loss = m_ShootSuckPerSecond * Time.deltaTime;
				m_Energy -= loss;
				m_Shoot.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, m_ShootAngle));
			}
        }
	}

	void SetOnGround(bool isOnGround)
	{
		m_IsOnGround = isOnGround;
		m_RigidBody.gravityScale = m_IsOnGround ? 0 : m_Gravity;
	}

	bool IsOnGround()
	{
		return m_IsOnGround;
	}
}
