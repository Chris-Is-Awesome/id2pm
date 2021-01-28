using System;
using UnityEngine;

namespace ModStuff.Utility
{
	public static class SaveManager
	{
		#region PlayerPrefs

		public static void SaveToPrefs(string prefName, object value)
		{
			switch (value)
			{
				case int _value:
					PlayerPrefs.SetInt(prefName, _value);
					break;
				case float _value:
					PlayerPrefs.SetFloat(prefName, _value);
					break;
				case string _value:
					PlayerPrefs.SetString(prefName, _value);
					break;
				default:
					DebugManager.LogDebugMessage(value.GetType().ToString() + " is not a valid type for PlayerPrefs. Value must be of type `int`, `float`, or `string`.", LogType.Error);
					break;
			}
		}

		public static object LoadFromPrefs<T>(string prefName)
		{
			TypeCode type = Type.GetTypeCode(typeof(T));

			switch (type)
			{
				case TypeCode.Int32:
					return PlayerPrefs.GetInt(prefName);
				case TypeCode.Decimal:
					return PlayerPrefs.GetFloat(prefName);
				case TypeCode.String:
					return PlayerPrefs.GetString(prefName);
				default:
					DebugManager.LogDebugMessage(type.ToString() + " is not a valid t ype for PlayerPrefs. Type must be of type `int`, `float`, or `string`. Returning null.", LogType.Error);
					return null;
			}
		}

		public static bool HasPref(string prefName)
		{
			return PlayerPrefs.HasKey(prefName);
		}

		public static void DeletePref(string prefName)
		{
			if (HasPref(prefName))
			{
				PlayerPrefs.DeleteKey(prefName);
			}
		}

		#endregion

		#region Entity

		public static void SaveToEnt(string path, int value, bool doSave = true)
		{
			Entity toEnt = Core.GetObjComp<Entity>("PlayerEnt");
			if (toEnt != null) { toEnt.SetStateVariable(path, value); }
			if (doSave) { GetSaverOwner().SaveAll(); }
		}

		public static int LoadFromEnt(string path)
		{
			Entity fromEnt = Core.GetObjComp<Entity>("PlayerEnt");
			if (fromEnt != null) { return fromEnt.GetStateVariable(path); }

			DebugManager.LogDebugMessage("'PlayerEnt' was not found. Returning 0.", LogType.Warning);
			return 0;
		}

		#endregion

		#region NewGame

		public static void SaveNewGameData(string path, string value, IDataSaver saver)
		{
			DataSaverData.DebugAddData[] newValue = new DataSaverData.DebugAddData[]
			{
				new DataSaverData.DebugAddData()
			};

			newValue[0].path = path;
			newValue[0].value = value;
			DataSaverData.AddDebugData(saver, newValue);
		}

		#endregion

		#region Core

		// Returns the primary SaverOwner
		public static SaverOwner GetSaverOwner()
		{
			foreach (SaverOwner owner in Resources.FindObjectsOfTypeAll<SaverOwner>())
			{
				if (owner.name == "MainSaver") { return owner; }
			}

			DebugManager.LogDebugMessage("SaverOwner named 'MainSaver' was not found. Returning null.", LogType.Warning);
			return null;
		}

		#endregion
	}
}