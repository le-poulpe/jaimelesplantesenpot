using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightGuy : MonoBehaviour {

    private Rigidbody2D m_RigidBody;
    private bool        m_CanJump = false;
    private List<GameObject> m_CollidingStuff;

    public float m_JumpImpulse;

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_CanJump = false;
        m_CollidingStuff = new List<GameObject>();
        if (m_RigidBody == null)
        {
            Debug.LogError("No rigidbody 2D attached to lightguy !");
        }
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
        float axisValue = Input.GetAxis("HorizontalP1Joy");
        m_RigidBody.AddForce(new Vector2(axisValue, 0), ForceMode2D.Impulse);
        if ( Input.GetKeyDown ( KeyCode.Joystick1Button0 ) && m_CanJump)
        {
            m_CanJump = false;
            m_RigidBody.AddForce(new Vector2(0, m_JumpImpulse), ForceMode2D.Impulse);
        }

	}
}
