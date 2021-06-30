using System;
using System.Collections.Generic;
using UnityEngine;
using ModStuff;

[AddComponentMenu("Ittle 2/Level/Level event")]
public class LevelEvent : MonoBehaviour
{
	[SerializeField]
	Range _duration;

	[SerializeField]
	bool _constant;

	List<LevelEventComp> comps = new List<LevelEventComp>();

	List<LevelEventCondition> conditions = new List<LevelEventCondition>();

	float timer;

	float timerScale;

	float startTime;

	bool isStarted;

	bool IsInited { get; set; }

	public bool CanFire
	{
		get
		{
			this.Init();
			for (int i = 0; i < this.conditions.Count; i++)
			{
				if (!this.conditions[i].CanStart())
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool IsStarted
	{
		get
		{
			return this.isStarted;
		}
	}

	public float Timer
	{
		get
		{
			return this.timer;
		}
	}

	public float StartTime
	{
		get
		{
			return this.startTime;
		}
	}

	public void Init()
	{
		if (this.IsInited)
		{
			return;
		}
		this.IsInited = true;
		this.comps = new List<LevelEventComp>(base.GetComponentsInChildren<LevelEventComp>(true));
		this.conditions = LevelEventCondition.GetChildConditions(base.gameObject, true);
		for (int i = this.comps.Count - 1; i >= 0; i--)
		{
			this.comps[i].Init(this);
		}
		for (int j = this.conditions.Count - 1; j >= 0; j--)
		{
			this.conditions[j].Init(this);
		}
	}

	public void StartEvent(float atTime = 0f, float maxTime = 0f)
	{
		if (this.isStarted)
		{
			this.StopEvent();
		}
		this.Init();
		this.isStarted = true;
		float num;
		if (this._constant)
		{
			num = 0.5f;
			this.startTime = (this.timer = 1f);
			this.timerScale = 0f;
		}
		else
		{
			if (atTime <= 0f)
			{
				this.startTime = (this.timer = this._duration.randomValue);
				this.timerScale = 1f / this.timer;
			}
			else
			{
				this.timer = atTime;
				this.startTime = maxTime;
				this.timerScale = 1f / maxTime;
			}
			num = Mathf.Clamp01(1f - this.timer * this.timerScale);
		}
		for (int i = this.comps.Count - 1; i >= 0; i--)
		{
			this.comps[i].StartEvent(num);
		}
	}

	public void StopEvent()
	{
		if (!this.isStarted)
		{
			return;
		}
		this.isStarted = false;
		for (int i = this.comps.Count - 1; i >= 0; i--)
		{
			this.comps[i].StopEvent();
		}
	}

	public bool UpdateEvent(float dt)
	{
		if (!this.isStarted)
		{
			return true;
		}
		float t;
		if (this.timerScale == 0f)
		{
			t = 0.5f;
		}
		else
		{
			this.timer -= dt;
			t = Mathf.Clamp01(1f - this.timer * this.timerScale);
		}
		for (int i = this.comps.Count - 1; i >= 0; i--)
		{
			this.comps[i].UpdateEvent(t);
		}

		EventListener.LevelEventUpdate(this, timer); // Invoke event

		return this.timer <= 0f;
	}
}
