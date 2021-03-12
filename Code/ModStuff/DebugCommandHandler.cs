using System.Collections.Generic;
using UnityEngine;
using ModStuff.Cheats;

namespace ModStuff
{
	public class DebugCommandHandler : Singleton<DebugCommandHandler>
	{
		public class CommandInfo
		{
			public string nameOfCommand;
			public CommandFunc methodToInvoke;
			public bool isDevOnly;

			public CommandInfo (string nameOfCommand, CommandFunc methodToInvoke, bool isDevOnly = false)
			{
				this.nameOfCommand = nameOfCommand;
				this.methodToInvoke = methodToInvoke;
				this.isDevOnly = isDevOnly;
			}
		}

		public delegate string CommandFunc(string[] args);
		public List<CommandInfo> allCommands;

		public KeyCode keyToOpenDebugMenu = KeyCode.F1;

		private void Awake()
		{
			// References to commands
			TestCommand testCommand = new TestCommand();
			GotoCommand gotoCommand = new GotoCommand();
			SpeedCommand speedCommand = new SpeedCommand();
			GodCommand godCommand = new GodCommand();
			HelpCommand helpCommand = new HelpCommand();

			// Create commands
			allCommands = new List<CommandInfo>
			{
				// Do not put dev commands at bottom of list, as this will add extra comma in help command and
				// is more performant to just not list dev command last rather than remove that trailing comma
				{ new CommandInfo("Test", new CommandFunc(testCommand.RunCommand), true) },
				{ new CommandInfo("Goto", new CommandFunc(gotoCommand.RunCommand)) },
				{ new CommandInfo("Speed", new CommandFunc(speedCommand.RunCommand)) },
				{ new CommandInfo("God", new CommandFunc(godCommand.RunCommand)) },
				{ new CommandInfo("Help", new CommandFunc(helpCommand.RunCommand)) },
			};

			DebugManager.LogToFile(this.GetType().ToString() + " initialized");
		}

		public CommandInfo GetCommand(string commandName)
		{
			for (int i  = 0; i < allCommands.Count; i++)
			{
				CommandInfo command = allCommands[i];
				if (StringHelper.DoStringsMatch(commandName, command.nameOfCommand))
				{
					if (!command.isDevOnly || (command.isDevOnly && VersionHelper.IsDevBuild)) return command;
				}
			}

			return null;
		}
	}
}