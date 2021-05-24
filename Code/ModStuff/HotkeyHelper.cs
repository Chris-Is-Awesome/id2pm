using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ModStuff.Data;
using ModStuff.UI;

namespace ModStuff
{
	public class HotkeyHelper : Singleton<HotkeyHelper>
	{
		HotkeyData hotkeyHolder = new HotkeyData();
		List<HotkeyData.HotkeyJson.Data> hotkeyData;
		DebugCommandHandler commandHandler = DebugCommandHandler.Instance;
		Coroutine textAnimation;
		GameObject textObj;
		float moveInterval = 1;
		Vector3 moveToPosition;

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
				if (textAnimation != null)
				{
					StopCoroutine(textAnimation);
					Destroy(textObj);
				}
				textAnimation = commandHandler.StartCoroutine(ShowText(hotkey.name, hotkey.key.ToString())); // Start animation of showing text
				EventListener.OnSceneUnload += OnSceneUnload;

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

		void OnSceneUnload(Scene scene)
		{
			commandHandler.StopCoroutine(textAnimation); // Stop animation of showing text
			EventListener.OnSceneUnload -= OnSceneUnload;
		}

		void Update()
		{
			if (textObj != null)
			{
				textObj.transform.localPosition = Vector3.MoveTowards(textObj.transform.localPosition, moveToPosition, moveInterval * Time.deltaTime);
			}
		}

		IEnumerator ShowText(string command, string key)
		{
			string message = command + " {" + key + "} fired!";
			textObj = UIText.Instance.CreateText("HotkeyNotification", message);
			textObj.transform.localPosition = new Vector3(-5f, -3, 0f);
			moveToPosition = new Vector3(-5f, -2.6375f, 0f);
			yield return new WaitForSeconds(0.5f);
			moveToPosition = new Vector3(-5f, -2.7f, 0f);

			yield return new WaitForSeconds(0.1f);
			moveToPosition = new Vector3(-5f, -2.58375f, 0f);

			yield return new WaitForSeconds(0.1f);
			moveToPosition = new Vector3(-5f, -2.66f, 0f);

			yield return new WaitForSeconds(0.1f);
			moveToPosition = new Vector3(-5f, -2.61f, 0f);

			yield return new WaitForSeconds(0.1f);
			moveToPosition = new Vector3(-5f, -2.6375f, 0f);

			yield return new WaitForSeconds(1.75f);
			moveToPosition = new Vector3(-5f, -3f, 0f);

			yield return new WaitForSeconds(0.5f);
			Destroy(textObj);
			StopCoroutine(textAnimation);
		}
	}
}