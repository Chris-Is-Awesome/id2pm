using System.IO;
using UnityEngine;

namespace ModStuff.Utility
{
	public static class DebugManager
	{
		public static void LogDebugMessage(string message, LogType logType = LogType.Log, bool includeStackTrace = true, bool isHeader = false, bool addWhiteSpace = true)
		{
			string output = FormatDebugMessage(message, logType, isHeader, addWhiteSpace);

			if (includeStackTrace)
			{
				output += "Stack trace:\n";
				string stackTrace = StackTraceUtility.ExtractStackTrace();
				using (StringReader reader = new StringReader(stackTrace))
				{
					string line = string.Empty;
					while((line = reader.ReadLine()) != null)
					{
						if (line.Contains(nameof(LogDebugMessage))) continue;
						output += "-> " + line + "\n";
					}
				}
			}

			Debug.Log(output);
		}

		private static string FormatDebugMessage(string message, LogType logType, bool isHeader, bool addWhiteSpace)
		{
			string formattedMessage =  message;

			switch (logType)
			{
				case LogType.Warning:
					formattedMessage = "[WARN]: " + formattedMessage;
					break;
				case LogType.Error:
					formattedMessage = "[ERROR]: " + formattedMessage;
					break;
			}

			if (isHeader)
			{
				formattedMessage = "---------- " + formattedMessage + " ----------";
			}

			formattedMessage = "[E2D] " + formattedMessage;

			if (addWhiteSpace)
			{
				formattedMessage = "\n" + formattedMessage + "\n";
			}

			return formattedMessage;
		}
	}
}