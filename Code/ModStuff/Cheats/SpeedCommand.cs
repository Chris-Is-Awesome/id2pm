using System.Collections.Generic;
using UnityEngine;

namespace ModStuff.Cheats
{
	public class SpeedCommand : SingletonForCheats<SpeedCommand>
	{
		public static string GetHelp()
		{
			string description = "<in>speed</in> lets you change the speed of various Entities. You can move faster or slower. Also affects roll speed. A negative speed value will let you move in reverse.\n\n";
			string usage = "<out>speed [int]{speed}</out> OR <out>speed [string]{reset/default}</out>";
			string examples = "<out>speed 15</out>, <out>speed -5</out>, <out>speed reset</out>";

			return description + usage + examples;
		}

		public string RunCommand(string[] args)
		{
			// If args given
			if (args.Length > 0)
			{
				string arg0 = args[0];

				// If resetting all Entities
				if (IsValidArgOfMany(arg0, new List<string> { "reset", "default", "def" }))
				{
					ResetSpeed();
					return DebugManager.LogToConsole("Reset speed for all Entities.", DebugManager.MessageType.Success);
				}
				// If not resetting & number is given
				else if (TryParseToFloat(arg0, out float velMultiplier))
				{
					velMultiplier /= 5;
					SetSpeed(velMultiplier);
					return DebugManager.LogToConsole("Set Ittle's speed to <in>" + velMultiplier + "</in>");
				}

				// If arg0 is invalid
				return DebugManager.LogToConsole("Value <in>" + arg0 + "</in> is not a valid value. Use <out>help speed</out> for more info.", DebugManager.MessageType.Error);
			}

			return DebugManager.LogToConsole(GetHelp());
		}

		private void SetSpeed(float multiplier)
		{
			RigidBodyController rigidbody = GameObject.Find("PlayerEnt").GetComponent<RigidBodyController>();
			SetSpeedForRigidbody(rigidbody, multiplier);
		}

		private void ResetSpeed()
		{
			SetSpeed(1);
		}

		private void SetSpeedForRigidbody(RigidBodyController rigidbody, float multiplier)
		{
			rigidbody.SetCustomVelocity(multiplier);
		}
	}
}