using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModStuff.Commands
{
	public class StopwatchCommand : DebugCommand
	{
		bool hasStarted;
		bool isPaused;
		float currentTime;
		int frameCount;
		TextMesh timerOverlayTextMesh;
		bool hasConfiguredOverlay;
		Coroutine animationCoroutine;
		bool isAnimating;

		public override string Activate(string[] args)
		{
			// If first arg given
			if (args.Length > 0)
			{
				switch (args[0].ToLower())
				{
					case "start":
					case "begin":
						if (isPaused) return PauseStopwatch(); // If stopwatch is paused, resume it
						if (hasStarted) StopStopwatch(); // If stopwatch has already been started, stop it
						return StartStopwatch(); // If stopwatcch is not paused, start it
					case "pause":
					case "unpause":
						if (hasStarted) return PauseStopwatch(); // If stopwatch is active, pause/unpause it
						return DebugManager.LogToConsole("Stopwatch is not active. Start a stopwatch with <out>stopwatch start</out>. Use <out>help stopwatch</out> for more info.", DebugManager.MessageType.Warn);
					case "stop":
					case "end":
						if (hasStarted) return StopStopwatch(); // If stopwatch is active, stop it
						return DebugManager.LogToConsole("Stopwatch is not active. Start a stopwatch with <out>stopwatch start</out>. Use <out>help stopwatch</out> for more info.", DebugManager.MessageType.Warn);
					case "restart":
						if (hasStarted) StopStopwatch(); // Stop active stopwatch
						return StartStopwatch(); // Start a new stopwatch
					default:
						return DebugManager.LogToConsole("First argument must be <out>start</out>, <out>stop</out>, <out>pause</out>, or <out>restart</out>. Use <out>help stopwatch</out> for more info.", DebugManager.MessageType.Error);
				}
			}

			return DebugManager.LogToConsole(GetHelp());
		}

		public void Deactivate()
		{
			StopStopwatch();
		}

		string StartStopwatch()
		{
			// Reset values to defaults
			currentTime = 0;
			frameCount = 0;

			if (isAnimating)
			{
				DebugCommandHandler.Instance.StopCoroutine(animationCoroutine); // Stop animating
				isAnimating = false;
				isActive = false;
				ConfigureOverlay();
			}

			isActive = true; // Set command to active to allow resuming command after loads
			hasStarted = true; // Start stopwatch

			EventListener.OnPlayerUpdate += UpdateStopwatch; // Only update timer as long as the player has control (slightly different from IGT to be more accurate to actual play)
			EventListener.OnPlayerSpawn += UpdateOverlay; // Initialize timer on load
			EventListener.OnSceneUnload += OnSceneUnload;

			return DebugManager.LogToConsole("Stopwatch started!", DebugManager.MessageType.Success);
		}

		string PauseStopwatch()
		{
			// If not paused, pause
			if (!isPaused)
			{
				isPaused = true;
				EventListener.OnPlayerUpdate -= UpdateStopwatch; // Stop updating
				return "Stopwatch paused at: <in>" + GetFormattedTime() + "</in> (<in>" + frameCount + "</in> frames)";
			}

			// If paused, resume
			isPaused = false;
			EventListener.OnPlayerUpdate += UpdateStopwatch; // Only update timer as long as the player has control (slightly different from IGT to be more accurate to actual play)
			return "Stopwatch resumed at: <in>" + GetFormattedTime() + "</in> (<in>" + frameCount + "</in> frames)";
		}

		string StopStopwatch()
		{
			EventListener.OnPlayerUpdate -= UpdateStopwatch; // Stop updating
			EventListener.OnPlayerSpawn -= UpdateOverlay; // Unsubscribe

			hasStarted = false;

			if (!isAnimating) animationCoroutine = DebugCommandHandler.Instance.StartCoroutine(AnimateText()); // Animate to indicate timer end

			return "Stopwatch stopped at: <in>" + GetFormattedTime() + "</in> (<in>" + frameCount + "</in> frames)";
		}

		void UpdateStopwatch()
		{
			// If stopwatch started and is not paused
			if (hasStarted && !isPaused)
			{
				// Update time
				currentTime += Time.deltaTime;
				frameCount++;

				// Update timer overlay
				UpdateOverlay();
			}
		}

		void UpdateOverlay(bool isRespawn = false)
		{
			// Activate timer, if not already
			if (timerOverlayTextMesh == null)
			{
				SaveManager.SaveToSaveFile("settings/showTime", "1"); // Saves flag
				// Note: This overlay has to have its parent be instantiated so this is the cleanest way to do this. Can't get away with simply activating the object
				GameObject.Find("Cameras").transform.parent.GetComponent<EntityHUD>().UpdateSaveVarWindows(); // Activates timer overlay
				timerOverlayTextMesh = GameObject.Find("PlayTimeOverlay").transform.GetChild(1).GetComponent<TextMesh>(); // Store reference

				hasConfiguredOverlay = false;
			}

			if (!hasConfiguredOverlay) ConfigureOverlay();

			// Update time shown in overlay
			timerOverlayTextMesh.text = GetFormattedTime() + "\n(" + frameCount + " frames)";
		}

		void ConfigureOverlay()
		{
			if (timerOverlayTextMesh != null)
			{
				// Make it look better during stopwatch
				if (isActive)
				{
					// Rescale text to make it fit better
					timerOverlayTextMesh.transform.localScale = new Vector3(0.2f, 0.2f, 0.1f);

					// Move text up slightly to center it better
					float x = timerOverlayTextMesh.transform.localPosition.x;
					float y = timerOverlayTextMesh.transform.localPosition.y + 0.05f;
					float z = timerOverlayTextMesh.transform.localPosition.z;
					timerOverlayTextMesh.transform.localPosition = new Vector3(x, y, z);

					hasConfiguredOverlay = true;
				}
				// Reset to defaults
				else
				{
					timerOverlayTextMesh.color = Color.black;
					float x = timerOverlayTextMesh.transform.localPosition.x;
					float y = timerOverlayTextMesh.transform.localPosition.y - 0.05f;
					float z = timerOverlayTextMesh.transform.localPosition.z;
					timerOverlayTextMesh.transform.localPosition = new Vector3(x, y, z);
					timerOverlayTextMesh.transform.localScale = new Vector3(0.3f, 0.3f, 0.1f);

					hasConfiguredOverlay = false;
				}
			}
		}

		string GetFormattedTime()
		{
			TimeSpan timespan = TimeSpan.FromSeconds(currentTime);

			// If minutes > 0, show minutes
			if (timespan.Minutes > 0)
			{
				return timespan.Minutes + "m " + timespan.Seconds + "s " + timespan.Milliseconds + "ms";
			}

			// If seconds > 0, show seconds
			if (timespan.Seconds > 0)
			{
				return timespan.Seconds + "s " + timespan.Milliseconds + "ms";
			}

			// If timespan.Milliseconds > 0, show milliseconds
			return timespan.Milliseconds + "ms";
		}

		void OnSceneUnload(Scene scene)
		{
			// Only stop animation if currently animating
			if (isAnimating)
			{
				if (animationCoroutine != null) DebugCommandHandler.Instance.StopCoroutine(animationCoroutine);
				isAnimating = false;
				isActive = false; // Set command to inactive so it won't resume command after loads
				EventListener.OnSceneUnload -= OnSceneUnload;
			}
		}

		public static string GetHelp()
		{
			return "Coming soon...";
		}

		IEnumerator AnimateText()
		{
			isAnimating = true;

			Color32 normalColor = new Color32(0, 0, 0, 255);
			Color32 flashColor = new Color32(7, 135, 22, 255);

			timerOverlayTextMesh.color = flashColor;

			yield return new WaitForSeconds(0.5f);
			timerOverlayTextMesh.color = normalColor;

			yield return new WaitForSeconds(0.5f);
			timerOverlayTextMesh.color = flashColor;

			// 1s has passed

			yield return new WaitForSeconds(0.5f);
			timerOverlayTextMesh.color = normalColor;

			yield return new WaitForSeconds(0.5f);
			timerOverlayTextMesh.color = flashColor;

			// 2s have passed

			yield return new WaitForSeconds(0.5f);
			timerOverlayTextMesh.color = normalColor;

			yield return new WaitForSeconds(0.5f);
			timerOverlayTextMesh.color = flashColor;

			// 3s have passed

			yield return new WaitForSeconds(0.5f);
			timerOverlayTextMesh.color = normalColor;

			yield return new WaitForSeconds(0.5f);
			timerOverlayTextMesh.color = flashColor;

			// 4s have passed

			yield return new WaitForSeconds(0.5f);
			timerOverlayTextMesh.color = normalColor;
			isActive = false; // Set command to inactive so it won't resume command after loads
			ConfigureOverlay(); // Reset overlay
			isAnimating = false;
		}
	}
}