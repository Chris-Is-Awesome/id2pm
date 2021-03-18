using System.Collections.Generic;
using UnityEngine;

namespace ModStuff.Commands
{
	public class FindCommand : DebugCommand
	{
		public GameObject savedObj;

		// find [objName] -save
		// find -load
		// find [objName]
		// find [objName] -activate
		// find [objName] -destroy
		// find [objName] -pos [x] [y] [z]
		// find [objName] -rot [x] [y] [z]
		// find [objName] -scale [x] [y] [z]
		// find [objName] [index]

		public override string Activate(string[] args)
		{
			// If args given
			if (args.Length > 0)
			{
				string arg0 = args[0];
				int allObjs = 0;
				List<GameObject> foundObjs = new List<GameObject>();
				GameObject foundObj = null;

				// If loading saved object
				if (IsValidArg(arg0, "-load"))
				{
					foundObj = LoadSavedObject();
					if (foundObj == null) return DebugManager.LogToConsole("No object has been saved! Save an object first. Use <out>help find</out> for more info.", DebugManager.MessageType.Error);
				}
				// If searching for object
				else
				{
					// Search through all objects for objects with matching name or name that contains the search name
					foreach (GameObject obj in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
					{
						allObjs++;
						if (StringHelper.DoStringsMatch(obj.name, arg0) || StringHelper.DoesStringContain(obj.name, arg0))
						{
							foundObjs.Add(obj);
							if (StringHelper.DoStringsMatch(arg0, "PlayerEnt")) break;
						}
					}
				}

				// If only one matching object found, we found the wanted one
				if (foundObjs.Count == 1) foundObj = foundObjs[0];

				// If 2+ args given
				if (args.Length > 1)
				{
					string arg1 = args[1];

					// If index for multiple found objects given
					if (foundObj == null && TryParseInt(arg1, out int index))
					{
						// If index is valid
						if (index > -1 && index < foundObjs.Count)
						{
							// Set found obj
							foundObj = foundObjs[index];
						}
						// If invalid index
						else return DebugManager.LogToConsole(index + " is not a valid index for the " + foundObjs.Count + " objects found. Must specify a number between 0 and " + foundObjs.Count + ". Use <out>help find</out> for more info.", DebugManager.MessageType.Error);
					}

					// If found obj exists
					if (foundObj != null)
					{
						// If saving obj
						if (IsValidArg(arg1, "-save"))
						{
							SaveObject(foundObj);
							return "Saved object " + foundObj.name + " to memory. It can be referenced at any time.";
						}

						// If changing active state of obj
						if (IsValidArg(arg1, "-activate"))
						{
							foundObj.SetActive(!foundObj.activeSelf);
							string state = foundObj.activeSelf ? "active" : "inactive";
							string state2 = foundObj.activeInHierarchy ? string.Empty : " (inactive in hierarchy because a parent is inactive)";
							return "Set " + foundObj.name + " to " + state + state2;
						}

						// If destroying obj
						if (IsValidArg(arg1, "-destroy"))
						{
							if (foundObj == savedObj) savedObj = null;
							Object.Destroy(foundObj);
							return "Destroyed " + foundObj.name;
						}

						// If 5+ args given
						if (args.Length > 4)
						{
							// If changing position of obj
							if (IsValidArg(arg1, "-pos") || IsValidArg(arg1, "-position"))
							{
								if (TryParseToVector3(args[2], args[3], args[4], out Vector3 pos))
								{
									foundObj.transform.position = pos;
									return "Set position for " + foundObj.name + " to " + pos.ToString();
								}

								return DebugManager.LogToConsole("Must specify a valid Vector3 (<out>x [float] y [float] z [float]</out> position. Use <out>help find</out> for more info.", DebugManager.MessageType.Error);
							}

							// If changing rotation of obj
							if (IsValidArg(arg1, "-rot") || IsValidArg(arg1, "-rotation"))
							{
								if (TryParseToVector3(args[2], args[3], args[4], out Vector3 rot))
								{
									foundObj.transform.localEulerAngles = rot;
									return "Set rotation for " + foundObj.name + " to " + rot.ToString();
								}

								return DebugManager.LogToConsole("Must specify a valid Vector3 (<out>x [float] y [float] z [float]</out> rotation. Use <out>help find</out> for more info.", DebugManager.MessageType.Error);
							}

							// If changing scale of obj
							if (IsValidArg(arg1, "-scale") || IsValidArg(arg1, "-size"))
							{
								if (TryParseToVector3(args[2], args[3], args[4], out Vector3 scale))
								{
									foundObj.transform.localScale = scale;
									return "Set scale for " + foundObj.name + " to " + scale.ToString();
								}

								return DebugManager.LogToConsole("Must specify a valid Vector3 (<out>x [float] y [float] z [float]</out> scale. Use <out>help find</out> for more info.", DebugManager.MessageType.Error);
							}
						}

						// Output object info
						return GetObjectInfo(foundObj);
					}

					// If found obj not found
					else if (foundObjs.Count > 1) DebugManager.LogToConsole(foundObjs.Count + " objects with name equalling or containing " + arg0 + " were found. Narrow the search down by providing a more specific name or specify which one of these objects to use. Use <out>help find</out> for more info.", DebugManager.MessageType.Warn);
				}

				// If no more args given and found object
				else if (foundObj != null) return GetObjectInfo(foundObj);
				// If no more args given and multiple objects were found
				else if (foundObjs.Count > 1) return DebugManager.LogToConsole(foundObjs.Count + " objects with name equalling or containing " + arg0 + " were found. Narrow the search down by providing a more specific name or specify which one of these objects to use. Use <out>help find</out> for more info.", DebugManager.MessageType.Warn);
				
				// If no more args given and no found object
				return DebugManager.LogToConsole("No object with name " + arg0 + " was found out of " + allObjs + " objects.", DebugManager.MessageType.Error);
			}

			// If no args given
			return GetHelp();
		}

		private string GetObjectInfo(GameObject obj)
		{
			Transform objParent = obj.transform.parent;
			List<string> parents = new List<string>();
			List<string> children = new List<string>();
			string familyTree = string.Empty;
			string isActiveText = obj.transform.parent != null ? "Is active (excl. parent): " + obj.activeSelf.ToString() + " | Is active (incl. parent): " + obj.activeInHierarchy.ToString() : "Is active: " + obj.activeSelf;

			// Get parents
			while (objParent != null)
			{
				parents.Add(objParent.name);
				objParent = objParent.parent;
			}

			// Get children
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				children.Add(obj.transform.GetChild(i).name);
			}

			// Make family tree
			for (int i = parents.Count - 1; i >= 0; i--)
			{
				familyTree += parents[i] + " -> ";
			}

			familyTree += "<color=blue>" + obj.name + "</color>";

			for (int i =  children.Count - 1; i >= 0; i--)
			{
				if (i == children.Count - 1)
				{
					familyTree += " -> " + children[i];
					continue;
				}

				familyTree += ", " + children[i];
			}

			// Get info
			string info = string.Concat
			(
				"<color=green>" + obj.name + "</color>\n\n",
				"----- TRANFORM -----\n",
				"Position: " + obj.transform.position.ToString() + "\n",
				"Rotation: " + obj.transform.localEulerAngles.ToString() + "\n",
				"Scale: " + obj.transform.localScale.ToString() + "\n\n",
				"----- HIERARCHY -----\n",
				isActiveText + "\n",
				"Family tree: " + familyTree
			);

			return info;
		}

		private void SaveObject(GameObject obj)
		{
			savedObj = obj;
			savedObj.SetActive(false);
			Object.DontDestroyOnLoad(savedObj);
		}

		private GameObject LoadSavedObject()
		{
			if (savedObj != null) return savedObj;

			DebugManager.LogToFile("Attempted to load null saved object", LogType.Error, true);
			return null;
		}

		public static string GetHelp()
		{
			return "";
		}
	}
}