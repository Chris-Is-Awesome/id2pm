namespace ModStuff.Commands
{
	public class LikeABossCommand : DebugCommand
	{
		public override string Activate(string[] args)
		{
			isActive = !isActive;

			if (isActive)
			{
				RunCommand(null, false);
				EventListener.OnEntitySpawn += RunCommand;
				return "LikeABoss is now <color=green>active</color> for Ittle.";
			}

			Deactivate();
			return "LikeABoss is now <color=red>deactivated</color> for Ittle.";
		}

		private void RunCommand(Entity ent, bool isEntActive)
		{
			// If first time using command in session, use ents stored in VarHelper
			if (ent == null)
			{
				for (int i = 0; i < VarHelper.ActiveEnts.Count; i++)
				{
					MakeIttleStrong(VarHelper.ActiveEnts[i], true);
				}
			}
			// If activ ent
			else if (ent != null && isEntActive) MakeIttleStrong(ent, true);
		}

		public void Deactivate()
		{
			EventListener.OnEntitySpawn -= RunCommand;

			for (int i = 0; i < VarHelper.ActiveEnts.Count; i++)
			{
				MakeIttleStrong(VarHelper.ActiveEnts[i], false);
			}
		}

		private void MakeIttleStrong(Entity ent, bool isLikeABoss)
		{
			if (ent != null)
			{
				Killable killable = ent.GetComponentInChildren<Killable>();
				if (killable != null)
				{
					if (isLikeABoss) killable.CurrentHp = 0;
					else killable.CurrentHp = killable.MaxHp;
				}
			}
		}

		public static string GetHelp()
		{
			return "Toggles one hit kill mode for Ittle. All enemies, even normally incincible ones, will die in one swing of your stick.";
		}
	}
}