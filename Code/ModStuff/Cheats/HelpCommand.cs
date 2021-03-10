using System;
using System.Reflection;

namespace ModStuff.Cheats
{
	public class HelpCommand : Singleton<HelpCommand>
	{
		public static string GetHelp()
		{
			return "You really need help <i>here</i>?! At this point, you really should consult Tippsie...";
		}

		public string RunCommand(string[] args)
		{
			string className = "ModStuff.Cheats." + char.ToUpper(args[0][0]) + args[0].Substring(1) + "Command";
			MethodInfo command = Type.GetType(className).GetMethod("GetHelp");
			return DebugManager.LogToConsole(command.Invoke(null, null).ToString());
		}
	}
}