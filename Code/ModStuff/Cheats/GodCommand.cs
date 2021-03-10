using UnityEngine;

namespace ModStuff.Cheats
{
	public class GodCommand : SingletonForCheats<GodCommand>
	{
		private bool isActive;

		public static string GetHelp()
		{
			return "Toggles Ittle invincibility. While active, you take no hit knockback and can't fall into pits.";
		}

		public string RunCommand(string[] args)
		{
			isActive = !isActive;

			if (isActive)
			{
				ToggleOn();
				return DebugManager.LogToConsole("Godmode is now <color=green>active</color> for Ittle.");
			}

			ToggleOff();
			return DebugManager.LogToConsole("Godmode is now <color=red>deactivated</color> for Ittle.");
		}

		private void ToggleOn()
		{
			GameObject playerObj = VarHelper.PlayerObj;

			// Disable hurtbox
			playerObj.transform.Find("Hittable").GetComponent<EntityHittable>().Disable = true;

			// Disable void planes
			Envirodeathable envirodeathable = playerObj.GetComponent<Envirodeathable>();
			if (envirodeathable != null) Destroy(envirodeathable);

			// Full heal
			Killable killable = playerObj.transform.Find("Hittable").GetComponent<Killable>();
			killable.CurrentHp = killable.MaxHp;

			PlayerSpawner.RegisterSpawnListener(delegate
			{
				ToggleOn();
			});
		}

		private void ToggleOff()
		{
			GameObject playerObj = VarHelper.PlayerObj;

			// Enable hurtbox
			playerObj.transform.Find("Hittable").GetComponent<EntityHittable>().Disable = false;

			// Enable void planes
			Entity entity = playerObj.GetComponent<Entity>();
			EntityEnvirodeathable entityEnvirodeathable = playerObj.transform.Find("Envirodeath").GetComponent<EntityEnvirodeathable>();
			entityEnvirodeathable.Enable(entity);

			PlayerSpawner.ClearListeners();
		}
	}
}