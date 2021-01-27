using UnityEngine;

namespace ModStuff.Utility
{
	public static class Core
	{
		public static T GetEntComp<T>(string entName) where T : Component
		{
			GameObject go = GameObject.Find(entName);

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
				DebugManager.LogDebugMessage("Entity with name '" + entName + "' was not found. Returning null.", LogType.Error);
				return null;
			}

			// If component not found
			DebugManager.LogDebugMessage("Component of type '" + typeof(T).ToString() + "' was not found on Entity named '" + entName + "'. Returning null.", LogType.Warning);
			return null;
		}
	}
}