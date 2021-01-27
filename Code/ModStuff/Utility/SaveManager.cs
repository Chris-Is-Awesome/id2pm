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
	}
}