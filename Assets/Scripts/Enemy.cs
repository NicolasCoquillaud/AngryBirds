﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour,IScore {

	Rigidbody m_Rigidbody;
	Transform m_Transform;

	[Header("Destruction")]
	[SerializeField] float m_DestructionForce;

	[Header("Score")]
	[SerializeField]
	int m_Score;
	public int Score { get { return m_Score; } }

	[Header("Time Start Check Collision")]
	[SerializeField] float m_WaitDurationBeforeStartCheckCollision=1f;
	float m_TimeStartCheckCollision;

	bool m_AlreadyHit = false;

	void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Transform = GetComponent<Transform>();
	}

	protected void Start()
	{
		m_TimeStartCheckCollision = Time.time + m_WaitDurationBeforeStartCheckCollision;
	}

	void OnDestroy()
	{
		if (GameManager.Instance.IsPlaying)
		{
			EventManager.Instance.Raise(new ScoreItemEvent() { eScore = this as IScore });
			EventManager.Instance.Raise(new EnemyHasBeenDestroyedEvent() { eEnemy = this });
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(Time.time> m_TimeStartCheckCollision
			&& GameManager.Instance.IsPlaying 
			&& !m_AlreadyHit)
		{

			bool toBeDestroyed = false;

			if (collision.gameObject.CompareTag("Ball")) toBeDestroyed = true;
			else if (collision.gameObject.CompareTag("Beam"))
			{
				float deltaTime = Time.deltaTime;
				Vector3 totalForce = deltaTime == 0 ? Vector3.zero : collision.impulse / deltaTime;
				if (totalForce.magnitude > m_DestructionForce)
				{
					toBeDestroyed = true;
					Debug.Log(name + " Collision with " + collision.gameObject.name + "   force = " + totalForce);
				}
			}
			if (toBeDestroyed)
			{
				m_AlreadyHit = true;
				Destroy(gameObject);
			}
		}
	}
}
