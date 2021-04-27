using System;
using System.Collections.Generic;
using UnityEngine;

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

		public static bool ParseVector3(string vector, out Vector3 result)
		{
			// If vector given
			if (!string.IsNullOrEmpty(vector))
			{
				string input = vector.Substring(1, vector.Length - 2);
				string[] values = input.Split(","[0]);
				if (values.Length == 3)
				{
					float x = float.Parse(values[0]);
					float y = float.Parse(values[1]);
					float z = float.Parse(values[2]);
					result = new Vector3(x, y, z);
					return true;
				}
			}

			result = Vector3.zero;
			return false;
		}
	}
}