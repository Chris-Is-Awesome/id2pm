using System;
using System.IO;
using UnityEngine;

namespace ModStuff
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

		public static void EnableDebugging()
		{
			// Enable Unity logging
			Debug.logger.logEnabled = true;
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
		}

		public static void LogToFile(string message, LogType logType = LogType.Log, bool includeStackTrace = false, bool addWhiteSpace = true)
		{
			string output = FormatDebugMessage(message, logType, addWhiteSpace);

			if (includeStackTrace)
			{
				output += "Stack trace:\n";
				string stackTrace = StackTraceUtility.ExtractStackTrace();
				using (StringReader reader = new StringReader(stackTrace))
				{
					string line = string.Empty;
					while((line = reader.ReadLine()) != null)
					{
						if (line.Contains(nameof(LogToFile))) continue;
						output += "-> " + line + "\n";
					}
				}
			}

			Debug.Log(output);
		}

		public static string LogToConsole(string message, MessageType type = MessageType.Info, bool includeStackTraceOnError = false, bool doFormat = true)
		{
			string output = string.Empty;
			string infoColor = "<color=#141414>";
			string successColor = "<color=#078716>";
			string warningColor = "<color=#c95e00>";
			string errorColor = "<color=#db1414>";
			string inputColor = "<color=#6e6e6e>";
			string outputColor = "<color=#3d3d3d>";

			// Add beginning formatting
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

			// Add final formatting
			if (doFormat)
			{
				// Replace any input/output tags for emphasis
				output = output.Replace("<in>", inputColor + "<i>");
				output = output.Replace("</in>", "</i></color>");
				output = output.Replace("<out>", outputColor + "[");
				output = output.Replace("</out>", "]</color>");

				output += "</color>";

				// Add stacktrace
				if (type == MessageType.Error && includeStackTraceOnError)
				{
					output += "\n\n<size=15>-------------------------\n" + StackTraceUtility.ExtractStackTrace() + "</size>";
				}
			}

			return output;
		}

		private static string FormatDebugMessage(string message, LogType logType, bool addWhiteSpace)
		{
			string formattedMessage = message;

			switch (logType)
			{
				case LogType.Warning:
					formattedMessage = "[WARN] " + formattedMessage;
					break;
				case LogType.Error:
					formattedMessage = "[ERROR] " + formattedMessage;
					break;
			}

			string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
			formattedMessage = "[E2D] [" + timestamp + "] " + formattedMessage;

			if (addWhiteSpace)
			{
				formattedMessage = "\n" + formattedMessage + "\n";
			}

			return formattedMessage;
		}
	}
}