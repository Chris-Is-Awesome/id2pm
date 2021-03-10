using UnityEngine;

namespace ModStuff
{
	public static class MenuHelper
	{
		public static void ClosePauseMenu()
		{
			// Closes debug menu & pause menu
			GameObject pauseOverlay = GameObject.Find("PauseOverlay");

			if (pauseOverlay != null)
			{
				pauseOverlay.GetComponentInChildren<DebugMenu>().Hide();
				pauseOverlay.GetComponent<PauseMenu>().Hide();
			}
		}
	}
}