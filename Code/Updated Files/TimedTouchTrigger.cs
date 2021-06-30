using System;
using System.Collections.Generic;
using UnityEngine;
using ModStuff;

[AddComponentMenu("Ittle 2/Room/Triggers/Timed touch trigger")]
public class TimedTouchTrigger : RoomTrigger, IBC_TriggerEnterListener, IBC_TriggerExitListener, IUpdatable, IBC_CollisionEventListener, IBaseUpdateable
{
	[SerializeField]
	float _time;

	[SerializeField]
	bool _switch;

	BC_Collider betterCollider;

	List<TimedTouchTrigger.CountData> colliders = new List<TimedTouchTrigger.CountData>();

	protected override void Awake()
	{
		base.Awake();
		this.betterCollider = PhysicsUtility.RegisterColliderEvents(base.gameObject, this);
	}

	protected override void OnDestroy()
	{
		PhysicsUtility.UnregisterColliderEvents(this.betterCollider, this);
		base.OnDestroy();
	}

	protected override void DoActivate(LevelRoom room)
	{
		this.colliders.Clear();
	}

	void IBC_TriggerEnterListener.OnTriggerEnter(BC_TriggerData data)
	{
		if (base.IsFired)
		{
			return;
		}
		BC_Collider collider = data.collider;
		for (int i = this.colliders.Count - 1; i >= 0; i--)
		{
			if (this.colliders[i].col == collider)
			{
				return;
			}
		}
		this.colliders.Add(new TimedTouchTrigger.CountData(collider, this._time));
	}

	void IBC_TriggerExitListener.OnTriggerExit(BC_TriggerData data)
	{
		BC_Collider collider = data.collider;
		for (int i = this.colliders.Count - 1; i >= 0; i--)
		{
			if (this.colliders[i].col == collider)
			{
				this.colliders.RemoveAt(i);
				EventListener.TimerUpdate(0); // Invoke event
			}
		}
		if (this._switch && base.IsFired)
		{
			base.Unfire();
		}
	}

	void SendFire(BC_Collider col)
	{
		base.SetTriggerer(col.GetComponent<Entity>());
		base.Fire();
	}

	void IUpdatable.UpdateObject()
	{
		if (base.IsFired)
		{
			return;
		}
		for (int i = this.colliders.Count - 1; i >= 0; i--)
		{
			TimedTouchTrigger.CountData countData = this.colliders[i];
			countData.timer -= Time.deltaTime;

			EventListener.TimerUpdate(countData.timer); // Invoke event

			if (countData.timer <= 0f)
			{
				this.SendFire(countData.col);
				return;
			}
		}
	}

	class CountData
	{
		public BC_Collider col;

		public float timer;

		public CountData(BC_Collider col, float time)
		{
			this.col = col;
			this.timer = time;
		}
	}
}
