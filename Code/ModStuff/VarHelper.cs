using System.Collections.Generic;
using UnityEngine;

namespace ModStuff
{
	public static class VarHelper
	{
		private static GameObject playerObj;
		public static GameObject PlayerObj
		{
			get
			{
				if (playerObj == null) DebugManager.LogToFile("Attempted to retrieve null PlayerEnt.", LogType.Error, true);
				return playerObj;
			}
			set
			{
				playerObj = value;
			}
		}

		private static List<Entity> activeEnts = new List<Entity>();
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