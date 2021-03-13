namespace ModStuff
{
	public static class EventListener
	{
		// Delegates
		public delegate void OnEntityFunc(Entity ent);
		public delegate void OnItemFunc(Entity ent, Item item);
		public delegate void OnDamageFunc(Entity ent, HitData hitData);
		public delegate void OnVariableFunc(Entity ent, string var, int value);
		public delegate void OnRoomChangeFunc(Entity ent, LevelRoom toRoom, LevelRoom fromRoom, EntityEventsOwner.RoomEventData data);

		// Events
		public static event OnEntityFunc EntityDeathListener;
		public static event OnEntityFunc EntityDeactivateListener;
		public static event OnItemFunc ItemGetListener;
		public static event OnDamageFunc EntityDamageListener;
		public static event OnVariableFunc EntityVarListener;
		public static event OnRoomChangeFunc RoomChangeListener;

		public static void OnEntityDeath(Entity ent)
		{
			// Don't run for player "dying" by voiding out
			if (ent.GetComponentInChildren<Killable>().CurrentHp <= 0)
			{
				DebugManager.LogToFile("[OnEntityDeath] " + ent.name + " has died");
				EntityDeathListener?.Invoke(ent);
			}
		}

		public static void OnEntityDeactivate(Entity ent)
		{
			DebugManager.LogToFile("[OnEntityDeactivate] " + ent.name + " has deactivated");
			EntityDeactivateListener?.Invoke(ent);
		}

		public static void OnItemGet(Entity ent, Item item)
		{
			string itemName = item.ItemId != null ? item.ItemId.name : item.name;
			DebugManager.LogToFile("[OnItemGet] " + ent.name + " has picked up " + itemName);
			ItemGetListener?.Invoke(ent, item);
		}

		public static void OnEntityDamage(Entity ent, HitData hitData)
		{
			DebugManager.LogToFile("[OnEntityDamage] " + ent.name + " has taken " + hitData.GetDamageData().Length + " damage from " + hitData.Attacker.name);
			EntityDamageListener?.Invoke(ent, hitData);
		}

		public static void OnEntityVarChange(Entity ent, string var, int value)
		{
			DebugManager.LogToFile("[OnEntityVarChange] Save flag changed for " + ent.name + ". " + var + " set to " + value);
			EntityVarListener?.Invoke(ent, var, value);
		}

		public static void OnRoomChange(Entity ent, LevelRoom toRoom, LevelRoom fromRoom, EntityEventsOwner.RoomEventData data)
		{
			string roomProgression = string.Empty;
			if (fromRoom != null) roomProgression += " from room " + fromRoom.RoomName;
			if (toRoom != null) roomProgression += " to room " + toRoom.RoomName;
			DebugManager.LogToFile("[OnRoomChange] Room changed" + roomProgression + " ending at position of " + data.targetPos + ", facing " + data.targetDir);
			RoomChangeListener?.Invoke(ent, toRoom, fromRoom, data);
		}
	}
}