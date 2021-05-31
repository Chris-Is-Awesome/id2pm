using System;
using UnityEngine;

namespace ModStuff.Commands
{
	public class SaveStateCommand : DebugCommand
	{
		public override string Activate(string[] args)
		{
			// If args given
			if (args.Length > 0)
			{
				// If index given
				if (TryParseInt(args[0], out int saveSlot))
				{
					// If positive number
					if (saveSlot >= 0)
					{
						CreateStateFile(saveSlot.ToString());
						return DebugManager.LogToConsole("Saved state <in>" + saveSlot + "</in>!", DebugManager.MessageType.Success);
					}
				}
				// If saving with name
				else
				{
					string stateName = CombineArgsToString(args, true);

					// Keep file name within a reasonable length (avoids issue with filename being too long, being unable to create file)
					if (stateName.Length < 200)
					{
						// Cycle through each character in savestateName to ensure each one is letter or digit (avoids issue with invalid chars in file name)
						foreach (char c in stateName)
						{
							if (!char.IsLetterOrDigit(c) && c != '_')
							{
								return DebugManager.LogToConsole("While naming a savestate, you must use only letters or numbers", DebugManager.MessageType.Error);
							}
						}

						CreateStateFile(stateName);
						return DebugManager.LogToConsole("Saved state <in>" + stateName + " </in>!", DebugManager.MessageType.Success);
					}

					// If filename is too long
					return DebugManager.LogToConsole("State name is too long. Keep it under 200 characters!", DebugManager.MessageType.Error);
				}
			}

			// If no index given
			return DebugManager.LogToConsole("No savestate slot or name was given. Use <out>help savestate/loadstate</out> for more info.", DebugManager.MessageType.Error);
		}

		private void CreateStateFile(string slotName)
		{
			string fileName = "state-" + slotName + ".state";
			string filePath = FileManager.GetFileNameFromText(FileManager.GetModDirectoryPath() + "/savestates/", fileName);

			// Delete any existing savestates with this name/slot
			if (!string.IsNullOrEmpty(filePath)) FileManager.DeleteFile(filePath);

			// Save data
			SaveTempData();

			// Copy save file to mod directory/savestates directory
			FileManager.CopyFile(VarHelper.CurrentSaveFilePath, FileManager.GetModDirectoryPath() + "/savestates/" + fileName, true);
		}

		private void SaveTempData()
		{
			/*
			// Store Transforms of all Entities in the current room
			List<Entity> ents = UnityEngine.Object.FindObjectsOfType<Entity>().ToList();

			for (int i = 0; i < ents.Count; i++)
			{
				Entity ent = ents[i];

				if (ent.gameObject.activeInHierarchy && ent.GetComponent<Moveable>() != null)
				{
					EntityInfo entInfo = ent.GetComponent<EntityInfo>();
					string entName = entInfo != null ? entInfo.uniqueName : ent.name;

					SaveManager.SaveToSaveFile("mod/savestate/objects/" + entName + "/position/x", ent.transform.position.x.ToString(), false);
					SaveManager.SaveToSaveFile("mod/savestate/objects/" + entName + "/position/y", ent.transform.position.y.ToString(), false);
					SaveManager.SaveToSaveFile("mod/savestate/objects/" + entName + "/position/z", ent.transform.position.z.ToString(), false);
				}
			*/

			// Store scene & room
			SaveManager.SaveToSaveFile("mod/savestate/scene", SceneAndRoomHelper.GetLoadedScene().name, false);
			SaveManager.SaveToSaveFile("mod/savestate/room", SceneAndRoomHelper.GetLoadedRoom().RoomName, false);

			// Save player position
			Transform playerTrans = VarHelper.PlayerObj.transform;
			SaveManager.SaveToSaveFile("mod/savestate/objects/PlayerEnt/position/x", playerTrans.position.x.ToString(), false);
			SaveManager.SaveToSaveFile("mod/savestate/objects/PlayerEnt/position/y", playerTrans.position.y.ToString(), false);
			SaveManager.SaveToSaveFile("mod/savestate/objects/PlayerEnt/position/z", playerTrans.position.z.ToString(), false);
			SaveManager.SaveToSaveFile("mod/savestate/objects/PlayerEnt/rotation/x", playerTrans.localEulerAngles.x.ToString(), false);
			SaveManager.SaveToSaveFile("mod/savestate/objects/PlayerEnt/rotation/y", playerTrans.localEulerAngles.y.ToString(), false);
			SaveManager.SaveToSaveFile("mod/savestate/objects/PlayerEnt/rotation/z", playerTrans.localEulerAngles.z.ToString(), false);

			// Save state
			SaveManager.GetSaverOwner().SaveAll(true, delegate (bool success, string error)
			{
				DeleteTempData();
			});
			VarHelper.PlayerObj.GetComponent<Entity>().SaveState();
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

		public static string GetHelp()
		{
			string description = "Makes a save state. Use <out>loadstate</out> to load this state. Currently only supports save data & player data, no enemy data is saved. If you are naming the savestate, you cannot use any special characters.\n\n";
			string aliases = "Aliases: save, ss\n";
			string usage = "Usage: <out>savestate slot {int}</out> OR <out>savestate name {string}</out>\n";
			string examples = "Examples: <out>savestate 1</out>, <out>savestate Hundo Pillow Fort</out>";

			return description + aliases + usage + examples;
		}
	}
}