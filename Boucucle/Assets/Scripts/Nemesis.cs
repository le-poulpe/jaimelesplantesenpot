using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nemesis : MonoBehaviour {

	enum E_NemState
	{
		NS_NORMAL,
		NS_RUSH,
		NS_SNEAK,
		NS_STUN
	};

    private Rigidbody2D m_RigidBody;
    private Collider2D m_Collider;
    private float m_StunTimer;
    private float m_PlayStepSoundTimer;
	private Vector3	m_LastPosition;
    private float m_Energy;
	private E_NemState m_State;
	private float m_CurrentSpeed;
	private float m_RushTimer;

	
	
	//step sound
	public AudioSource m_StepSource;
	public float m_MinStepFrequency = 1.0f;
	public float m_MaxStepFrequency = 3.0f;
	public float m_MinStepVolume = 0.3f;
	public float m_MaxStepVolume = 1.0f;
	public float m_MinStepSpeed = 0.2f; //below, no footstep
	public float m_MaxStepSpeed = 1; //over, footstep does not change

    public float m_EnergySuckPerSecond = 80;
    public float m_MoveSpeed = 1;
	public float m_RushSpeed = 1;   //Vitesse augmentée en rush
	public float m_RushEndSpeed = 0.2f;
	public float m_RushTime = 1.0f;
	public float m_RushCoolDown = 0.5f;
	public float m_SneakSpeed = 0.3f;
	public float m_StunTime = 1.0f;
	public float m_StunLightIntensity = 1.0f;
	public float m_StunLightMaxIntensity = 1.0f;
	public float m_BeamRepel = 1.0f;
	public float m_BlastStunTime = 0.125f;
	public float m_BlastRepel = 1.0f;
    public float m_MaxEnergy = 83f;
    public float m_RushSuckPerSecond = 15;  //Coût du rush
    public float m_RotationSpeed = 400;
    public float m_MeshRotateSpeed1 = 1;
    public float m_MeshRotateSpeed2 = 1;
    public float m_MeshRotateSpeed3 = 1;
	public float m_MeshRotateSpeed4	= 1;
	public float m_MeshRotateSpeed5	= 1;
	public float m_MeshRotateSpeed6	= 1;
	public Light m_RushLight = null;
	public Light m_StunLight = null;
	public GameObject m_StunShockSound = null;
	public GameObject m_DrainingLight = null;
    public GameObject m_MeshRotate1;
    public GameObject m_MeshRotate2;
    public GameObject m_MeshRotate3;
	public GameObject m_MeshRotate4;
	public GameObject m_MeshRotate5;
	public GameObject m_MeshRotate6;
	public GameObject m_SneakMesh = null;
	public AudioSource m_SneakSound = null;
	public GameObject m_NormaMesh = null;

    
	//walking part
	//private bool m_CanJump = false;
	//private bool m_IsOnLadder = false;
	//public float m_LadderClimbSpeed = 1;
	//public float m_JumpImpulse = 5;
	//public bool m_Flying = true;

	//energy part
	//public AudioSource m_GruntSource;
	//public float m_StartGruntingEnergy = 30;
	//public float m_GruntRate = 1.0f;
	//public float m_EnergyLossPerSecond = 0.1f;
	//private float m_PlayGruntSoundTimer;

	/* energy considerations disabled for nemesis
	public bool IsDead()
	{
		return m_Energy <= 0;
	}*/

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_Collider = GetComponentInChildren<Collider2D>();
        //m_CanJump = false; jump disabled
		m_State = E_NemState.NS_NORMAL;
		m_CurrentSpeed = m_MoveSpeed;
        if (m_RigidBody == null)
        {
            Debug.LogError("No rigidbody 2D attached to nemesis !");
        }
        if (m_RushLight == null)
        {
            Debug.LogError("No rush light attached to nemesis !");
        }
        if (m_DrainingLight == null)
        {
            Debug.LogError("No draining light attached to nemesis !");
        }
		if (m_StunLight == null)
        {
            Debug.LogError("No stun light attached to nemesis !");
        }
		if (m_StunShockSound == null)
        {
            Debug.LogError("No stun shock sound attached to nemesis !");
        }

	    m_RushLight.gameObject.SetActive(false);
		m_DrainingLight.gameObject.SetActive(false);
		m_SneakMesh.gameObject.SetActive(false);
		m_StunLight.gameObject.SetActive(false);
		m_StunShockSound.gameObject.SetActive(false);
    
        m_StunTimer = 0;
		m_RushTimer = 0;
        m_PlayStepSoundTimer = 1;
		m_LastPosition = new Vector2(transform.position.x, transform.position.y);
        m_Energy = m_MaxEnergy;
		
		//if (m_Flying)
		m_RigidBody.gravityScale = 0;

    }
	
	void OnCollisionEnter2D(Collision2D coll)
    {
        LightGuy lightGuy = coll.gameObject.GetComponentInParent<LightGuy>();
        if (lightGuy != null)
            m_DrainingLight.gameObject.SetActive(true);
    }
	
    void OnCollisionExit2D(Collision2D coll)
    {
        LightGuy lightGuy = coll.gameObject.GetComponentInParent<LightGuy>();
        if (lightGuy != null)
            m_DrainingLight.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		//m_Energy -= m_EnergyLossPerSecond * Time.deltaTime; energy considerations disabled for nemesis
		/*m_CanJump = false; jump disabled
        Collider2D[] jumpColliders = Physics2D.OverlapAreaAll(m_Collider.transform.position + new Vector3(-0.2f, -m_Collider.bounds.extents.y - 0.1f, 0),
                                             m_Collider.transform.position + new Vector3(0.2f, -m_Collider.bounds.extents.y + 0.1f, 0));
        foreach (Collider2D col in jumpColliders)
        {
            if (col != m_Collider && !col.isTrigger)
            {
                m_CanJump = true;
                break;
            }
        }*/

        m_MeshRotate1.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed1, Time.deltaTime * m_MeshRotateSpeed1, Time.deltaTime * m_MeshRotateSpeed1));
        m_MeshRotate2.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed2, Time.deltaTime * m_MeshRotateSpeed2, Time.deltaTime * m_MeshRotateSpeed2));
        m_MeshRotate3.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed3, Time.deltaTime * m_MeshRotateSpeed3, Time.deltaTime * m_MeshRotateSpeed3));
		m_MeshRotate4.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed4, Time.deltaTime * m_MeshRotateSpeed4, Time.deltaTime * m_MeshRotateSpeed4));
		m_MeshRotate5.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed5, Time.deltaTime * m_MeshRotateSpeed5, Time.deltaTime * m_MeshRotateSpeed5));
		m_MeshRotate6.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed6, Time.deltaTime * m_MeshRotateSpeed6, Time.deltaTime * m_MeshRotateSpeed6));
        
		if (m_Energy > 0)
        {	
			// update controls
			float axisValueX = Input.GetAxis("HorizontalP2Joy");
			if (axisValueX == 0)
				axisValueX = Input.GetAxis("HorizontalP2Keyboard");
			float axisValueY = Input.GetAxis("VerticalP2Joy");
			if (axisValueY == 0)
				axisValueY = Input.GetAxis("VerticalP2Keyboard");
			
			
			if (Mathf.Abs(axisValueX) == Mathf.Abs(axisValueY) && Mathf.Abs(axisValueX) == 1)
			{
				// got keyboard input : normalize it
				axisValueX *= 0.70710678118654752440084436210485f;
				axisValueY *= 0.70710678118654752440084436210485f;
			}

			bool rush = Input.GetAxis("BlastP2Joy") < 0 || Input.GetAxis("BlastP2Keyboard") < 0;
			bool sneak = Input.GetAxis("SneakP2Joy") > 0 || Input.GetAxis("SneakP2Keyboard") < 0;

			//global transition to stun
			if (m_StunTimer > 0)
			{
				m_StunLight.gameObject.SetActive(true);
				m_NormaMesh.gameObject.SetActive(true);
				m_SneakMesh.gameObject.SetActive(false);				
				m_RushLight.gameObject.SetActive(false);
				m_RushTimer = 0;
				m_CurrentSpeed = m_MoveSpeed;
				m_State = E_NemState.NS_STUN;		
				
				if (m_StunTimer > m_StunTime * 0.95f)
				{
					m_StunShockSound.gameObject.SetActive(true);
					m_StunLight.intensity = m_StunLightMaxIntensity;
				}
				
				if (m_StunTimer < m_StunTime * 0.95f)
				{
					m_StunLight.intensity = m_StunLightIntensity * m_StunTimer / 2;
				}
				
				if (m_StunTimer < m_StunTime * 0.90f)
				{
					m_StunShockSound.gameObject.SetActive(false);
				}
			}
			
			//Rush
			switch (m_State)
			{
			case E_NemState.NS_NORMAL:
				if (rush)
				{
					m_RushLight.gameObject.SetActive(true);
					m_RushTimer = m_RushTime + m_RushCoolDown;
					m_State = E_NemState.NS_RUSH;
				}
				else if (sneak)
				{
					m_NormaMesh.gameObject.SetActive(false);
					m_SneakMesh.gameObject.SetActive(true);
					m_SneakSound.Play();
					m_State = E_NemState.NS_SNEAK;
				}
				UpdateMove(axisValueX, axisValueY);
				break;
			case E_NemState.NS_RUSH:
				m_RushTimer -= Time.deltaTime;
				float t = (m_RushTime + m_RushCoolDown - m_RushTimer) / (m_RushTime);
				t = Mathf.Pow(t, 0.3f);
				m_CurrentSpeed = Mathf.Lerp(m_RushSpeed, m_RushEndSpeed, t);
				if (!rush && m_RushTimer <= 0)
				{
					m_RushLight.gameObject.SetActive(false);
					m_CurrentSpeed = m_MoveSpeed;
					m_State = E_NemState.NS_NORMAL;
				}
				UpdateMove(axisValueX, axisValueY);
				break;
			case E_NemState.NS_SNEAK:
				m_CurrentSpeed = m_SneakSpeed;
				if (!sneak)
				{
					m_NormaMesh.gameObject.SetActive(true);
					m_SneakMesh.gameObject.SetActive(false);
					m_State = E_NemState.NS_NORMAL;
					m_CurrentSpeed = m_MoveSpeed;
				}
				UpdateMove(axisValueX, axisValueY);
				break;
			case E_NemState.NS_STUN:
				m_StunTimer -= Time.deltaTime;
				if (m_StunTimer < 0)
				{
					m_StunLight.gameObject.SetActive(false);
					m_State = E_NemState.NS_NORMAL;
				}
				break;
			}

            /*if (m_GruntSource != null) energy code disabled for nemesis
            {
                if (m_Energy < m_StartGruntingEnergy)
                {
                    if (m_PlayGruntSoundTimer > 0)
                    {
                        float gruntSpeedPerSecond = (m_StartGruntingEnergy - m_Energy) / m_StartGruntingEnergy * m_GruntRate;
                        if (gruntSpeedPerSecond > 1)
                            gruntSpeedPerSecond = 1;
                        m_PlayGruntSoundTimer -= Time.deltaTime * gruntSpeedPerSecond;
                    }
                    else
                    {
                        m_PlayGruntSoundTimer = 1;
                        m_GruntSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
                        m_GruntSource.Play();
                    }
                }
            }*/
    
            m_LastPosition = new Vector2(transform.position.x, transform.position.y);

		}
	}

	void UpdateMove(float axisValueX, float axisValueY)
	{
		/*if (m_Flying)
		{*/
		if (axisValueX != 0 || axisValueY != 0)
		{
			if (axisValueX != 0)
				m_Collider.transform.Rotate(new Vector3(0, 0, 1), axisValueX * -m_RotationSpeed * Time.deltaTime);
			m_RigidBody.AddForce(new Vector2(axisValueX * m_CurrentSpeed, -axisValueY * m_CurrentSpeed), ForceMode2D.Impulse);
		}
		
		// footstep sound
		if (m_StepSource != null)
		{
			Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
			Vector2 delta = new Vector2(currentPosition.x - m_LastPosition.x,
			                            currentPosition.y - m_LastPosition.y);
			float velocity = delta.magnitude / Time.deltaTime;
			if (Mathf.Abs(velocity) >= m_MinStepSpeed)
			{
				float t = (velocity - m_MinStepSpeed) / (m_MaxStepSpeed - m_MinStepSpeed);
				if (m_PlayStepSoundTimer > 0)
				{
					t *= t; // frequency should be low most of the time
					m_PlayStepSoundTimer -= Time.deltaTime * Mathf.Lerp(m_MinStepFrequency, m_MaxStepFrequency, t);
				}
				else
				{
					m_PlayStepSoundTimer = 1;
					//t = Mathf.Sqrt(t); // volume should be high most of the time
					m_StepSource.volume = Mathf.Lerp(m_MinStepVolume, m_MaxStepVolume, t);
					m_StepSource.pitch = 1 + Random.Range(-0.1f, 0.1f);
					m_StepSource.Play();
				}
			}
		}
		/*}
		else walking code disabled for nemesis
		{
			LightGuy lightGuy = gameObject.GetComponentInParent<LightGuy>();

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
				if (axisValueY < -0.5 && Mathf.Abs(axisValueX) < 0.5)
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
				if (Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown("right ctrl") || Input.GetKeyDown("page down"))
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
				
				if (axisValueX != 0)
				{
					m_Collider.transform.Rotate(new Vector3(0, 0, 1), axisValueX * -m_RotationSpeed * Time.deltaTime);
					m_RigidBody.AddForce(new Vector2(axisValueX * m_MoveSpeed, 0), ForceMode2D.Impulse);
				}
				
				// footstep sound
				if (m_StepSource != null)
				{
					Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
					float velocity = Mathf.Abs((currentPosition.x - m_LastPosition.x) / Time.deltaTime);
					if (Mathf.Abs(velocity) > 0.1 && m_CanJump)
					{
						if (m_PlayStepSoundTimer > 0)
						{
							m_PlayStepSoundTimer -= Time.deltaTime * velocity * m_StepRate;
						}
						else
						{
							m_PlayStepSoundTimer = 1;
							m_StepSource.pitch = 1 + Random.RandomRange(-0.1f, 0.1f);
							m_StepSource.Play();
						}
					}
				}
				
				
				if ((Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown("right ctrl") || Input.GetKeyDown("page down")) && m_CanJump)
				{
					m_CanJump = false;
					m_RigidBody.AddForce(new Vector2(0, m_JumpImpulse), ForceMode2D.Impulse);
				}
			}
		}*/
	}

    public void Stun()
    {
        m_StunTimer = m_StunTime;
    }
	
	public void Repel(Vector2 dir, bool blast = false)
	{
		float dot = dir.x * m_RigidBody.velocity.x + dir.y * m_RigidBody.velocity.y;
		float mult = blast ? m_BlastRepel : m_BeamRepel;
		if (dot < 0)
			m_RigidBody.velocity -= dir*dot;
		m_RigidBody.AddForce (new Vector2 (dir.x * mult, dir.y * mult));
	}
	
	public void StunByBlast()
    {
		m_StunTimer = m_BlastStunTime;
    }

    public void Heal(float energy)
    {
        //m_Energy += energy; energy code disabled for nemesis
    }

    public Collider2D GetCollider()
    {
        return m_Collider;
    }
}
