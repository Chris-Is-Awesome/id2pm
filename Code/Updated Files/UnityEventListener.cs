using System;
using System.Diagnostics;
using UnityEngine;
using ModStuff;

public class UnityEventListener : MonoBehaviour
{
	public event UnityEventListener.OnEventFunc StartHook;
	public event UnityEventListener.OnEventFunc OnDestroyHook;
	public event UnityEventListener.OnEventFunc OnApplicationQuitHook;

	void Start()
	{
		if (this.StartHook != null)
		{
			this.StartHook();
		}
		EventListener.GameStart(); // Invoke custom event
	}

	void OnDestroy()
	{
		if (this.OnDestroyHook != null)
		{
			this.OnDestroyHook();
		}
	}

	void OnApplicationQuit()
	{
		if (this.OnApplicationQuitHook != null)
		{
			this.OnApplicationQuitHook();
		}
		EventListener.GameQuit(); // Invoke custom event
	}

	public delegate void OnEventFunc();
}
