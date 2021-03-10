using ModStuff.Data;

namespace ModStuff.Cheats
{
	public class GotoCommand : SingletonForCheats<GotoCommand>
	{
		public static string GetHelp()
		{
			// TODO: Make help command

			string description = "<in>goto</in> allows you to load into any scene with any spawn. If you don't specify a room/spawn, you'll load at the fallback/default spawn. Many shorthand names/abbreviations are supported for scene/spawn names. For a list of valid scene, room, and spawn names, check the GitHub.\n\n"; // TODO: Add link to .md file
			string usage = "<out>goto [scene], [room/spawn] (optional)</out>";
			string examples = "<out>goto pillowfort checkpoint</out>, <out>goto pillowfort room C</out>, <out>goto pillowfort</out>";

			return description + usage + examples;
		}

		public string RunCommand(string[] args)
		{
			// If args given
			if (args.Length > 0)
			{
				string wantedScene = args[0];
				GotoData.SceneData sceneData = GotoData.GetSceneData(wantedScene);

				// If valid scene
				if (sceneData != null)
				{
					// If spawn is given
					if (args.Length > 1)
					{
						string wantedSpawn = args[1];

						// If valid room
						for (int i = 0; i < sceneData.rooms.Count; i++)
						{
							GotoData.RoomData room = sceneData.rooms[i];

							if (StringHelper.DoStringsMatch(room.roomName, wantedSpawn))
							{
								// Load room
								SceneAndRoomHelper.LoadRoom(sceneData.realSceneName, room.roomName, room.spawnPosition, room.facingAngle);
								return DebugManager.LogToConsole("Now loading <in>" + sceneData.sceneName + "</in> room <in>" + room.roomName + "</in>...");
							}
						}

						GotoData.SpawnData spawnData = GotoData.GetSpawnData(sceneData.realSceneName, wantedSpawn);

						// If valid spawn
						if (spawnData != null)
						{
							// Load scene at spawn point
							DoLoad(sceneData, spawnData);
							return DebugManager.LogToConsole("Now loading <in>" + sceneData.sceneName + "</in> at <in>" + spawnData.spawnName + "</in>...");
						}

						// If invalid spawn, check if there's a 2nd scene
						sceneData = GotoData.GetSceneData(sceneData.realSceneName + "2");
						spawnData = GotoData.GetSpawnData(sceneData.realSceneName, wantedSpawn);

						// If valid spawn
						if (spawnData != null)
						{
							// Load scene at spawn point
							DoLoad(sceneData, spawnData);
							return DebugManager.LogToConsole("Now loading <in>" + sceneData.sceneName + "</in> at <in>" + spawnData.spawnName + "</in>...");
						}

						// If invalid spawn
						return DebugManager.LogToConsole("<in>" + wantedSpawn + "</in> is not a valid spawn or room for <in>" + sceneData.realSceneName + "</in>. Use <out>help goto</out> for more info.", DebugManager.MessageType.Error);
					}
					// If no spawn given
					else
					{
						// Load scene at fallback spawn
						DoLoad(sceneData);
						return DebugManager.LogToConsole("Now loading <in>" + sceneData.sceneName + "</in> at default spawn...");
					}
				}

				// If invalid scene
				return DebugManager.LogToConsole("<in>" + wantedScene + "</in> is not a valid scene. Use <out>help goto</out> for more info.", DebugManager.MessageType.Error);
			}

			// Return help text if no args given
			return DebugManager.LogToConsole(GetHelp());
		}

		private void DoLoad(GotoData.SceneData sceneData, GotoData.SpawnData spawnData = null)
		{
			// Load with spawn
			if (spawnData != null) SceneAndRoomHelper.LoadScene(sceneData.realSceneName, spawnData.realSpawnName);
			// Load with fallback spawn
			else SceneAndRoomHelper.LoadScene(sceneData.realSceneName);
		}
	}
}