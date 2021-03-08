using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModStuff.Utility
{
	public static class Core
	{
		public enum FadeType
		{
			Circle,
			Flash,
			Fullscreen
		}

		public static T GetObjComp<T>(string objName) where T : Component
		{
			GameObject go = GameObject.Find(objName);

			if (go != null)
			{
				// Check if component is on the GameObject
				T foundComp = go.GetComponent<T>();
				if (foundComp != null) { return foundComp; }

				// Check if component is on any of its  children
				foreach (Transform trans in go.transform)
				{
					foundComp = trans.GetComponent<T>();
					if (foundComp != null) { return foundComp; }
				}
			}
			else
			{
				// If GameObject not found
				DebugManager.LogToFile("GameObject with name '" + objName + "' was not found. Returning null.", LogType.Error);
				return null;
			}

			// If component not found
			DebugManager.LogToFile("Component of type '" + typeof(T).ToString() + "' was not found on GameObject named '" + objName + "'. Returning null.", LogType.Warning);
			return null;
		}

		public static bool DoStringsMatch(string string1, string string2, bool ignoreCase = true)
		{
			if (!ignoreCase) return string1 == string2;
			return string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);
		}

		public static void LoadScene(string scene, string spawn, bool doSave = true, bool doFade = true, FadeType fadeType = FadeType.Circle, Color? fadeColor = null, float fadeOutTime = 0.5f, float fadeInTime = 1f)
		{
			// If saving, trigger save
			if (doSave)
			{
				SaveManager.SaveToSaveFile("start/level", scene);
				SaveManager.SaveToSaveFile("start/door", spawn);
			}

			// If no fade, load scene instantly
			if (doFade)
			{
				SceneManager.LoadScene(scene);
				return;
			}

			// If fading, make fade & trigger load
			FadeEffectData fadeData = MakeFadeEffect(fadeType, fadeColor, fadeOutTime, fadeInTime);
			SceneDoor.StartLoad(scene, spawn, fadeData, SaveManager.GetSaverOwner());
		}

		static private FadeEffectData MakeFadeEffect(FadeType type, Color? color, float outTime, float inTime)
		{
			string fadeType;

			switch (type)
			{
				case FadeType.Flash:
					fadeType = "AdditiveFade";
					break;
				case FadeType.Fullscreen:
					fadeType = "ScreenFade";
					break;
				default:
					fadeType = "ScreenCircleWipe";
					break;
			}

			FadeEffectData fadeData = new FadeEffectData
			{
				_faderName = fadeType,
				_targetColor = color ?? Color.black,
				_fadeOutTime = outTime,
				_fadeInTime = inTime,
				_useScreenPos = true
			};

			return fadeData;
		}
	}
}