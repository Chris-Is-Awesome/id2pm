using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModStuff.Data
{
	[Serializable]
	public class HotkeyData
	{
		[Serializable]
		public class HotkeyJson
		{
			public List<Data> hotkeys;

			[Serializable]
			public class Data
			{
				public string name;
				public string key;
			}
		}

		public class Hotkey
		{
			public string name;
			public KeyCode key;
			public DebugCommandHandler.CommandInfo commandToRun;
			public string[] commandArgs;

			public Hotkey(string name, KeyCode key, DebugCommandHandler.CommandInfo commandToRun = null, string[] commandArgs = null)
			{
				this.name = name;
				this.key = key;
				this.commandToRun = commandToRun;
				this.commandArgs = commandArgs;
			}
		}

		public List<Hotkey> hotkeys = new List<Hotkey>();
	}
}