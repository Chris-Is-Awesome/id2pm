using System;
using System.Collections.Generic;
using UnityEngine;
using ModStuff.Data;

namespace ModStuff
{
	public class HotkeyHelper : Singleton<HotkeyHelper>
	{
		HotkeyData hotkeyHolder = new HotkeyData();
		List<HotkeyData.HotkeyJson.Data> hotkeyData;
		DebugCommandHandler commandHandler = DebugCommandHandler.Instance;

		private void Awake()
		{
			// Load all hotkey data
			hotkeyData = FileManager.GetDataFromJson<HotkeyData.HotkeyJson>(FileManager.GetModDirectoryPath() + "/Hotkeys.json").hotkeys;

			// Convert json key value from string to KeyCode
			for (int i = 0; i < hotkeyData.Count; i++)
			{
				HotkeyData.HotkeyJson.Data hotkey = hotkeyData[i];

				// If keycode name is valid
				try
				{
					KeyCode key = (KeyCode)Enum.Parse(typeof(KeyCode), hotkey.key);
					DebugCommandHandler.CommandInfo command = GetCommandMethod(hotkey.name);
					string[] args = GetCommandArgs(hotkey.name);
					hotkeyHolder.hotkeys.Add(new HotkeyData.Hotkey(hotkey.name, key, command, args));
				}
				// If keycode name is invalid
				catch
				{
					DebugManager.LogToFile("Hotkey '" + hotkey.name + "' does not have a key assigned and will never run.", LogType.Warning, false, false);
				}
			}

			DebugManager.LogToFile("Hotkeys initialized!");
		}

		private DebugCommandHandler.CommandInfo GetCommandMethod(string commandInput)
		{
			string commandName = commandInput.Split(' ')[0];
			return commandHandler.GetCommand(commandName);
		}

		private string[] GetCommandArgs(string commandInput)
		{
			// If input contains args
			if (commandInput.Contains(" "))
			{
				return commandInput.Substring(commandInput.IndexOf(' ') + 1).Split(' ');
			}

			return null;
		}

		void Update()
		{
			if (Input.anyKeyDown)
			{
				if (VarHelper.PlayerObj != null)
				{
					for (int i = 0;  i < hotkeyHolder.hotkeys.Count; i++)
					{
						HotkeyData.Hotkey hotkey = hotkeyHolder.hotkeys[i];

						if (Input.GetKeyDown(hotkey.key))
						{
							commandHandler.ActivateCommand(hotkey.commandToRun, hotkey.commandArgs);
							Debug.Log("Hotkey used for '" + hotkey.name + "'!");
						}
					}
				}
			}
		}
	}
}