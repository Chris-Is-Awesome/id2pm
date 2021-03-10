using UnityEngine;

namespace ModStuff.Cheats
{
	public class GodCommand : Singleton<GodCommand>
	{
		public static string GetHelp()
		{
			return "Toggles Ittle invincibility. While active, you take no hit knockback and can't fall into pits.";
		}

		private GameObject player;
		private Entity entity;
		private Killable killable;
		private EntityHittable hittable;
		private Envirodeathable envirodeathable;
		private EntityEnvirodeathable entityEnvirodeathable;

		public string RunCommand(string[] args, bool isActive)
		{
			GetReferences();

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
			GetReferences();
			hittable.Disable = true;
			if (envirodeathable != null) Destroy(envirodeathable);
			killable.CurrentHp = killable.MaxHp;

			PlayerSpawner.RegisterSpawnListener(delegate
			{
				ToggleOn();
			});
		}

		private void ToggleOff()
		{
			hittable.Disable = false;
			entityEnvirodeathable.Enable(entity);
			PlayerSpawner.ClearListeners();
		}

		private void GetReferences()
		{
			if (player == null) player = GameObject.Find("PlayerEnt");
			if (entity == null) entity = player.GetComponent<Entity>();
			if (killable == null) killable = player.transform.Find("Hittable").GetComponent<Killable>();
			if (hittable == null) hittable = killable.GetComponent<EntityHittable>();
			if (envirodeathable == null) envirodeathable = player.GetComponent<Envirodeathable>();
			if (entityEnvirodeathable == null) entityEnvirodeathable = player.transform.Find("Envirodeath").GetComponent<EntityEnvirodeathable>();
		}
	}
}