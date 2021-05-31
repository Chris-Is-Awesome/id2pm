using System.Collections.Generic;
using UnityEngine;

namespace ModStuff
{
	public abstract class DebugCommand
	{
		public abstract string Activate(string[] args);

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

		public bool TryParseInt(string arg, out int num)
		{
			bool  isInt = int.TryParse(arg, out num);
			return isInt;
		}

		public bool TryParseToVector3(string x, string y, string z, out Vector3 vector)
		{
			if (TryParseToFloat(x, out float _x) && TryParseToFloat(y, out float _y) && TryParseToFloat(z, out float _z))
			{
				vector = new Vector3(_x, _y, _z);
				return true;
			}

			vector = Vector3.zero;
			return false;
		}

		public string CombineArgsToString(string[] args, bool useDivider = false, char divider = '_')
		{
			string output = string.Empty;

			for (int i = 0; i < args.Length; i++)
			{
				output += args[i];
				if (useDivider && i < args.Length - 1 && !args[i].Contains(divider.ToString())) output += divider;
			}

			return output;
		}
	}
}