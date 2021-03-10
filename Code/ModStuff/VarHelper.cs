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
				if (playerObj == null) DebugManager.LogToFile("Attempted to retrieve null PlayerEnt.", LogType.Error);
				return playerObj;
			}
			set
			{
				playerObj = value;
			}
		}
	}
}