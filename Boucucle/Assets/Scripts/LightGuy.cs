using UnityEngine;
using System.Collections;

public class LightGuy : MonoBehaviour {

    private Rigidbody2D m_RigidBody;

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        if (m_RigidBody == null)
        {
            Debug.LogError("No rigidbody 2D attached to lightguy !");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        float axisValue = Input.GetAxis("Horizontal");
        m_RigidBody.AddForce(new Vector2(axisValue, 0), ForceMode2D.Impulse);

	}
}
