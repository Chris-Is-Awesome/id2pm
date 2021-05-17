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

			EventListener.OnPlayerUpdate += PlayerUpdate;
			DebugManager.LogToFile("Hotkeys initialized!");
		}

		public HotkeyData.Hotkey GetHotkey(string name)
		{
			// Iterate through all hotkeys
			for (int i = 0; i < hotkeyHolder.hotkeys.Count; i++)
			{
				HotkeyData.Hotkey hotkey = hotkeyHolder.hotkeys[i];

				// If found wanted hotkey, return it
				if (StringHelper.DoStringsMatch(name, hotkey.name))
				{
					return hotkey;
				}
			}

			return null;
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

		private HotkeyData.Hotkey IsUsingHotkey()
		{
			// If any key is pressed
			if (Input.anyKeyDown)
			{
				// Iterate through all hotkeys
				for (int i = 0; i < hotkeyHolder.hotkeys.Count; i++)
				{
					HotkeyData.Hotkey hotkey = hotkeyHolder.hotkeys[i];

					// If hotkey is pressed
					if (Input.GetKeyDown(hotkey.key))
					{
						return hotkey;
					}
				}
			}

			return null;
		}

		private void PlayerUpdate()
		{
			HotkeyData.Hotkey hotkey = IsUsingHotkey();

			if (hotkey != null)
			{
				// If hotkey is for OpenDebugMenu
				if (hotkey.name == "OpenDebugMenu") return;

				DebugManager.LogToFile("Hotkey '" + hotkey.name + "' triggered from pressing '" + hotkey.key.ToString() + "'!", LogType.Log, false, false);

				// If command is active
				if (commandHandler.IsCommandActive(hotkey.commandToRun))
				{
					// Deactivate command
					commandHandler.DeactivateCommand(hotkey.commandToRun);
				}
				// If command inactive
				else
				{
					// Activate command
					commandHandler.ActivateCommand(hotkey.commandToRun, hotkey.commandArgs);
				}
			}
		}
	}
}