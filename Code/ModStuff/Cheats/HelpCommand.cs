using System;
using System.Reflection;
using ModStuff.Utility;

namespace ModStuff.Cheats
{
	public class HelpCommand : Singleton<HelpCommand>
	{
		public string RunCommand(string[] args)
		{
			// Convert arg0 to class name (generic?) and return GetHelp() ????????????
			// Bad way: switch
			string className = char.ToUpper(args[0][0]) + args[0].Substring(1) + "Command";
			MethodInfo command = Type.GetType("ModStuff.Cheats." + className).GetMethod("GetHelp");
			return DebugManager.LogToConsole(command.Invoke(null, null).ToString());
		}
	}
}