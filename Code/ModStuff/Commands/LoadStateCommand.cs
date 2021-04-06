using System;

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

					// Load data
					SaveManager.GetSaverOwner().LoadAll(true);
					SceneAndRoomHelper.LoadScene("PIllowFort", "PillowFortInside", false, false); // TODO: Load saved scene

					return "<color=green>Loaded state from slot " + saveSlot + "</color>";
				}
			}

			// If no index given
			return DebugManager.LogToConsole("No <out>(int)</out> index was given. Use <out>help savestate/loadstate</out> for more info.", DebugManager.MessageType.Error);
		}

		public static string GetHelp()
		{
			return "Coming soon...";
		}
	}
}