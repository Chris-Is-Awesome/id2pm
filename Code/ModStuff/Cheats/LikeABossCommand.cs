namespace ModStuff.Cheats
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

		private void RunCommand(Entity ent, bool isActive)
		{
			// If first time using command in session, use ents stored in VarHelper
			if (ent == null)
			{
				for (int i = 0; i < VarHelper.ActiveEnts.Count; i++)
				{
					MakeIttleStrong(VarHelper.ActiveEnts[i]);
				}
			}
			// If active ent
			else MakeIttleStrong(ent);

			DebugManager.LogToFile("[Cheat] LikeABoss activated for Ittle");
		}

		public void Deactivate()
		{
			EventListener.OnEntitySpawn -= RunCommand;
		}

		private void MakeIttleStrong(Entity ent)
		{
			Killable killable = ent.GetComponentInChildren<Killable>();
			if (killable != null) killable.CurrentHp = 0;
		}

		public static string GetHelp()
		{
			return "Toggles one hit kill mode for Ittle. All enemies, even normally incincible ones, will die in one swing of your stick.";
		}
	}
}