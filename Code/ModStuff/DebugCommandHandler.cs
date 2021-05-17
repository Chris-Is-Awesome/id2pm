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
			public bool isDevOnly;

			public CommandInfo (string nameOfCommand, ActivationMethod activationMethod, DeactivationMethod deactivationMethod = null, bool isDevOnly = false)
			{
				this.nameOfCommand = nameOfCommand;
				this.activationMethod = activationMethod;
				this.deactivationMethod = deactivationMethod;
				this.isDevOnly = isDevOnly;
			}
		}

		public delegate string ActivationMethod(string[] args = null);
		public delegate void DeactivationMethod();
		public List<CommandInfo> allCommands;

		// References to commands
		public TestCommand testCommand = new TestCommand();
		public GotoCommand gotoCommand = new GotoCommand();
		public SpeedCommand speedCommand = new SpeedCommand();
		public GodCommand godCommand = new GodCommand();
		public HelpCommand helpCommand = new HelpCommand();
		public LikeABossCommand likeABossCommand = new LikeABossCommand();
		public NoClipCommand noClipCommand = new NoClipCommand();
		public FindCommand findCommand = new FindCommand();
		public SaveStateCommand saveStateCommand = new SaveStateCommand();
		public LoadStateCommand loadStateCommand = new LoadStateCommand();
		public SetItemsCommand setItemsCommand = new SetItemsCommand();

		public KeyCode keyToOpenDebugMenu = KeyCode.F1;

		private void Awake()
		{
			// Create commands
			allCommands = new List<CommandInfo>
			{
				// Do not put dev commands at bottom of list, as this will add extra comma in help command and
				// is more performant to just not list dev command last rather than remove that trailing comma
				{ new CommandInfo("Test", new ActivationMethod(testCommand.Activate), null, true) },
				{ new CommandInfo("Goto", new ActivationMethod(gotoCommand.Activate)) },
				{ new CommandInfo("Speed", new ActivationMethod(speedCommand.Activate), new DeactivationMethod(speedCommand.Deactivate)) },
				{ new CommandInfo("God", new ActivationMethod(godCommand.Activate), new DeactivationMethod(godCommand.Deactivate)) },
				{ new CommandInfo("Help", new ActivationMethod(helpCommand.Activate)) },
				{ new CommandInfo("LikeABoss", new ActivationMethod(likeABossCommand.Activate), new DeactivationMethod(likeABossCommand.Deactivate)) },
				{ new CommandInfo("NoClip", new ActivationMethod(noClipCommand.Activate), new DeactivationMethod(noClipCommand.Deactivate)) },
				{ new CommandInfo("Find", new ActivationMethod(findCommand.Activate)) },
				{ new CommandInfo("SaveState", new ActivationMethod(saveStateCommand.Activate)) },
				{ new CommandInfo("LoadState", new ActivationMethod(loadStateCommand.Activate)) },
				{ new CommandInfo("SetItems", new ActivationMethod(setItemsCommand.Activate)) },
			};

			keyToOpenDebugMenu = HotkeyHelper.Instance.GetHotkey("OpenDebugMenu").key;
			DebugManager.LogToFile("DebugCommandHandler initialized");
		}

		public CommandInfo GetCommand(string commandName)
		{
			for (int i  = 0; i < allCommands.Count; i++)
			{
				CommandInfo command = allCommands[i];
				if (StringHelper.DoStringsMatch(commandName, command.nameOfCommand))
				{
					if (!command.isDevOnly || (command.isDevOnly && VersionHelper.IsDevBuild)) return command;
				}
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
			if (!IsCommandActive(command)) command.activationMethod.Invoke(args);
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