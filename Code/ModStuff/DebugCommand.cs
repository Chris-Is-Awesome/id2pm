using System.Collections.Generic;

namespace ModStuff
{
	public abstract class DebugCommand
	{
		public abstract string RunCommand(string[] args);

		public bool isActive;

		public bool IsValidArg(string arg, string validArg)
		{
			return StringHelper.DoStringsMatch(arg, validArg);
		}

		public bool IsValidArgOfMany(string arg, List<string> validArgs)
		{
			for (int i = 0; i < validArgs.Count; i++)
			{
				if (StringHelper.DoStringsMatch(arg, validArgs[i])) return true;
			}

			return false;
		}

		public bool TryParseToFloat(string arg, out float num)
		{
			bool isFloat = float.TryParse(arg, out num);
			return isFloat;
		}

		public bool TryParseInt(string arg, int num)
		{
			bool  isInt = int.TryParse(arg, out num);
			return isInt;
		}
	}
}