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

    public float m_EnergySuckPerSecond = 80;
    public float m_JumpImpulse = 5;
    public float m_MoveSpeed = 1;
    public float m_StunTime = 1.0f;
    public float m_StepRate = 1.0f;
    public float m_GruntRate = 1.0f;
    public float m_MaxEnergy = 83f;
    public float m_EnergyLossPerSecond = 0.1f;
    public float m_StartGruntingEnergy = 30;

    public AudioSource m_StepSource;
    public AudioSource m_GruntSource;

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_Collider = this.collider2D;
        m_CanJump = false;
        if (m_RigidBody == null)
        {
            Debug.LogError("No rigidbody 2D attached to nemesis !");
        }
        m_StunTimer = 0;
        m_PlayStepSoundTimer = 1;
		m_LastPosition = new Vector2(transform.position.x, transform.position.y);
        m_Energy = m_MaxEnergy;

    }
	
	// Update is called once per frame
	void Update ()
    {
        m_Energy -= m_EnergyLossPerSecond * Time.deltaTime;

        m_CanJump = Physics2D.OverlapCircleAll(m_Collider.transform.position + new Vector3(0, -m_Collider.bounds.extents.y, 0), 0.2f).Length > 1; // will collide at least with self
        if (m_Energy > 0)
        {
            if (m_StunTimer > 0)
                m_StunTimer -= Time.deltaTime;
            else
            {
                float axisValue = Input.GetAxis("HorizontalP2Joy");
                m_RigidBody.AddForce(new Vector2(axisValue * m_MoveSpeed, 0), ForceMode2D.Impulse);

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
             

                if (Input.GetKeyDown(KeyCode.Joystick2Button0) && m_CanJump)
                {
                    m_CanJump = false;
                    m_RigidBody.AddForce(new Vector2(0, m_JumpImpulse), ForceMode2D.Impulse);
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

    public void Heal(float energy)
    {
        m_Energy += energy;
    }
}
