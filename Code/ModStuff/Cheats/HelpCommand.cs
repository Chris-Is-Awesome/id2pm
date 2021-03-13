using System.Collections.Generic;

namespace ModStuff.Cheats
{
	public class HelpCommand : DebugCommand
	{
		public override string Activate(string[] args)
		{
			// If args given, output command help
			if (args.Length > 0) return GetHelpForCommand(args[0]);

			// If no args given, output list of commands
			return GetListOfCommands();
		}

		private string GetHelpForCommand(string arg)
		{
			DebugCommandHandler.CommandInfo command = DebugCommandHandler.Instance.GetCommand(arg); // Get command

			// If valid command
			if (command != null) return command.activationMethod.Method.DeclaringType.GetMethod("GetHelp").Invoke(null, null).ToString();

			// If invalid command
			return "<in>" + arg + "</in> is not a command. Use <out>help</out> to get list of commands";
		}

		private string GetListOfCommands()
		{
			string output = string.Empty;
			List<DebugCommandHandler.CommandInfo> allCommands = DebugCommandHandler.Instance.allCommands;

			for (int i = 0; i < allCommands.Count; i++)
			{
				DebugCommandHandler.CommandInfo command = allCommands[i];

				// If is dev command, only output if is dev build
				if (command.isDevOnly)
				{
					if (!VersionHelper.IsDevBuild) continue;
				}

				output += command.nameOfCommand;
				if (i < allCommands.Count - 1) output += ", ";
			}

			return output;
		}

		public static string GetHelp()
		{
			return "You really need help <i>here</i>?! At this point, you really should consult Tippsie...";
		}
	}
}