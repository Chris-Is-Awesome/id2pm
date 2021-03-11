using System;
using UnityEngine;

[AddComponentMenu("Ittle 2/Physics/Rigidbody controller")]
public class RigidBodyController : MonoBehaviour
{
	Rigidbody realBody;

	BC_Body betterbody;

	Vector3 accumVel;

	Vector3 previousPosition;

	float velocityMultiplier; // Added - MUST BE LEFT PRIVATE OR SCENE DATA WILL CORRUPT LIKELY DUE TO PREFAB SERIALIZAATION

	void Awake()
	{
		velocityMultiplier = 1; // Set velocity to default (for some reason setting it on initialization doesn't do this???)

		this.realBody = base.GetComponent<Rigidbody>();
		if (this.realBody != null)
		{
			this.realBody.freezeRotation = true;
			this.realBody.useGravity = false;
		}
		else
		{
			this.betterbody = base.GetComponent<BC_Body>();
			if (this.betterbody != null)
			{
				this.betterbody.UseGravity = false;
			}
		}
	}

	void OnDisable()
	{
		this.accumVel = Vector3.zero;
	}

	public void SetCustomVelocity(float multiplier)
	{
		velocityMultiplier = multiplier;
	}

	public void SetVelocity(Vector3 vel)
	{
		// Multiply velocity
		if (velocityMultiplier != 1) this.accumVel += vel * velocityMultiplier;
		else this.accumVel += vel;
	}

	public void MovePosition(Vector3 P)
	{
		if (this.realBody != null)
		{
			this.realBody.MovePosition(P);
		}
		else
		{
			this.betterbody.transform.position = P;
		}
		this.accumVel = Vector3.zero;
	}

	public void LateUpdate()
	{
		if (this.realBody != null)
		{
			this.realBody.velocity = this.accumVel;
		}
		else
		{
			this.betterbody.Velocity = this.accumVel;
		}
		this.accumVel = Vector3.zero;
	}
}
