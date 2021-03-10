using UnityEngine;
using ModStuff.Utility;

namespace ModStuff.Cheats
{
	public class GodCommand : Singleton<GodCommand>
	{
		public static string GetHelp()
		{
			return "You really need help <i>here</i>?! At this point, you really should consult Tippsie...";
		}

		private static bool isActive;

		public static bool IsActive
		{
			get
			{
				return isActive;
			}
		}

		private GameObject player;
		private Entity entity;
		private Killable killable;
		private EntityHittable hittable;
		private Envirodeathable envirodeathable;
		private EntityEnvirodeathable entityEnvirodeathable;

		public string RunCommand(string[] args)
		{
			GetReferences();

			isActive = !isActive; // Toggle
			hittable.Disable = isActive; // Disable hurtbox
			
			if (isActive)
			{
				if (envirodeathable != null) Destroy(envirodeathable); // Disable environmental damage
				killable.CurrentHp = killable.MaxHp; // Full heal

				return DebugManager.LogToConsole("Godmode is now <color=green>active</color> for Ittle.");
			}

			entityEnvirodeathable.Enable(entity);
			return DebugManager.LogToConsole("Godmode is now <color=red>deactivated</color> for Ittle.");
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