﻿using System.Collections.Generic;
using UnityEngine;

namespace ModStuff.Cheats
{
	public class SpeedCommand : SingletonForCheats<SpeedCommand>
	{
		private bool isActive;

		public static string GetHelp()
		{
			string description = "<in>speed</in> lets you change the speed of Ittle. You can move faster or slower. Also affects roll & knockback speeds. A negative speed value will let you move in reverse.\n\n";
			string usage = "<out>speed [float]{speed}</out> OR <out>speed [string]{reset/default}</out>";
			string examples = "<out>speed 15</out>, <out>speed -5</out>, <out>speed reset</out>";

			return description + usage + examples;
		}

		public string RunCommand(string[] args)
		{
			// If args given
			if (args.Length > 0)
			{
				string arg0 = args[0];

				// If resetting
				if (IsValidArgOfMany(arg0, new List<string> { "reset", "default", "def" }))
				{
					ToggleOff();
					return DebugManager.LogToConsole("Reset speed for Ittle to default.");
				}
				// If not resetting & number is given
				else if (TryParseToFloat(arg0, out float multiplier))
				{
					isActive = true;
					ToggleOn(multiplier);
					return DebugManager.LogToConsole("Set Ittle's speed to <in>" + multiplier + "</in>");
				}

				// If arg0 is invalid
				return DebugManager.LogToConsole("Value <in>" + arg0 + "</in> is not a valid value. Use <out>help speed</out> for more info.", DebugManager.MessageType.Error);
			}

			// If no args given
			return DebugManager.LogToConsole(GetHelp());
		}

		private void ToggleOn(float multiplier)
		{
			if (!isActive) return;

			RigidBodyController rigidbody = VarHelper.PlayerObj.GetComponent<RigidBodyController>();
			rigidbody.SetCustomVelocity(multiplier);

			PlayerSpawner.RegisterSpawnListener(delegate
			{
				DebugManager.LogToFile("[Cheat] Ittle's speed set to " + multiplier);
				ToggleOn(multiplier);
			});
		}

		private void ToggleOff()
		{
			isActive = false;
			RigidBodyController rigidbody = VarHelper.PlayerObj.GetComponent<RigidBodyController>();
			rigidbody.SetCustomVelocity(1);
		}
	}
}