using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ModStuff.Utility
{
	public static class SaveManager
	{
		private static List<string> globalDataTypes = new List<string>()
		{
			"sound/musicVol",
			"sound/soundVol",
			"extras/soundtest",
			"extras/gallery",
			"extras/secretGallery"
		};

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

		#region SaveFile

		public static void SaveToSaveFile(string path, string value, bool doSave = true)
		{
			IDataSaver saver = GetSaver(path);

			if (saver != null)
			{
				string key = GetSaveKey(path);
				saver.SaveData(key, value);
				if (doSave) GetSaverOwner().SaveAll();
			}
			else DebugManager.LogDebugMessage("[Save Data] Path " + path + " could not be loaded. Unable to save the requested data.", LogType.Warning);
		}

		public static string LoadFromSaveFile(string path)
		{
			IDataSaver saver = GetSaver(path);

			if (saver != null)
			{
				string key = GetSaveKey(path);
				return saver.LoadData(key);
			}

			DebugManager.LogDebugMessage("[Save Data] Path " + path + " could not be loaded. Returning empty string.", LogType.Warning);
			return string.Empty;
		}

		public static bool HasSaveData(string path)
		{
			IDataSaver saver = GetSaver(path);

			if (saver != null)
			{
				string key = GetSaveKey(path);
				return saver.HasData(key);
			}

			return false;
		}

		public static List<string> GetSaveKeys(string header)
		{
			return MainMenu.GetSortedSaveKeys(GetSaverOwner(), header);
		}

		public static void DeleteSaveData(string path, bool doSave = true)
		{
			IDataSaver saver = GetSaver(path);

			if (saver != null)
			{
				saver.ClearValue(GetSaveKey(path));
				if (doSave) GetSaverOwner().SaveAll();
				return;
			}

			DebugManager.LogDebugMessage("[Save Data] Path " + path + " could not be loaded. Cannot delete save data that was not found.", LogType.Warning);
		}

		#endregion

		#region CustomFile

		public static void SaveToCustomFile(object data, string fileName, string fileDirectory = "", bool toJson = true, bool doOverwrite = false)
		{
			string dataToWrite = toJson ? JsonUtility.ToJson(data) : data.ToString();
			string fullPath = GetModFilePath(fileDirectory, fileName);
			if (doOverwrite) File.WriteAllText(fullPath, dataToWrite);
			else
			{
				// If file exists, add new line before appending
				if (File.Exists(fullPath)) File.AppendAllText(fullPath, Environment.NewLine + dataToWrite);
				else File.AppendAllText(fullPath, dataToWrite);
			}
		}

		/*
		public static T LoadFromCustomFile<T>(string fileName, string fileDirectory = "")
		{
			string fullPath = GetModFilePath(fileDirectory, fileName);
			if (File.Exists(fullPath)) return JsonUtility.FromJson<T>(File.ReadAllText(fullPath));

			DebugManager.LogDebugMessage("File at path " + fullPath + " does not exist. Returning default.", LogType.Warning);
			return default;
		}
		*/

		private static string GetModFilePath(string fileDirectory, string fileName)
		{
			string rootDirectory = Application.dataPath + "/extra2dew/";
			string fullDirectory = string.IsNullOrEmpty(fileDirectory) ? rootDirectory : rootDirectory + fileDirectory;
			if (fullDirectory[fullDirectory.Length - 1] != '/') fullDirectory += "/";
			if (!Directory.Exists(fullDirectory)) Directory.CreateDirectory(fullDirectory);
			return fullDirectory + fileName;
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

		public static IDataSaver GetSaver(string path)
		{
			string header = path.Remove(path.LastIndexOf('/'));
			if (header.StartsWith("/local/") || header.StartsWith("/global/")) return GetSaverOwner().GetSaver(header);

			if (globalDataTypes.Contains(path)) return GetSaverOwner().GlobalStorage.GetLocalSaver(header);
			return GetSaverOwner().GetSaver("/local/" + header);
		}

		private static string GetSaveKey(string path)
		{
			return path.Remove(0, path.LastIndexOf('/') + 1);
		}

		#endregion
	}
}