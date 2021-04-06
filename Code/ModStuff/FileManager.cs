using System.IO;
using UnityEngine;

namespace ModStuff
{
	public static class FileManager
	{
		// Copies a file from one location to another (allows renaming & overwriting destination file)
		public static void CopyFile(string sourcePath, string destinationPath, bool doOverwrite = false)
		{
			destinationPath = destinationPath.Replace('/', '\\');

			// If source file exists
			if (File.Exists(sourcePath))
			{
				// If not overwriting, check if destination file exists
				if (!doOverwrite && File.Exists(destinationPath))
				{
					// Output warning
					DebugManager.LogToFile("Attempted to copy file `" + sourcePath + "` to destination `" + destinationPath + "` which already exists. File not copied.", LogType.Warning);
				}
				// If overwriting or destination file does not exist
				else
				{
					// Remove filename from destination path
					string directoryPath = destinationPath.Remove(destinationPath.LastIndexOf('\\')) ;

					// If directory does not exist
					if (!Directory.Exists(directoryPath))
					{
						// Create directory
						Directory.CreateDirectory(directoryPath);
					}

					// Copy source file to destination
					File.Copy(sourcePath, destinationPath, doOverwrite);
				}
			}
			// If source file does not exist
			else
			{
				// Output warning
				DebugManager.LogToFile("Attempted to copy missing file `" + sourcePath + "`", LogType.Warning);
			}
		}

		// Moves a file from one location to another (allows renaming destination file)
		public static void MoveFile(string sourcePath, string destinationPath)
		{
			// If source file exists
			if (File.Exists(sourcePath))
			{
				File.Move(sourcePath, destinationPath);
			}
			// If source file does not exist
			else
			{
				// Output warning
				DebugManager.LogToFile("Attempted to move missing file `" + sourcePath + "`", LogType.Warning);
			}
		}

		// Deletes a file
		public static void DeleteFile(string path)
		{
			// If file exists
			if (File.Exists(path))
			{
				// Delete file
				File.Delete(path);
			}
			// If file does not exist
			else
			{
				// Output warning
				DebugManager.LogToFile("Attempted to delete missing file `" + path + "`", LogType.Warning);
			}
		}

		// Returns true if file exists, false if not
		public static bool DoesFileExist(string path)
		{
			return File.Exists(path);
		}

		// Returns specific filename containing specified text out of a directory, empty string if no such file exists
		public static string GetFileNameFromText(string path, string text)
		{
			string[] allFiles = Directory.GetFiles(path);
			
			for (int i = 0; i < allFiles.Length; i++)
			{
				if (StringHelper.DoesStringContain(allFiles[i], text))
				{
					return allFiles[i];
				}
			}

			return string.Empty;
		}

		// Returns path to mod directory
		public static string GetModDirectoryPath()
		{
			return Application.dataPath + "/extra2dew/";
		}
	}
}