using System.Collections.Generic;
using UnityEngine;

namespace ModStuff
{
	public static class VarHelper
	{
		private static string currentSaveFilePath;
		private static GameObject playerObj;
		private static List<Entity> activeEnts = new List<Entity>();

		public static string CurrentSaveFilePath
		{
			get
			{
				if (string.IsNullOrEmpty(currentSaveFilePath)) DebugManager.LogToFile("Attempted to retrieve path from null save file.", LogType.Error);
				return currentSaveFilePath;
			}
			set
			{
				currentSaveFilePath = Application.persistentDataPath + "/steam/" + value;
			}
		}

		public static bool IsAnticheatActive
		{
			get
			{
				return SaveManager.LoadFromSaveFile("mod/settings/isSpeedrun") == "1";
			}
		}

		public static GameObject PlayerObj
		{
			get
			{
				return playerObj;
			}
			set
			{
				playerObj = value;
			}
		}

		public static List<Entity> ActiveEnts
		{
			get
			{
				if (activeEnts == null) DebugManager.LogToFile("Attempted to retrieve null list of active Entities.", LogType.Error, true);
				return activeEnts;
			}
			set
			{
				activeEnts = value;
			}
		}

		public static void AddEnts(Entity ent, bool isActive)
		{
			// If not player
			if (ent.name != "PlayerEnt")
			{
				// If active, add to list
				if (isActive) ActiveEnts.Add(ent);
				// If inactive, remove from list
				else ActiveEnts.Remove(ent);
			}
		}
	}
}