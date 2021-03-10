using System.Collections.Generic;
using UnityEngine;

namespace ModStuff
{
	/// <summary>
	/// Inherit from this base class to create a singleton.
	/// e.g. public class MyClassName : Singleton<MyClassName> {}
	/// </summary>
	public class SingletonForCheats<T> : MonoBehaviour where T : MonoBehaviour
	{
		// Check to see if we're about to be destroyed.
		private static bool m_ShuttingDown = false;
		private static object m_Lock = new object();
		private static T m_Instance;

		/// <summary>
		/// Access singleton instance through this propriety.
		/// </summary>
		public static T Instance
		{
			get
			{
				if (m_ShuttingDown)
				{
					Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
						"' already destroyed. Returning null.");
					return null;
				}

				lock (m_Lock)
				{
					if (m_Instance == null)
					{
						// Search for existing instance.
						m_Instance = (T)FindObjectOfType(typeof(T));

						// Create new instance if one doesn't already exist.
						if (m_Instance == null)
						{
							// Need to create a new GameObject to attach the singleton to.
							var singletonObject = new GameObject();
							m_Instance = singletonObject.AddComponent<T>();
							//singletonObject.name = typeof(T).ToString() + " (Singleton)";

							// Make instance persistent.
							DontDestroyOnLoad(singletonObject);
						}
					}

					return m_Instance;
				}
			}
		}

		private void OnApplicationQuit()
		{
			m_ShuttingDown = true;
		}


		private void OnDestroy()
		{
			m_ShuttingDown = true;
		}

		public bool IsValidArg(string arg, string validArg)
		{
			return StringHelper.DoStringsMatch(arg, validArg);
		}

		public bool IsValidArgOfMany(string arg, List<string> validArgs)
		{
			for (int i = 0; i < validArgs.Count; i++)
			{
				if (StringHelper.DoStringsMatch(arg, validArgs[i])) return true;
			}

			return false;
		}

		public bool TryParseToFloat(string arg, out float num)
		{
			bool isFloat = float.TryParse(arg, out num);
			return isFloat;
		}

		public bool TryParseInt(string arg, int num)
		{
			bool  isInt = int.TryParse(arg, out num);
			return isInt;
		}
	}
}