using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModStuff
{
	public static class SceneAndRoomHelper
	{
		public static void LoadScene(string scene, string spawn = "", bool doSave = true, bool doFade = true, EffectHelper.FadeType fadeType = EffectHelper.FadeType.Circle, Color? fadeColor = null, float fadeOutTime = 0.5f, float fadeInTime = 1f)
		{
			// If saving, trigger save
			if (doSave)
			{
				SaveManager.SaveToSaveFile("start/level", scene);
				SaveManager.SaveToSaveFile("start/door", spawn);
			}

			// If no fade, load scene instantly
			if (!doFade)
			{
				SceneManager.LoadScene(scene);
				return;
			}

			// If fading, make fade & trigger load
			FadeEffectData fadeData = EffectHelper.MakeFadeEffect(fadeType, fadeColor, fadeOutTime, fadeInTime);
			SceneDoor.StartLoad(scene, spawn, fadeData, SaveManager.GetSaverOwner());
		}

		public static void LoadRoom(string scene, string room, Vector3? positionForPlayer = null, Vector3? facingDirectionForPlayer = null)
		{
			LevelRoom realRoom;

			// If not in scene with room
			if (GetLoadedScene().name != scene)
			{
				// Load scene
				LoadScene(scene);

				// Wait until player has spawned
				PlayerSpawner.RegisterSpawnListener(delegate
				{
					// Load room
					realRoom = GameObject.Find("LevelRoot").GetComponent<LevelRoot>().GetRoom(room);
					LevelRoom.SetCurrentActiveRoom(realRoom, true); // Sets room as active & unloads prior room
					GameObject.Find("Cameras").transform.parent.GetComponent<CameraContainer>().SetRoom(realRoom); // Set camera to look at room

					// Teleport player
					if (positionForPlayer != null)
					{
						Transform playerEnt = GameObject.Find("PlayerEnt").transform;
						playerEnt.position = (Vector3)positionForPlayer; // Teleport player

						if (facingDirectionForPlayer != null) playerEnt.localEulerAngles = (Vector3)facingDirectionForPlayer; // Change player facing direction
					}
				});
			}
			else
			{
				MenuHelper.ClosePauseMenu(); // Unpause game (prevents issue with entity animations breaking if visible upon spawn while paused on first frame)

				// Load room
				realRoom = GameObject.Find("LevelRoot").GetComponent<LevelRoot>().GetRoom(room);
				LevelRoom.SetCurrentActiveRoom(realRoom, true); // Sets room as active & unloads prior room
				GameObject.Find("Cameras").transform.parent.GetComponent<CameraContainer>().SetRoom(realRoom); // Sets camera to look at room

				// Teleport player
				if (positionForPlayer != null)
				{
					Transform playerEnt = GameObject.Find("PlayerEnt").transform;
					playerEnt.position = (Vector3)positionForPlayer; // Teleport player

					if (facingDirectionForPlayer != null) playerEnt.localEulerAngles = (Vector3)facingDirectionForPlayer; // Change player facing direction
				}
			}
		}

		public static Scene GetLoadedScene()
		{
			return SceneManager.GetActiveScene();
		}

		public static LevelRoom GetLoadedRoom()
		{
			// TODO: In the case of multiple rooms being loaded, check if setting is enabled, then add each active room to a list and if multiple are indeed active, call GetRoomPlayerIsIn() to get current room

			Transform levelRoot = GameObject.Find("LevelRoot").transform;

			for (int i = 0; i < levelRoot.childCount; i++)
			{
				LevelRoom room = levelRoot.GetChild(i).GetComponent<LevelRoom>();

				if (room != null && room.IsActive) return room;
			}

			return null;
		}

		public static LevelRoom GetRoomPlayerIsIn()
		{
			return LevelRoom.GetRoomForPosition(GameObject.Find("PlayerEnt").transform.position);
		}
	}
}