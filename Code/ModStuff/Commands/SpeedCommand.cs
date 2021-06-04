using System.Collections.Generic;
using UnityEngine;

namespace ModStuff.Commands
{
	public class SpeedCommand : DebugCommand
	{
		private float multiplier;

		public override string Activate(string[] args)
		{
			// If args given
			if (args.Length > 0)
			{
				string arg0 = args[0];

				// If resetting
				if (IsValidArgOfMany(arg0, new List<string> { "reset", "default", "def" }))
				{
					Deactivate();
					return "Reset speed for Ittle to default.";
				}
				// If not resetting & number is given
				else if (TryParseToFloat(arg0, out multiplier))
				{
					MakeActive(GetType());
					RunCommand(false);
					EventListener.OnPlayerSpawn += RunCommand;
					return "Set Ittle's speed to <in>" + multiplier + "</in>";
				}

				// If arg0 is invalid
				return DebugManager.LogToConsole("Value <in>" + arg0 + "</in> is not a valid value. Use <out>help speed</out> for more info.", DebugManager.MessageType.Error);
			}

			// If no args given
			return GetHelp();
		}

		private void RunCommand(bool isRespawn)
		{
			if (!isActive) return;
			if (isRespawn) return;

			RigidBodyController rigidbody = VarHelper.PlayerObj.GetComponent<RigidBodyController>();
			rigidbody.SetCustomVelocity(multiplier);

			DebugManager.LogToFile("[Cheat] Ittle's speed set to " + multiplier);
		}

		public void Deactivate()
		{
			EventListener.OnPlayerSpawn -= RunCommand;
			GameObject playerObj = VarHelper.PlayerObj;
			if (playerObj != null)
			{
				RigidBodyController rigidbody = playerObj.GetComponent<RigidBodyController>();
				rigidbody.SetCustomVelocity(1);
			}
			MakeInactive(GetType());
		}

		public static string GetHelp()
		{
			string description = "Change the speed of Ittle. You can move faster or slower. Also affects roll & knockback speeds. A negative speed value will let you move in reverse.\n\n";
			string aliases = "Aliases: speed\n";
			string usage = "Usage: <out>speed {float}</out> OR <out>speed reset/default</out>\n";
			string examples = "Examples: <out>speed 15.5</out>, <out>speed -5</out>, <out>speed reset</out>";

			return description + aliases + usage + examples;
		}
	}
}