using UnityEngine;

namespace ModStuff.Commands
{
	public class GodCommand : DebugCommand
	{
		public override string Activate(string[] args)
		{
			isActive = !isActive;

			if (isActive)
			{
				RunCommand(false);
				EventListener.OnPlayerSpawn += RunCommand;
				return "Godmode is now <color=green>active</color> for Ittle.";
			}

			Deactivate();
			return "Godmode is now <color=red>deactivated</color> for Ittle.";
		}

		private void RunCommand(bool isRespawn)
		{
			if (!isActive) return;
			if (isRespawn) return;

			GameObject playerObj = VarHelper.PlayerObj;

			// Disable hurtbox
			playerObj.transform.Find("Hittable").GetComponent<EntityHittable>().Disable = true;

			// Disable void planes
			Envirodeathable envirodeathable = playerObj.GetComponent<Envirodeathable>();
			if (envirodeathable != null) Object.Destroy(envirodeathable);

			// Full heal
			Killable killable = playerObj.transform.Find("Hittable").GetComponent<Killable>();
			killable.CurrentHp = killable.MaxHp;

			DebugManager.LogToFile("[Cheat] God mode activated for Ittle");
		}

		public void Deactivate()
		{
			isActive = !isActive;
			EventListener.OnPlayerSpawn -= RunCommand;
			GameObject playerObj = VarHelper.PlayerObj;

			// Enable hurtbox
			playerObj.transform.Find("Hittable").GetComponent<EntityHittable>().Disable = false;

			// Enable void planes
			Entity entity = playerObj.GetComponent<Entity>();
			EntityEnvirodeathable entityEnvirodeathable = playerObj.transform.Find("Envirodeath").GetComponent<EntityEnvirodeathable>();
			entityEnvirodeathable.Enable(entity);

			isActive = false;
		}

		public static string GetHelp()
		{
			return "Toggles Ittle invincibility. While active, you take no hit knockback and can't fall into pits.";
		}
	}
}