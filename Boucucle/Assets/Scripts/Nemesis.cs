using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nemesis : MonoBehaviour {

    private Rigidbody2D m_RigidBody;
    private bool m_CanJump = false;
    private List<GameObject> m_CollidingStuff;
    private float m_StunTimer;
    private float m_PlayStepSoundTimer;
	private Vector3	m_LastPosition;

    public float m_EnergySuckPerSecond = 80;
    public float m_JumpImpulse = 5;
    public float m_MoveSpeed = 1;
    public float m_StunTime = 1.0f;
    public float m_StepRate = 1.0f;

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_CanJump = false;
        m_CollidingStuff = new List<GameObject>();
        if (m_RigidBody == null)
        {
            Debug.LogError("No rigidbody 2D attached to nemesis !");
        }
        m_StunTimer = 0;
        m_PlayStepSoundTimer = 1;
		m_LastPosition = new Vector2(transform.position.x, transform.position.y);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.bounds.max.y < this.collider2D.bounds.min.y)
        {
            m_CollidingStuff.Add(coll.gameObject);
            m_CanJump = true;
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        m_CollidingStuff.Remove(coll.gameObject);
        m_CanJump = m_CollidingStuff.Count > 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_StunTimer > 0)
            m_StunTimer -= Time.deltaTime;
        else
        {
            float axisValue = Input.GetAxis("HorizontalP2Joy");
            m_RigidBody.AddForce(new Vector2(axisValue * m_MoveSpeed, 0), ForceMode2D.Impulse);

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
                    audio.pitch = 1 + Random.RandomRange(-0.1f, 0.1f);
                    audio.Play();
                }
            }
                

            if (Input.GetKeyDown(KeyCode.Joystick2Button0) && m_CanJump)
            {
                m_CanJump = false;
                m_RigidBody.AddForce(new Vector2(0, m_JumpImpulse), ForceMode2D.Impulse);
            }
        }
		
		m_LastPosition = new Vector2(transform.position.x, transform.position.y);
	}

    public void Stun()
    {
        m_StunTimer = m_StunTime;
    }
}
