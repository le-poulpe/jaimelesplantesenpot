using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nemesis : MonoBehaviour {

    private Rigidbody2D m_RigidBody;
    private Collider2D m_Collider;
    private bool m_CanJump = false;
    private float m_StunTimer;
    private float m_PlayStepSoundTimer;
    private float m_PlayGruntSoundTimer;
	private Vector3	m_LastPosition;
    private float m_Energy;
    private bool m_IsRushing = false;
    private bool m_IsOnLadder = false;

    public float m_EnergySuckPerSecond = 80;
    public float m_JumpImpulse = 5;
    public float m_MoveSpeed = 1;
    public float m_MoveSpeedPostRush = 1;   
	public float m_RushSpeed = 1;   //Vitesse augmentée en rush
	public float m_StunTime = 1.0f;
	public float m_BeamRepel = 1.0f;
	public float m_BlastStunTime = 0.125f;
	public float m_BlastRepel = 1.0f;
    public float m_StepRate = 1.0f;
    public float m_GruntRate = 1.0f;
    public float m_MaxEnergy = 83f;
    public float m_EnergyLossPerSecond = 0.1f;
    public float m_RushSuckPerSecond = 15;  //Coût du rush
    public float m_StartGruntingEnergy = 30;
    public float m_RotationSpeed = 400;
    public float m_LadderClimbSpeed = 1;
    public float m_MeshRotateSpeed1 = 1;
    public float m_MeshRotateSpeed2 = 1;
    public float m_MeshRotateSpeed3 = 1;
	public float m_MeshRotateSpeed4	= 1;
	public float m_MeshRotateSpeed5	= 1;
	public bool m_Flying = true;
    public Light m_RushLight = null;
	public GameObject m_DrainingLight = null;
	public GameObject m_StunLight = null;
    public GameObject m_MeshRotate1;
    public GameObject m_MeshRotate2;
    public GameObject m_MeshRotate3;
    public GameObject m_MeshRotate4;
	public GameObject m_MeshRotate5;

    public AudioSource m_StepSource;
    public AudioSource m_GruntSource;
	
	public bool IsDead()
	{
		return m_Energy <= 0;
	}

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_Collider = GetComponentInChildren<Collider2D>();
        m_CanJump = false;
        if (m_RigidBody == null)
        {
            Debug.LogError("No rigidbody 2D attached to nemesis !");
        }
        if (m_RushLight == null)
        {
            Debug.LogError("No rush light 2D attached to lightguy !");
        }
        if (m_DrainingLight == null)
        {
            Debug.LogError("No draining light 2D attached to lightguy !");
        }
		if (m_StunLight == null)
        {
            Debug.LogError("No stun light 2D attached to lightguy !");
        }
        else
        {
            m_RushLight.gameObject.SetActive(false);
            m_DrainingLight.gameObject.SetActive(false);			
            m_IsRushing = false;
            
	        m_StunTimer = 0;
	        m_PlayStepSoundTimer = 1;
			m_LastPosition = new Vector2(transform.position.x, transform.position.y);
	        m_Energy = m_MaxEnergy;
        }

		if (m_Flying)
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
        //m_Energy -= m_EnergyLossPerSecond * Time.deltaTime;

        m_CanJump = false;
        Collider2D[] jumpColliders = Physics2D.OverlapAreaAll(m_Collider.transform.position + new Vector3(-0.2f, -m_Collider.bounds.extents.y - 0.1f, 0),
                                             m_Collider.transform.position + new Vector3(0.2f, -m_Collider.bounds.extents.y + 0.1f, 0));
        foreach (Collider2D col in jumpColliders)
        {
            if (col != m_Collider && !col.isTrigger)
            {
                m_CanJump = true;
                break;
            }
        }

        m_MeshRotate1.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed1, Time.deltaTime * m_MeshRotateSpeed1, Time.deltaTime * m_MeshRotateSpeed1));
        m_MeshRotate2.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed2, Time.deltaTime * m_MeshRotateSpeed2, Time.deltaTime * m_MeshRotateSpeed2));
        m_MeshRotate3.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed3, Time.deltaTime * m_MeshRotateSpeed3, Time.deltaTime * m_MeshRotateSpeed3));
		m_MeshRotate4.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed4, Time.deltaTime * m_MeshRotateSpeed4, Time.deltaTime * m_MeshRotateSpeed4));
		m_MeshRotate5.transform.Rotate(new Vector3(Time.deltaTime * m_MeshRotateSpeed5, Time.deltaTime * m_MeshRotateSpeed5, Time.deltaTime * m_MeshRotateSpeed5));
        
		if (m_Energy > 0)
        {	
			LightGuy lightGuy = gameObject.GetComponentInParent<LightGuy>();
            if (m_StunTimer > 0)
			{
				m_StunLight.gameObject.SetActive(true);
                m_StunTimer -= Time.deltaTime;
			}
            else
            {
                m_StunLight.gameObject.SetActive(false);
				// update controls
				float axisValueX = Input.GetAxis("HorizontalP2Joy");
				if (axisValueX == 0)
					axisValueX = Input.GetAxis("HorizontalP2Keyboard");
				float axisValueY = Input.GetAxis("VerticalP2Joy");
				if (axisValueY == 0)
					axisValueY = Input.GetAxis("VerticalP2Keyboard");
                
                //Rush
				if (Input.GetAxis("BlastP2Joy") < 0 || Input.GetAxis("BlastP2Keyboard") < 0) // xbox left trigger
                {
                    m_RushLight.gameObject.SetActive(true);
                    m_IsRushing = true;
                }
                
                else
                {
                    m_RushLight.gameObject.SetActive(false);
                    m_IsRushing = false;
                }                
                
                if (m_IsRushing)
                {
                    float loss = m_RushSuckPerSecond * Time.deltaTime;
                    //m_Energy -= loss;
                    m_MoveSpeed = m_RushSpeed;
                }
                else
                {
                    m_RushLight.gameObject.SetActive(false);
                    m_IsRushing = false;
                    m_MoveSpeed = m_MoveSpeedPostRush;
                }

				if (m_Flying)
				{
					if (axisValueX != 0 || axisValueY != 0)
					{
						if (axisValueX != 0)
							m_Collider.transform.Rotate(new Vector3(0, 0, 1), axisValueX * -m_RotationSpeed * Time.deltaTime);
						m_RigidBody.AddForce(new Vector2(axisValueX * m_MoveSpeed, -axisValueY * m_MoveSpeed), ForceMode2D.Impulse);
					}
					
					// footstep sound
					if (m_StepSource != null)
					{
						Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
						Vector2 delta = new Vector2(currentPosition.x - m_LastPosition.x,
						                            currentPosition.y - m_LastPosition.y);
						float velocity = delta.magnitude / Time.deltaTime;
						if (Mathf.Abs(velocity) > 0.1)
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
				}
				else
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
				}
            }

            if (m_GruntSource != null)
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
                        m_GruntSource.pitch = 1 + Random.RandomRange(-0.1f, 0.1f);
                        m_GruntSource.Play();
                    }
                }
            }
    
            m_LastPosition = new Vector2(transform.position.x, transform.position.y);

		}
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
        m_Energy += energy;
    }

    public Collider2D GetCollider()
    {
        return m_Collider;
    }
}
