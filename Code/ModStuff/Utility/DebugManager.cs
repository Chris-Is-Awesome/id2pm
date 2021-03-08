using System.IO;
using UnityEngine;

namespace ModStuff.Utility
{
	public static class DebugManager
	{
		public enum MessageType
		{
			Info,
			Success,
			Warn,
			Error
		}

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

		public static string LogDebugMessageToConsole(string message,  MessageType type, bool doFormat = true)
		{
			string output = string.Empty;
			string infoColor = "<color=#141414>";
			string successColor = "<color=#078716>";
			string warningColor = "<color=#c95e00>";
			string errorColor = "<color=#db1414>";

			switch (type)
			{
				case MessageType.Success:
					if (doFormat) output = successColor;
					break;
				case MessageType.Warn:
					if (doFormat) output = warningColor;
					output += "WARNING: ";
					break;
				case MessageType.Error:
					if (doFormat) output = errorColor;
					output += "ERROR: ";
					break;
				default:
					if (doFormat) output = infoColor;
					break;
			}

			output += message;
			if (doFormat) output += "</color>";

			return output;
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