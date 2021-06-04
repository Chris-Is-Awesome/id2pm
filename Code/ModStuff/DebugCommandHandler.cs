using System;
using System.Collections.Generic;
using UnityEngine;
using ModStuff.Commands;

namespace ModStuff
{
	public class DebugCommandHandler : Singleton<DebugCommandHandler>
	{
		public class CommandInfo
		{
			public string nameOfCommand;
			public ActivationMethod activationMethod;
			public DeactivationMethod deactivationMethod;
			public string[] alternateNamesForCommand;
			public bool reactivateIfActive;
			public bool isDevOnly;

			public CommandInfo (string nameOfCommand, ActivationMethod activationMethod, DeactivationMethod deactivationMethod = null, string[] alternateNamesForCommand = null, bool reactivateIfActive = false, bool isDevOnly = false)
			{
				this.nameOfCommand = nameOfCommand;
				this.activationMethod = activationMethod;
				this.deactivationMethod = deactivationMethod;
				this.alternateNamesForCommand = alternateNamesForCommand;
				this.reactivateIfActive = reactivateIfActive;
				this.isDevOnly = isDevOnly;
			}
		}

		public delegate string ActivationMethod(string[] args = null);
		public delegate void DeactivationMethod();
		public List<CommandInfo> allCommands;
		public List<CommandInfo> activeCommands = new List<CommandInfo>();

		// References to commands
		public TestCommand testCommand = new TestCommand();
		//public GotoCommand gotoCommand = new GotoCommand();
		public SpeedCommand speedCommand = new SpeedCommand();
		public GodCommand godCommand = new GodCommand();
		public HelpCommand helpCommand = new HelpCommand();
		public LikeABossCommand likeABossCommand = new LikeABossCommand();
		public NoClipCommand noClipCommand = new NoClipCommand();
		public FindCommand findCommand = new FindCommand();
		public SaveStateCommand saveStateCommand = new SaveStateCommand();
		public LoadStateCommand loadStateCommand = new LoadStateCommand();
		public SetItemsCommand setItemsCommand = new SetItemsCommand();
		public SetHpCommand setHpCommand = new SetHpCommand();
		public StopwatchCommand stopwatchCommand = new StopwatchCommand();
		public DebugOverlayCommand debugOverlayCommand = new DebugOverlayCommand();

		public KeyCode keyToOpenDebugMenu = KeyCode.F1;

		void OnEnable()
		{
			// Create commands
			allCommands = new List<CommandInfo>
			{
				// Dev commands
				{ new CommandInfo("Test", new ActivationMethod(testCommand.Activate), null, null, false, true) },
				{ new CommandInfo("Find", new ActivationMethod(findCommand.Activate), null, null, false, true) },

				// Global commands
				//{ new CommandInfo("Goto", new ActivationMethod(gotoCommand.Activate), null, new string[] { "warpto" }) },
				{ new CommandInfo("SetSpeed", new ActivationMethod(speedCommand.Activate), new DeactivationMethod(speedCommand.Deactivate), new string[] { "speed" }) },
				{ new CommandInfo("God", new ActivationMethod(godCommand.Activate), new DeactivationMethod(godCommand.Deactivate)) },
				{ new CommandInfo("Help", new ActivationMethod(helpCommand.Activate)) },
				{ new CommandInfo("LikeABoss", new ActivationMethod(likeABossCommand.Activate), new DeactivationMethod(likeABossCommand.Deactivate)) },
				{ new CommandInfo("NoClip", new ActivationMethod(noClipCommand.Activate), new DeactivationMethod(noClipCommand.Deactivate)) },
				{ new CommandInfo("SaveState", new ActivationMethod(saveStateCommand.Activate), null, new string[] { "save", "ss" }) },
				{ new CommandInfo("LoadState", new ActivationMethod(loadStateCommand.Activate), null, new string[] { "load", "ls" }) },
				{ new CommandInfo("SetItems", new ActivationMethod(setItemsCommand.Activate), null, new string[] { "setitem", "items", "item" }) },
				{ new CommandInfo("SetHp", new ActivationMethod(setHpCommand.Activate), null, new string[] { "hp" }) },
				{ new CommandInfo("Stopwatch", new ActivationMethod(stopwatchCommand.Activate), new DeactivationMethod(stopwatchCommand.Deactivate), new string[] { "sw" }, true) },
				{ new CommandInfo("DebugOverlay", new ActivationMethod(debugOverlayCommand.Activate), debugOverlayCommand.Deactivate, new string[] { "debug" }, true) },
			};

			keyToOpenDebugMenu = HotkeyHelper.Instance.GetHotkey("OpenDebugMenu").key; // Store hotkey

			DebugManager.LogToFile("DebugCommandHandler initialized!");
		}

		private void OnDisable()
		{
			DeactivateAllCommands();
		}

		public CommandInfo GetCommand(string commandName)
		{
			for (int i  = 0; i < allCommands.Count; i++)
			{
				CommandInfo command = allCommands[i];

				// If name matches internal name
				if (StringHelper.DoStringsMatch(commandName, command.nameOfCommand))
				{
					if (!command.isDevOnly || (command.isDevOnly && VersionHelper.IsDevBuild)) return command;
				}

				// If name matches altnernate names
				if (command.alternateNamesForCommand != null)
				{
					for (int j = 0; j < command.alternateNamesForCommand.Length; j++)
					{
						if (StringHelper.DoStringsMatch(commandName, command.alternateNamesForCommand[j]))
						{
							if (!command.isDevOnly || (command.isDevOnly && VersionHelper.IsDevBuild)) return command;
						}
					}
				}
			}

			return null;
		}

		public CommandInfo GetCommand(Type type)
		{
			for (int i = 0; i < allCommands.Count; i++)
			{
				CommandInfo command = allCommands[i];

				// If type matches command type
				if (type == command.activationMethod.Method.DeclaringType) return command;
			}

			return null;
		}

		public bool IsCommandActive(CommandInfo command)
		{
			Type commandClass = command.activationMethod.Method.DeclaringType;
			return (bool)commandClass.GetField("isActive").GetValue(command.activationMethod.Target);
		}

		public void ActivateCommand(CommandInfo command, string[] args = null)
		{
			// Only activate if not already active, otherwise don't bother (saves on performance)
			if (!IsCommandActive(command) || command.reactivateIfActive) command.activationMethod.Invoke(args);
		}

		public void DeactivateCommand(CommandInfo command)
		{
			// Only deactivate if active, otherwise don't bother (saves on performance)
			if (IsCommandActive(command)) command.deactivationMethod.Invoke();
		}

		public void DeactivateAllCommands()
		{
			for (int i = 0; i < allCommands.Count; i++)
			{
				CommandInfo command = allCommands[i];

				// Only deactivate if active, otherwise don't bother (saves on performance)
				if (IsCommandActive(command)) command.deactivationMethod.Invoke();
			}
		}
	}
}