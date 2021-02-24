using System.Collections.Generic;
using UnityEngine;

namespace ModStuff.Utility
{
	public static class Core
	{
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
				DebugManager.LogDebugMessage("GameObject with name '" + objName + "' was not found. Returning null.", LogType.Error);
				return null;
			}

			// If component not found
			DebugManager.LogDebugMessage("Component of type '" + typeof(T).ToString() + "' was not found on GameObject named '" + objName + "'. Returning null.", LogType.Warning);
			return null;
		}
	}
}