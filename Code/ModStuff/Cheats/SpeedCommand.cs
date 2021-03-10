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

		// speed self/player/ittle/ent value/reset
		public string RunCommand(string[] args)
		{
			// If args given
			if (args.Length > 0)
			{
				string howToEffect = args[0];

				// If resetting all Entities
				if (IsValidArgOfMany(howToEffect, new List<string> { "reset", "default", "def" }))
				{
					ResetSpeedForEnts();
					return DebugManager.LogToConsole("Reset speed for all Entities.", DebugManager.MessageType.Success);
				}
				// If not resetting
				else
				{
					if (args.Length > 1)
					{
						string multiplier = args[1];

						// If number is given
						if (ParseArgToNumber(multiplier, out float velMultiplier))
						{
							// If applying to Ittle
							if (IsValidArgOfMany(howToEffect, new List<string> { "self", "player", "ittle" }))
							{
								SetSpeedForIttle(velMultiplier);
								return DebugManager.LogToConsole("Set speed multiplier to <in>" + velMultiplier + "</in> for <in>Ittle</in>.", DebugManager.MessageType.Success);
							}
							// If applying to everyone
							else if (StringHelper.DoStringsMatch(howToEffect, "all") || StringHelper.DoStringsMatch(howToEffect, "everyone"))
							{
								SetSpeedForEnts(velMultiplier);
								return DebugManager.LogToConsole("Set speed multiplier to <in>" + velMultiplier + "</in> all Entities.", DebugManager.MessageType.Success);
							}

							// If arg1 is invalid
							return DebugManager.LogToConsole("Value <in>" + howToEffect + "</in> is not a valid value. Please specify a <out>target (string)</out>. Use <out>help speed</out> for more info.", DebugManager.MessageType.Error);
						}

						// If speed not specified
						return DebugManager.LogToConsole("Value <in>" + multiplier + "</in> is not a number. Please specify a <out>speed (float)</out>. Use <out>help speed</out> for more info.", DebugManager.MessageType.Error);
					}
				}

				// If arg0 is invalid
				return DebugManager.LogToConsole("Value <in>" + howToEffect + "</in> is not a valid value. Please specify a <out>target (string)</out>. Use <out>help speed</out> for more info.", DebugManager.MessageType.Error);
			}

			return DebugManager.LogToConsole(GetHelp());
		}

		private void SetSpeedForIttle(float multiplier)
		{
			RigidBodyController rigidbody = GameObject.Find("PlayerEnt").GetComponent<RigidBodyController>();
			SetSpeedForRigidbody(rigidbody, multiplier);
		}

		private void SetSpeedForEnts(float multiplier)
		{
			List<RigidBodyController> allRigidbodies = GetAllRigidbodies();
			int numOfRigidbodies = 0;

			for (int i = 0; i < allRigidbodies.Count; i++)
			{
				numOfRigidbodies++;
				SetSpeedForRigidbody(allRigidbodies[i], multiplier);
			}
		}

		private void ResetSpeedForEnts()
		{
			List<RigidBodyController> allRigidbodies = GetAllRigidbodies();
			int numOfRigidbodies = 0;

			for (int i = 0; i < allRigidbodies.Count; i++)
			{
				RigidBodyController rigidbody = allRigidbodies[i];

				// Only reset if it needs to
				if (rigidbody.velocityMultiplier != 1)
				{
					numOfRigidbodies++;
					SetSpeedForRigidbody(rigidbody, 1);
				}
			}
		}

		private void SetSpeedForRigidbody(RigidBodyController rigidbody, float multiplier)
		{
			rigidbody.velocityMultiplier = multiplier;
		}

		private List<RigidBodyController> GetAllRigidbodies()
		{
			List<RigidBodyController> rigidbodies = new List<RigidBodyController>();

			foreach (Entity ent in Resources.FindObjectsOfTypeAll<Entity>())
			{
				RigidBodyController rigidbody = ent.GetComponent<RigidBodyController>();

				if (rigidbody != null) rigidbodies.Add(rigidbody);
			}

			return rigidbodies;
		}

		public bool ParseArgToNumber(string arg, out float num)
		{
			bool isFloat = float.TryParse(arg, out num);
			return isFloat;
		}
	}
}