using UnityEngine;

namespace ModStuff.Utility
{
	public static class Core
	{
		public static Transform FindNestedChild(string indirectParentName, string childName, string directParentName = "")
		{
			Transform indirectParentObj = GameObject.Find(indirectParentName).transform;
			string output = string.Empty;

			// If indirect parent found
			if (indirectParentObj != null)
			{
				// If indirect parent has children
				if (indirectParentObj.childCount > 0)
				{
					Transform[] children = indirectParentObj.GetComponentsInChildren<Transform>();

					// Loop through each child
					foreach (Transform child in children)
					{
						// If child name matches what is wanted
						if (child.name == childName || child.name.ToLower() == childName.ToLower())
						{
							// If searching for direct parent
							if (!string.IsNullOrEmpty(directParentName))
							{
								// If direct parent of child matches what is wanted
								if ((child.parent.name == directParentName || child.parent.name.ToLower() == directParentName.ToLower())) { return child; }
								// If direct parent of child does not match what is wanted
								else { output = "No direct parent with name '" + directParentName + "' was found for child '" + childName + "' with indirect parent of '" + indirectParentName + "'. Returning null."; }
							}
							// If not searching for direct parent
							else { return child; }
						}
						// If no child matching the wanted name is found
						else { output = "No child with name '" + childName + "' was found under indirect parent '" + indirectParentName + "'. Returning null."; }
					}
				}
				// If indirect parent has no children
				else { output = "'" + indirectParentName + "' has no children. Returning null."; }
			}
			// If indirect parent not found
			else { output = "'" + indirectParentName + "' was not found. Returning null."; }

			DebugManager.LogDebugMessage(output, LogType.Warning);
			return null;
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