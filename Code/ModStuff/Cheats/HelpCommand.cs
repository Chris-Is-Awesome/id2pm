using System;
using System.Reflection;

namespace ModStuff.Cheats
{
	public class HelpCommand : DebugCommand
	{
		public override string RunCommand(string[] args)
		{
			// If args given
			if (args.Length > 0)
			{
				string arg0 = args[0];
				string className = "ModStuff.Cheats." + char.ToUpper(arg0[0]) + arg0.Substring(1) + "Command";
				MethodInfo command = Type.GetType(className).GetMethod("GetHelp");
				return DebugManager.LogToConsole(command.Invoke(null, null).ToString());
			}

			// TODO: Output list of commands
			return "This will print a list of commands... eventually...";
		}

		public static string GetHelp()
		{
			return "You really need help <i>here</i>?! At this point, you really should consult Tippsie...";
		}
	}
}