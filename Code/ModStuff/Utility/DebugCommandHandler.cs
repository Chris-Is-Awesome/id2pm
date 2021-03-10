using System.Collections.Generic;
using UnityEngine;
using ModStuff.Cheats;

namespace ModStuff.Utility
{
	public class DebugCommandHandler : Singleton<DebugCommandHandler>
	{
		public delegate void CommandFunc(string[] args);
		public Dictionary<string, CommandFunc> allCommands;

		public KeyCode keyToOpenDebugMenu = KeyCode.F1;
		public DebugMenu debugMenu;

		private void Awake()
		{
			InitializeCommands();
		}

		private void InitializeCommands()
		{
			allCommands = new Dictionary<string, CommandFunc>
			{
				{ "test", new CommandFunc(Test) },
				{ "help", new CommandFunc(Help) },
				{ "goto", new CommandFunc(Goto) },
				{ "speed", new CommandFunc(Speed) },
				{ "god", new CommandFunc(God) },
			};
		}

		private void Test(string[] args)
		{
			OutputText(TestCommand.Instance.RunCommand(args));
		}

		private void Help(string[] args)
		{
			OutputText(HelpCommand.Instance.RunCommand(args));
		}

		private void Goto(string[] args)
		{
			OutputText(GotoCommand.Instance.RunCommand(args));
		}

		private void Speed(string[] args)
		{
			OutputText(SpeedCommand.Instance.RunCommand(args));
		}

		private void God(string[] args)
		{
			OutputText(GodCommand.Instance.RunCommand(args));
		}

		private void OutputText(string output)
		{
			debugMenu.OutputText(output);
		}
	}
}