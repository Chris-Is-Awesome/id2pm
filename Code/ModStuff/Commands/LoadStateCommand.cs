using UnityEngine;

namespace ModStuff.Commands
{
	public class LoadStateCommand : DebugCommand
	{
		public override string Activate(string[] args)
		{
			// If index given
			if (TryParseInt(args[0], out int saveSlot))
			{
				// If positive number
				if (saveSlot >= 0)
				{
					// Get save state file path
					string filePath = FileManager.GetFileNameFromText(FileManager.GetModDirectoryPath() + "/savestates/", "state-" + saveSlot);

					// If no save state found for save slot
					if (string.IsNullOrEmpty(filePath))
					{
						return DebugManager.LogToConsole("No save state found for slot " + saveSlot, DebugManager.MessageType.Error);
					}

					// Load state
					FileManager.CopyFile(filePath, VarHelper.CurrentSaveFilePath, true);

					// Load data & update player transform
					SaveManager.GetSaverOwner().LoadAll(false, delegate (bool success, string error) { LoadTempData(); });

					// EventListener.OnEntitySpawn += OnEntitySpawn;

					return "<color=green>Loaded state from slot " + saveSlot + "</color>";
				}
			}

			// If no index given
			return DebugManager.LogToConsole("No <out>(int)</out> index was given. Use <out>help savestate/loadstate</out> for more info.", DebugManager.MessageType.Error);
		}

		private void LoadTempData()
		{
			string scene = SaveManager.LoadFromSaveFile("mod/savestate/scene");
			string room = SaveManager.LoadFromSaveFile("mod/savestate/room");
			float posX = float.Parse(SaveManager.LoadFromSaveFile("mod/savestate/objects/PlayerEnt/position/x"));
			float posY = float.Parse(SaveManager.LoadFromSaveFile("mod/savestate/objects/PlayerEnt/position/y"));
			float posZ = float.Parse(SaveManager.LoadFromSaveFile("mod/savestate/objects/PlayerEnt/position/z"));
			float rotX = float.Parse(SaveManager.LoadFromSaveFile("mod/savestate/objects/PlayerEnt/rotation/x"));
			float rotY = float.Parse(SaveManager.LoadFromSaveFile("mod/savestate/objects/PlayerEnt/rotation/y"));
			float rotZ = float.Parse(SaveManager.LoadFromSaveFile("mod/savestate/objects/PlayerEnt/rotation/z"));

			// Update player transform
			Vector3 playerPos = new Vector3(posX, posY, posZ); // Teleport player
			Vector3 playerRot = new Vector3(rotX, rotY, rotZ); // Rotate player
			VarHelper.PlayerObj.GetComponent<Entity>().LoadState();
			SceneAndRoomHelper.LoadRoom(scene, room, false, false, playerPos, playerRot, true); // Load scene/room

			DeleteTempData();
		}

		private void DeleteTempData()
		{
			// Delete temp saved data
			SaveManager.DeleteSaveData("mod/savestate/scene", false);
			SaveManager.DeleteSaveData("mod/savestate/room", false);
			SaveManager.DeleteSaveData("mod/savestate/objects/PlayerEnt/position/x", false);
			SaveManager.DeleteSaveData("mod/savestate/objects/PlayerEnt/position/y", false);
			SaveManager.DeleteSaveData("mod/savestate/objects/PlayerEnt/position/z", false);
			SaveManager.DeleteSaveData("mod/savestate/objects/PlayerEnt/rotation/x", false);
			SaveManager.DeleteSaveData("mod/savestate/objects/PlayerEnt/rotation/y", false);
			SaveManager.DeleteSaveData("mod/savestate/objects/PlayerEnt/rotation/z", false);
			SaveManager.GetSaverOwner().SaveAll();
		}


		/*
		private void OnEntitySpawn(Entity ent, bool isActive)
		{
			// Load Transforms of Entities
			EntityInfo entInfo = ent.GetComponent<EntityInfo>();
			string entName = entInfo != null ? entInfo.uniqueName : ent.name;
			float x = float.Parse(SaveManager.LoadFromSaveFile("mod/savestate/objects/" + entName + "/position/x"));
			float y = float.Parse(SaveManager.LoadFromSaveFile("mod/savestate/objects/" + entName + "/position/y"));
			float z = float.Parse(SaveManager.LoadFromSaveFile("mod/savestate/objects/" + entName + "/position/z"));
			ent.transform.position = new Vector3(x, y, z);
		}
		*/

		public static string GetHelp()
		{
			return "Coming soon...";
		}
	}
}