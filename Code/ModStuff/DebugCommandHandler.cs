using System.Collections.Generic;
using UnityEngine;
using ModStuff.Cheats;

namespace ModStuff
{
	public class DebugCommandHandler : Singleton<DebugCommandHandler>
	{
		public delegate string CommandFunc(string[] args);
		public Dictionary<string, CommandFunc> allCommands;

		public KeyCode keyToOpenDebugMenu = KeyCode.F1;

		private void Awake()
		{
			// References to commands
			GotoCommand gotoCommand = new GotoCommand();
			GodCommand godCommand = new GodCommand();
			HelpCommand helpCommand = new HelpCommand();
			SpeedCommand speedCommand = new SpeedCommand();
			TestCommand testCommand = new TestCommand();

			// Create commands
			allCommands = new Dictionary<string, CommandFunc>
			{
				{ "goto", new CommandFunc(gotoCommand.RunCommand) },
				{ "god", new CommandFunc(godCommand.RunCommand) },
				{ "help", new CommandFunc(helpCommand.RunCommand) },
				{ "speed", new CommandFunc(speedCommand.RunCommand) },
				{ "test", new CommandFunc(testCommand.RunCommand) },
			};

			DebugManager.LogToFile(this.GetType().ToString() + " initialized");
		}
	}
}