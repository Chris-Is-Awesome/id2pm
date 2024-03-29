﻿using UnityEngine.SceneManagement;

namespace ModStuff
{
	public static class EventListener
	{
		// Delegates
		public delegate void Func();
		public delegate void BoolFunc(bool value);
		public delegate void EntFunc(Entity ent);
		public delegate void EntBoolFunc(Entity ent, bool isActive);
		public delegate void SceneFunc(Scene scene);
		public delegate void RoomFunc(LevelRoom room, bool isActive);
		public delegate void DamageFunc(Entity ent, HitData data);
		public delegate void ColCollisionFunc(BC_CollisionData data);
		public delegate void ColTriggerFunc(BC_TriggerData data);
		public delegate void ItemFunc(Entity ent, Item item);
		public delegate void VarSaveFunc(Entity ent, string var, int value);
		public delegate void FileFunc(bool isNew, IDataSaver saver = null);

		// Events

		// Entity
		public static event BoolFunc OnPlayerSpawn;
		public static event Func OnPlayerUpdate;
		public static event EntBoolFunc OnEntitySpawn;
		public static event DamageFunc OnDamageDone;
		public static event EntFunc OnEntityDeath;

		// Scene/room loading
		public static event SceneFunc OnSceneLoad;
		public static event SceneFunc OnSceneUnload;
		public static event RoomFunc OnRoomLoad;

		// Collision
		public static event ColCollisionFunc OnCollisionEnter;
		public static event ColCollisionFunc OnCollisionStay;
		public static event ColCollisionFunc OnCollisionExit;
		public static event ColTriggerFunc OnTriggerEnter;
		public static event ColTriggerFunc OnTriggerStay;
		public static event ColTriggerFunc OnTriggerExit;

		// Game
		public static event Func OnGameStart;
		public static event FileFunc OnFileLoad;
		public static event BoolFunc OnGamePause;
		public static event Func OnGameQuit;

		// Other
		public static event ItemFunc OnItemGet;
		public static event VarSaveFunc OnEntVarSave;

		#region Entity

		public static void PlayerSpawn(bool isRespawn)
		{
			//string state = isRespawn ? "respawned" : "spawned";
			//DebugManager.LogToFile("[OnplayerSpawn] PlayerEnt has " + state);
			HotkeyHelper hotkeyHelper = HotkeyHelper.Instance; // Initialize hotkeys
			OnPlayerSpawn?.Invoke(isRespawn);
		}

		public static void PlayerUpdate()
		{
			//DebugManager.LogToFile("[OnPlayerUpdate] Player is updating!");
			OnPlayerUpdate?.Invoke();
		}

		public static void EntitySpawn(Entity ent, bool isActive)
		{
			//string state = isActive ? "spawned" : "despawned";
			//DebugManager.LogToFile("[OnEntitySpawn] Entity " + ent.name + " has " + state);
			VarHelper.AddEnts(ent, isActive);
			OnEntitySpawn?.Invoke(ent, isActive);
		}

		public static void DamageDone(Entity ent, HitData data)
		{
			//DebugManager.LogToFile("[OnDamageDone] " + data.Attacker.name + " did " + data.GetDamageData().Length + " damage to " + ent.name);
			OnDamageDone?.Invoke(ent, data);
		}

		public static void EntityDeath(Entity ent)
		{
			//DebugManager.LogToFile("[OnEntityDeath] " + ent.name + " has died");
			OnEntityDeath?.Invoke(ent);
		}

		#endregion

		#region Scene/room loading

		public static void SceneLoad()
		{
			Scene scene = SceneManager.GetActiveScene();
			//DebugManager.LogToFile("[OnSceneLoad] " + scene.name + " has loaded");
			OnSceneLoad?.Invoke(scene);
		}

		public static void SceneUnload()
		{
			Scene scene = SceneManager.GetActiveScene();
			//DebugManager.LogToFile("[OnSceneLoad] " + scene.name + " has unloaded");
			OnSceneUnload?.Invoke(scene);
		}

		public static void RoomLoad(LevelRoom room, bool isActive)
		{
			//string state = isActive ? "loaded" : "unloaded";
			//DebugManager.LogToFile("[OnRoomLoad] " + room.RoomName + " has " + state);
			OnRoomLoad?.Invoke(room, isActive);
		}

		#endregion

		#region Collision

		public static void CollisionEnter(BC_CollisionData data)
		{
			//DebugManager.LogToFile("[OnCollisionEnter] " + data.collider.name + " entered collision with " + data.myCollider.name + " at point " + data.point.ToString() + " with normal of " + data.normal.ToString());
			OnCollisionEnter?.Invoke(data);
		}

		public static void CollisionStay(BC_CollisionData data)
		{
			//DebugManager.LogToFile("[OnCollisionStay] " + data.collider.name + " is colliding with " + data.myCollider.name + " at point " + data.point.ToString() + " with normal of " + data.normal.ToString());
			OnCollisionStay?.Invoke(data);
		}

		public static void CollisionExit(BC_CollisionData data)
		{
			//DebugManager.LogToFile("[OnCollisionExit] " + data.collider.name + " exited collision with " + data.myCollider.name + " at point " + data.point.ToString() + " with normal of " + data.normal.ToString());
			OnCollisionExit?.Invoke(data);
		}

		public static void TriggerEnter(BC_TriggerData data)
		{
			//DebugManager.LogToFile("[OnTriggerEnter] " + data.collider.name + " entered collision with " + data.myCollider.name);
			OnTriggerEnter?.Invoke(data);
		}

		public static void TriggerStay(BC_TriggerData data)
		{
			//DebugManager.LogToFile("[OnTriggerStay] " + data.collider.name + " is  colliding with " + data.myCollider.name);
			OnTriggerStay?.Invoke(data);
		}

		public static void TriggerExit(BC_TriggerData data)
		{
			//DebugManager.LogToFile("[OnTriggerExit] " + data.collider.name + " exited collision with " + data.myCollider.name);
			OnTriggerExit?.Invoke(data);
		}

		#endregion

		#region Game

		public static void GameStart()
		{
			//DebugManager.LogToFile("[OnGameStart] The game has started");
			OnGameStart?.Invoke();
		}

		public static void FileLoad(bool isNew, IDataSaver saver)
		{
			//string state = isNew ? "new file was created" : "file was loaded";
			//DebugManager.LogToFile("[OnFileLoad] A " + state);
			OnFileLoad?.Invoke(isNew, saver);
		}

		public static void GamePause(bool isPaused)
		{
			//string state = isPaused ? "paused" : "unpaused";
			//DebugManager.LogToFile("[OnGamePause] The game has " + state);
			OnGamePause?.Invoke(isPaused);
		}

		public static void GameQuit()
		{
			//DebugManager.LogToFile("[OnGameQuit] The game has quit");
			OnGameQuit?.Invoke();
		}

		#endregion

		#region Other

		public static void ItemGet(Entity ent, Item item)
		{
			//DebugManager.LogToFile("[OnItemGet] " + ent.name + " got " + item.name);
			OnItemGet?.Invoke(ent, item);
		}

		public static void EntVarSave(Entity ent, string var, int value)
		{
			//DebugManager.LogToFile("[OnEntVarSave] " + ent.name + "/" + var + " set to " + value);
			OnEntVarSave?.Invoke(ent, var, value);
		}

		#endregion
	}
}