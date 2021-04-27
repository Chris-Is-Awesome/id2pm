namespace ModStuff.Commands
{
	public class NoClipCommand : DebugCommand
	{
		public override string Activate(string[] args)
		{
			isActive = !isActive;

			if (isActive)
			{
				RunCommand(false);
				return "NoClip is now <color=green>activated</color> for Ittle.";
			}

			Deactivate();
			return "NoClip is now <color=red>deactivated</color> for Ittle.";
		}

		private void RunCommand(bool isRespawn)
		{
			if (!isRespawn)
			{
				VarHelper.PlayerObj.GetComponent<BC_ColliderAACylinderN>().enabled = false;
				DebugManager.LogToFile("[Cheat] NoClip activated for Ittle");
			}
		}

		public void Deactivate()
		{
			VarHelper.PlayerObj.GetComponent<BC_ColliderAACylinderN>().enabled = true;
		}

		public static string GetHelp()
		{
			return "Toggles no clip mode for Ittle. Allows her to walk through everything, even some of the level.";
		}
	}
}