using System;
using System.Collections.Generic;

namespace ModStuff
{
	public static class StringHelper
	{
		public enum StringConversionType
		{
			None,
			ToString,
			GetType,
		}

		public static bool DoStringsMatch(string string1, string string2, bool ignoreCase = true)
		{
			if (!ignoreCase) return string1 == string2;
			return string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);
		}

		public static bool DoesStringContain(string parentString, string isThisInIt, bool ignoreCase = true)
		{
			if (!ignoreCase) return parentString.Contains(isThisInIt);
			return parentString.ToLower().Contains(isThisInIt.ToLower());
		}

		public static string GetStringFromList<T>(List<T> values, string divider = ",", StringConversionType conversionType = StringConversionType.ToString)
		{
			string output = string.Empty;

			for (int i = 0; i < values.Count; i++)
			{
				switch (conversionType)
				{
					case StringConversionType.ToString:
						output += values[i].ToString();
						break;
					case StringConversionType.GetType:
						output += values[i].GetType().ToString();
						break;
					default:
						output += values[i];
						break;
				}

				// Only add divider if not on last element
				if (i < values.Count - 1) output += divider;
			}

			return output;
		}
	}
}