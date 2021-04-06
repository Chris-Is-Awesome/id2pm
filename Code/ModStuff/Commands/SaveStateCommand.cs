using System;

namespace ModStuff.Commands
{
	public class SaveStateCommand : DebugCommand
	{
		public override string Activate(string[] args)
		{
			// If index given
			if (TryParseInt(args[0], out int saveSlot))
			{
				// If positive number
				if (saveSlot >= 0)
				{
					// Create filename for save state file
					DateTime timestamp = DateTime.Now;
					string date = timestamp.ToString("MM-dd-yy");
					string time = timestamp.ToString("HH-mm");
					string fileName = "state-" + saveSlot + "_" + date + "_" + time + ".state";

					// Delete any existing save states for this slot
					string filePath = FileManager.GetFileNameFromText(FileManager.GetModDirectoryPath() + "/savestates/", "state-" + saveSlot);
					if (!string.IsNullOrEmpty(filePath)) FileManager.DeleteFile(filePath);

					// Save state
					SaveManager.GetSaverOwner().SaveAll();
					FileManager.CopyFile(VarHelper.CurrentSaveFilePath, FileManager.GetModDirectoryPath() + "/savestates/" + fileName, true);

					return "<color=green>Saved state to slot " + saveSlot + "</color>";
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