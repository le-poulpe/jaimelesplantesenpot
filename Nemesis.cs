﻿using UnityEngine;
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
    private bool m_IsOnLadder = false;

    public float m_EnergySuckPerSecond = 80;
    public float m_JumpImpulse = 5;
    public float m_MoveSpeed = 1;
    public float m_StunTime = 1.0f;
    public float m_StepRate = 1.0f;
    public float m_GruntRate = 1.0f;
    public float m_MaxEnergy = 83f;
    public float m_EnergyLossPerSecond = 0.1f;
    public float m_StartGruntingEnergy = 30;
    public float m_RotationSpeed = 400;
    public float m_LadderClimbSpeed = 1;
    public float m_MeshRotateSpeed1 = 1;
    public float m_MeshRotateSpeed2 = 1;
    public GameObject m_MeshRotate1;
    public GameObject m_MeshRotate2;
    private GameState m_GameState;

    public AudioSource m_StepSource;
    public AudioSource m_GruntSource;

	// Use this for initialization
	void Start () {
        m_RigidBody = this.rigidbody2D;
        m_Collider = GetComponentInChildren<Collider2D>();
        m_CanJump = false;
        if (m_RigidBody == null)
        {
            Debug.LogError("No rigidbody 2D attached to nemesis !");
        }
        m_StunTimer = 0;
        m_PlayStepSoundTimer = 1;
		m_LastPosition = new Vector2(transform.position.x, transform.position.y);
        m_Energy = m_MaxEnergy;
        m_GameState = FindObjectOfType<GameState>();

    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        m_Energy -= m_EnergyLossPerSecond * Time.deltaTime;

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

		if (m_Energy > 0)
        {
            if (m_StunTimer > 0)
                m_StunTimer -= Time.deltaTime;
            else
            {
                // update controls
                float axisValueX = Input.GetAxis("HorizontalP2Joy");
                float axisValueY = Input.GetAxis("VerticalP2Joy");

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
                    if (Input.GetKeyDown(KeyCode.Joystick2Button0))
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


                    if (Input.GetKeyDown(KeyCode.Joystick2Button0) && m_CanJump)
                    {
                        m_CanJump = false;
                        m_RigidBody.AddForce(new Vector2(0, m_JumpImpulse), ForceMode2D.Impulse);
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
        else
        {
            m_GameState.SetGameState(GameState.E_GameState.GM_LIGHT_WIN);
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

    public Collider2D GetCollider()
    {
        return m_Collider;
    }
}
