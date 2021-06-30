using ModStuff.UI;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModStuff.Commands
{
	public class DebugOverlayCommand : DebugCommand
	{
		Vector2 oldPos;
		double PlayerSpeed
		{
			get
			{
				Vector3 playerPos = VarHelper.PlayerObj.transform.position;
				Vector2 currPos = new Vector2(playerPos.x, playerPos.z);
				float distance;

				if (oldPos != currPos)
				{
					distance = Vector2.Distance(oldPos, currPos);
					oldPos = currPos;
					return Math.Round(distance / Time.deltaTime, 2);
				}

				return 0f;
			}
		}

		Dictionary<string, string> textToOutput = new Dictionary<string, string>()
		{
			{ "player_header", "===== PLAYER =====" },
			{ "player_position", string.Empty },
			{ "player_rotation", string.Empty },
			{ "player_speed", string.Empty },
			{ "scene_header", "===== SCENE =====" },
			{ "scene", string.Empty },
			{ "room", string.Empty },
			{ "spawn", string.Empty },
			{ "timer", string.Empty },
			{ "droptable_header", "===== DROPTABLE =====" },
			{ "droptable_info", string.Empty },
			{ "cheats_header", "===== CHEATS =====" },
			{ "active_cheats", string.Empty },
		};
		TextMesh overlay;
		DebugCommandHandler commandHandler = DebugCommandHandler.Instance;
		
		public override string Activate(string[] args)
		{
			if (!isActive)
			{
				MakeActive(GetType());
				SetupOverlay();

				EventListener.OnDamageDone += OnDamageDone;
				EventListener.OnDebugCommand += OnDebugCommand;
				EventListener.OnEntDrop += OnEntDrop;
				EventListener.OnFlagSaved += OnFlagSaved;
				EventListener.OnGameQuit += OnGameQuit;
				EventListener.OnPlayerUpdate += OnPlayerUpdate;
				EventListener.OnRoomLoad += OnRoomLoad;
				EventListener.OnSceneLoad += OnSceneLoad;
				EventListener.OnTimerUpdate += OnTimerUpdate;

				return DebugManager.LogToConsole("Debug overlay <color=green>activated</color>!");
			}

			Deactivate();
			return DebugManager.LogToConsole("Debug overlay <color=red>deactivated</color>");
		}

		void Deactivate()
		{
			EventListener.OnDamageDone -= OnDamageDone;
			EventListener.OnDebugCommand -= OnDebugCommand;
			EventListener.OnEntDrop -= OnEntDrop;
			EventListener.OnFlagSaved -= OnFlagSaved;
			EventListener.OnGameQuit -= OnGameQuit;
			EventListener.OnPlayerUpdate -= OnPlayerUpdate;
			EventListener.OnRoomLoad -= OnRoomLoad;
			EventListener.OnSceneLoad -= OnSceneLoad;
			EventListener.OnTimerUpdate -= OnTimerUpdate;

			UnityEngine.Object.Destroy(overlay.gameObject);
			ModOptions.SaveOption("debugOverlay", false);

			MakeInactive(GetType());
		}

		void SetupOverlay()
		{
			SaveManager.GetSaverOwner().SaveAll(); // Trigger save

			// Create overlay
			Vector3 position = new Vector3(1.875f, 2f, 1f);
			overlay = UIText.Instance.CreateText("Debug Overlay", "Hello, world!", position).GetComponent<TextMesh>();

			// Player data
			OnPlayerUpdate();

			// Scene data
			ReplaceLine("scene", "Scene: " + SceneAndRoomHelper.GetLoadedScene().name);
			ReplaceLine("room", "Room: " + SceneAndRoomHelper.GetLoadedRoom().RoomName);
			ReplaceLine("spawn", "Spawn: " + SaveManager.LoadFromSaveFile("start/door"));

			// Droptable data
			string dtMaster = SaveManager.LoadFromSaveFile("player/dt_master");
			string dtDrops = SaveManager.LoadFromSaveFile("player/dt_drops");
			string noHits = SaveManager.LoadFromPrefs<int>("droptables/noHitCount").ToString();
			string tier1Drops = SaveManager.LoadFromSaveFile("player/droptables/DT_Tier1");
			string tier1SuperDrops = SaveManager.LoadFromSaveFile("player/droptables/DT_Tier1Super");
			string tier2Drops = SaveManager.LoadFromSaveFile("player/droptables/DT_Tier2");
			string tier2SuperDrops = SaveManager.LoadFromSaveFile("player/droptables/DT_Tier2Super");
			string tier3Drops = SaveManager.LoadFromSaveFile("player/droptables/DT_Tier3");
			string tier3SuperDrops = SaveManager.LoadFromSaveFile("player/droptables/DT_Tier3Super");
			string tier4Drops = SaveManager.LoadFromSaveFile("player/droptables/DT_Tier4");
			string tier4SuperDrops = SaveManager.LoadFromSaveFile("player/droptables/DT_Tier4Super");

			StringBuilder sb = new StringBuilder();
				sb.Append("Master: " + dtMaster + "\n");
				sb.Append("Drops: " + dtDrops + "\n");
				sb.Append("No hits: " + noHits + "\n");
				sb.Append("Tier 1: " + tier1Drops + " (Super: " + tier1SuperDrops + ")\n");
				sb.Append("Tier 2: " + tier2Drops + " (Super: " + tier2SuperDrops + ")\n");
				sb.Append("Tier 3: " + tier3Drops + " (Super: " + tier3SuperDrops + ")\n");
				sb.Append("Tier 4: " + tier4Drops + " (Super: " + tier4SuperDrops + ")");
			textToOutput["droptable_info"] = sb.ToString();

			// Cheat data
			if (commandHandler.activeCommands.Count < 2)
			{
				textToOutput["cheats_header"] = string.Empty;
				textToOutput["active_cheats"] = string.Empty;
				UpdateText();
			}
			else
			{
				for (int i = 0; i < commandHandler.activeCommands.Count; i++)
				{
					OnDebugCommand(commandHandler.activeCommands[i], true);
				}
			}
		}

		void UpdateText()
		{
			string[] lines = textToOutput.Values.ToArray();
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < lines.Length; i++)
			{
				// If line is not empty string/null
				if (!string.IsNullOrEmpty(lines[i]))
				{
					sb.Append(lines[i]);
					sb.Append("\n");
				}
			}

			if (overlay == null)
			{
				SaveManager.SaveToPrefs("droptables/noHitCount", 0);
				SetupOverlay();
			}

			overlay.text = sb.ToString();
		}

		void OnDamageDone(Entity ent, HitData data)
		{
			// If damage done to player and is not from checkpoint
			if (ent.name == "PlayerEnt" && (data.GetDamageData().Length < 1 || (data.GetDamageData().Length > 0 && data.GetDamageData()[0].damage > 0)))
			{
				string droptableInfo = textToOutput["droptable_info"];

				using (StringReader reader = new StringReader(droptableInfo))
				{
					string line;
					string modifiedLine;

					while ((line = reader.ReadLine()) != null)
					{
						modifiedLine = line;

						if (line.StartsWith("No hits"))
						{
							modifiedLine = line.Remove(0).Insert(0, "No hits: " + 0);
							droptableInfo = droptableInfo.Replace(line, modifiedLine);
							textToOutput["droptable_info"] = droptableInfo;
							break;
						}
					}
				}

				SaveManager.SaveToPrefs("droptables/noHitCount", 0);
			}
		}

		void OnDebugCommand(DebugCommandHandler.CommandInfo command, bool isActive)
		{
			// If command is not debug overlay (itself)
			if (command.nameOfCommand != "DebugOverlay")
			{
				// If no other commands are active
				if (commandHandler.activeCommands.Count < 2)
				{
					ReplaceLine("cheats_header", string.Empty);
					ReplaceLine("active_cheats", string.Empty);
				}
				else ReplaceLine("cheats_header", "===== CHEATS =====");

				// If command is active and is not already in the list
				if (isActive && !textToOutput["active_cheats"].Contains(command.nameOfCommand))
				{
					textToOutput["active_cheats"] += command.nameOfCommand + "\n";
					UpdateText();
				}
				else if (!isActive)
				{
					ReplaceLineWithinLine("active_cheats", command.nameOfCommand + "\n", string.Empty);
				}
			}
		}

		void OnEntDrop(int entTier, bool isSuper, int totalCount, int currentCount, int noHitCount, int currentTierCount, ItemBase currentItem)
		{
			string droptableInfo = textToOutput["droptable_info"];

			using (StringReader reader = new StringReader(droptableInfo))
			{
				string line;
				string modifiedLine;

				while ((line = reader.ReadLine()) != null)
				{
					modifiedLine = line;

					if (line.StartsWith("Master"))
					{
						modifiedLine = line.Remove(0).Insert(0, "Master: " + totalCount);
						droptableInfo = droptableInfo.Replace(line, modifiedLine);
					}

					if (line.StartsWith("Drops"))
					{
						modifiedLine = line.Remove(0).Insert(0, "Drops: " + currentCount);
						droptableInfo = droptableInfo.Replace(line, modifiedLine);
					}

					if (line.StartsWith("No hits"))
					{
						modifiedLine = line.Remove(0).Insert(0, "No hits: " + noHitCount);
						droptableInfo = droptableInfo.Replace(line, modifiedLine);
					}

					if (line.StartsWith("Tier " + entTier))
					{
						if (isSuper)
						{
							modifiedLine = line.Remove(line.IndexOf('('));
							modifiedLine = modifiedLine.Insert(modifiedLine.Length - 1, " (Super: " + currentTierCount + ")");
						}
						else modifiedLine = line.Remove(line.IndexOf(':'), line.IndexOf('(') - line.IndexOf(':')).Insert(6, ": " + currentTierCount + " ");
						droptableInfo = droptableInfo.Replace(line, modifiedLine);
					}

					textToOutput["droptable_info"] = droptableInfo;
				}
			}
		}

		void OnFlagSaved(string flag, object value)
		{
			if (flag == "door" && value.GetType() == typeof(string)) ReplaceLine("spawn", "Spawn: " + value.ToString());
		}

		void OnGameQuit()
		{
			SaveManager.SaveToPrefs("droptables/noHitCount", 0);
		}

		void OnPlayerUpdate()
		{
			Transform playerTrans = VarHelper.PlayerObj.transform;
			ReplaceLine("player_position", "Position: " + playerTrans.position.ToString()); // Update player position
			ReplaceLine("player_rotation", "Angle: " + Mathf.Round(playerTrans.localEulerAngles.y)); // Update player angle
			ReplaceLine("player_speed", "Speed: " + PlayerSpeed); // Update player speed
		}

		void OnRoomLoad(LevelRoom room, bool isActive)
		{
			if (isActive) textToOutput["room"] = "Room: " + room.RoomName;
		}

		void OnSceneLoad(Scene scene)
		{
			SaveManager.SaveToPrefs("droptables/noHitCount", 0);
			if (overlay == null) SetupOverlay();
			ReplaceLine("scene", "Scene: " + scene.name);
			ReplaceLine("timer", string.Empty);
		}

		void OnTimerUpdate(float time)
		{
			string scene = SceneAndRoomHelper.GetLoadedScene().name;
			string timerName = string.Empty;

			// If in Pepperpain Mountain, use cow UFO timer
			if (scene == "VitaminHills3") timerName = "Cow UFO";
			// If in Lost City of Avlopp, use Remedy timer
			else if (scene == "Deep13") timerName = "Remedy";

			if (time > 0) ReplaceLine("timer", timerName + ": " + time);
			else ReplaceLine("timer", string.Empty);
		}

		void ReplaceLine(string lineToReplace, string replaceWith)
		{
			lineToReplace = lineToReplace.ToLower();

			// If line exists
			if (textToOutput[lineToReplace] != null)
			{
				string replacedText = replaceWith;

				// If deleting line
				if (string.IsNullOrEmpty(replaceWith))
				{
					replacedText = Regex.Replace(replaceWith, "^\\s*$[\\r\\n]*", string.Empty, RegexOptions.Multiline);
				}

				textToOutput[lineToReplace] = replacedText;
				UpdateText();
			}
		}

		void ReplaceLineWithinLine(string parentLine, string lineToReplace, string replaceWith)
		{
			// If line exists
			if (textToOutput[parentLine] != null && textToOutput[parentLine].Contains(lineToReplace))
			{
				string replacedText = replaceWith;

				// If deleting line
				if (string.IsNullOrEmpty(replaceWith))
				{
					replacedText = Regex.Replace(replaceWith, "^\\s*$[\\r\\n]*", string.Empty, RegexOptions.Multiline);
				}

				textToOutput[parentLine] = textToOutput[parentLine].Replace(lineToReplace, replacedText);
				UpdateText();
			}
		}

		public static string GetHelp()
		{
			return "Toggles the display of an overlay that shows various debug stats, such as player position & speed, scene data, droptable info, and more.";
		}
	}
}