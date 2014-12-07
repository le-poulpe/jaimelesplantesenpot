using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nemesis : MonoBehaviour {

    private Rigidbody2D m_RigidBody;
    private bool m_CanJump = false;
    private List<GameObject> m_CollidingStuff;
    private float m_StunTimer;

    public float m_EnergySuckPerSecond = 80;
    public float m_JumpImpulse = 5;
    public float m_MoveSpeed = 1;
    public float m_StunTime = 1.0f;

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

            if (Input.GetKeyDown(KeyCode.Joystick2Button0) && m_CanJump)
            {
                m_CanJump = false;
                m_RigidBody.AddForce(new Vector2(0, m_JumpImpulse), ForceMode2D.Impulse);
            }
        }
	}

    public void Stun()
    {
        m_StunTimer = m_StunTime;
    }
}
