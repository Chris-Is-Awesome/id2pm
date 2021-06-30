using System;
using UnityEngine;
using ModStuff;

[AddComponentMenu("Ittle 2/Room/Triggers/Timer trigger")]
public class TimerTrigger : RoomTrigger, IUpdatable, IBaseUpdateable
{
	[SerializeField]
	float _time = 1f;

	[SerializeField]
	float _unfireTime;

	[SerializeField]
	bool _loop;

	[SerializeField]
	bool _startActive = true;

	[SerializeField]
	float _maxTime;

	[SerializeField]
	bool _randomTime;

	float timer;

	bool gotSignal;

	bool countdown;

	float GetTime()
	{
		return (!this._randomTime) ? this._time : UnityEngine.Random.Range(this._time, this._maxTime);
	}

	protected override void DoActivate(LevelRoom room)
	{
		base.DoActivate(room);
		if (!this.gotSignal)
		{
			this.timer = this.GetTime();
			this.countdown = this._startActive;
		}
	}

	protected override void DoDeactivate(LevelRoom room)
	{
		this.gotSignal = false;
		base.DoDeactivate(room);
	}

	protected override void OnDisableTrigger(bool fast)
	{
		base.enabled = false;
		this.countdown = false;
	}

	public void RestartTimer()
	{
		this.gotSignal = true;
		if (base.IsFired)
		{
			base.Unfire();
		}
		this.timer = this.GetTime();
		this.countdown = true;
		base.enabled = true;
		EventListener.TimerUpdate(0); // Invoke event
	}

	public void StartTimer()
	{
		this.gotSignal = true;
		this.countdown = true;
		base.enabled = true;
	}

	public void PauseTimer()
	{
		this.gotSignal = true;
		this.countdown = false;
		base.enabled = false;
		EventListener.TimerUpdate(0); // Invoke event
	}

	public void ZeroTimer()
	{
		this.gotSignal = true;
		this.timer = 0f;
	}

	void IUpdatable.UpdateObject()
	{
		if (!this.countdown)
		{
			base.enabled = false;
			return;
		}
		this.timer -= Time.deltaTime;

		if (transform.parent != null && transform.parent.name != "Dynabomb(Clone)") EventListener.TimerUpdate(timer); // Invoke event

		if (this.timer <= 0f)
		{
			bool isFired = base.IsFired;
			if (!isFired)
			{
				base.Fire();
			}
			else
			{
				base.Unfire();
			}
			if (!this._loop)
			{
				base.enabled = false;
			}
			else if (isFired)
			{
				this.timer = this.GetTime();
			}
			else
			{
				this.timer = this._unfireTime;
			}
		}
	}
}
