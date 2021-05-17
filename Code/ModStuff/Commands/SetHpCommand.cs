namespace ModStuff.Commands
{
	public class SetHpCommand : DebugCommand
	{
		public override string Activate(string[] args)
		{
			// If 1+ args given
			if (args.Length > 0)
			{
				// If setting max HP
				if (IsValidArg(args[0], "max"))
				{
					// If 2nd arg given and is int
					if (args.Length > 1 && TryParseInt(args[1], out int hp))
					{
						Killable killable = VarHelper.PlayerObj.GetComponentInChildren<Killable>();
						killable.MaxHp = hp;
						killable.CurrentHp = hp;
						VarHelper.PlayerObj.GetComponent<Entity>().SaveState();
						SaveManager.GetSaverOwner().SaveAll();

						return DebugManager.LogToConsole("Set current HP & max HP to " + hp + "!", DebugManager.MessageType.Success);
					}

					// If invalid arg
					return DebugManager.LogToConsole("Must specify what value to set max HP to (int). Use <out>help sethp</out> for more info", DebugManager.MessageType.Error);
				}
				// If setting to full
				else if (IsValidArg(args[0], "full"))
				{
					Killable killable = VarHelper.PlayerObj.GetComponentInChildren<Killable>();
					killable.MaxHp = 40;
					killable.CurrentHp = 40;
					VarHelper.PlayerObj.GetComponent<Entity>().SaveState();
					SaveManager.GetSaverOwner().SaveAll();

					return DebugManager.LogToConsole("Set current HP & max HP to " + 40 + "!", DebugManager.MessageType.Success);
				}
				// If setting current hp
				else if (TryParseInt(args[0], out int hp))
				{
					Killable killable = VarHelper.PlayerObj.GetComponentInChildren<Killable>();
					if (hp > killable.MaxHp) killable.MaxHp = hp;
					killable.CurrentHp = hp;
					VarHelper.PlayerObj.GetComponent<Entity>().SaveState();
					SaveManager.GetSaverOwner().SaveAll();

					return DebugManager.LogToConsole("Set current HP to " + hp + "!", DebugManager.MessageType.Success);
				}

				// If invalid arg
				return DebugManager.LogToConsole("<in>" + args[0] + "</in> is an invalid argument. Use <out>help sethp</out> for more info", DebugManager.MessageType.Error);
			}

			return DebugManager.LogToConsole("Must specify an argument. Use <out>help sethp</out> for more info", DebugManager.MessageType.Error);
		}

		public static string GetHelp()
		{
			return "Coming soon...";
		}
	}
}