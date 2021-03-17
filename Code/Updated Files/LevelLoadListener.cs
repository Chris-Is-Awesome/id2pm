using System;
using UnityEngine;
using ModStuff;

public class LevelLoadListener : MonoBehaviour
{
	LevelLoadListener.OnDoneFunc onDone;

	void OnDestroy()
	{
		this.onDone = null;
	}

	void OnLevelWasLoaded()
	{
		LevelLoadListener.OnDoneFunc onDoneFunc = this.onDone;
		this.onDone = null;
		Destroy(base.gameObject);
		if (onDoneFunc != null)
		{
			onDoneFunc();
		}
		EventListener.SceneLoad(); // Invoke custom event
	}

	public static GameObject RegisterListener(LevelLoadListener.OnDoneFunc onDone)
	{
		GameObject gameObject = new GameObject("LevelLoadListener");
		LevelLoadListener levelLoadListener = gameObject.AddComponent<LevelLoadListener>();
		DontDestroyOnLoad(levelLoadListener);
		levelLoadListener.onDone = onDone;
		return gameObject;
	}

	public delegate void OnDoneFunc();
}
