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
			};
		}

		private void Test(string[] args)
		{
			OutputText(TestCommand.Instance.RunCommand(args));
		}

		private void OutputText(string output)
		{
			debugMenu.OutputText(output);
		}
	}
}