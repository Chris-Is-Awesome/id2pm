using System.Collections.Generic;
using UnityEngine;

namespace ModStuff.Utility
{
	public static class Core
	{
		private static List<Transform> children = new List<Transform>();
		public static Transform FindNestedChild(string indirectParentName, string childName, string directParentName = "")
		{
			GameObject indirectParentObj = GameObject.Find(indirectParentName);
			string output = string.Empty;

			// If indirect parent found
			if (indirectParentObj != null)
			{
				// Get all children
				FindNestedChildRecursive(indirectParentObj.transform);

				// If children were found
				if (children.Count > 0)
				{
					// Check each child
					for (int i = 0; i < children.Count; i++)
					{
						Transform child = children[i];

						// Check child name
						if (child.name == childName)
						{
							// Check for direct parent
							if (!string.IsNullOrEmpty(directParentName))
							{
								// If direct parent is what is wanted
								if ((child.parent.name == directParentName || child.parent.name.ToLower() == directParentName.ToLower())) { return child; }
								// If direct parent is NOT what is wanted
								else { output = "No direct parent with name '" + directParentName + "' was found for child '" + childName + "' with indirect parent of '" + indirectParentName + "'. Returning null."; }
							}
							else { return child; }
						}
						// If no child matching the wanted name is found
						else { output = "No child with name '" + childName + "' was found under indirect parent '" + indirectParentName + "'. Returning null."; }
					}
				}
				// If no children were found
				else { output = "'" + indirectParentName + "' has no children. Returning null."; }
			}
			// If indirect parent NOT found
			else { output = "Indirect parent '" + indirectParentName + "' was not found. Returning null."; }

			DebugManager.LogDebugMessage(output, LogType.Warning);
			return null;
		}

		private static void FindNestedChildRecursive(Transform parent)
		{
			// Add existing children to list
			foreach (Transform child in parent)
			{
				if (child == null) continue;
				children.Add(child);
				FindNestedChildRecursive(child);
			}
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
				DebugManager.LogDebugMessage("GameObject with name '" + objName + "' was not found. Returning null.", LogType.Error);
				return null;
			}

			// If component not found
			DebugManager.LogDebugMessage("Component of type '" + typeof(T).ToString() + "' was not found on GameObject named '" + objName + "'. Returning null.", LogType.Warning);
			return null;
		}
	}
}