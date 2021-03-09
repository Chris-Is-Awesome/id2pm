using System.Collections.Generic;
using UnityEngine;
using ModStuff.Utility;

namespace ModStuff.Data
{
	public static class GotoData
	{
		public enum SceneType
		{
			Overworld,
			Cave,
			PortalWorld,
			Dungeon,
			Special,
		}

		public class SceneData
		{
			public string sceneName;
			public string realSceneName;
			public SceneType type;
			public List<string> otherNames;
			public List<RoomData> rooms;
			public List<SpawnData> spawns;

			public SceneData(string sceneName, string realSceneName, SceneType type, List<string> otherNames, List<RoomData> rooms, List<SpawnData> spawns)
			{
				this.sceneName = sceneName;
				this.realSceneName = realSceneName;
				this.type = type;
				this.otherNames = otherNames;
				this.rooms = rooms;
				this.spawns = spawns;
			}
		}

		public class SpawnData
		{
			public string spawnName;
			public string realSpawnName;
			public List<string> otherNames;
			public string room;

			public SpawnData(string spawnName, string realSpawnName, List<string> otherNames, string room)
			{
				this.spawnName = spawnName;
				this.realSpawnName = realSpawnName;
				this.otherNames = otherNames;
				this.room = room;
			}
		}

		public class RoomData
		{
			public string roomName;
			public Vector3 spawnPosition;
			public Vector3 facingAngle;
			public string identifier;

			public RoomData(string roomName, Vector3 spawnPosition, Vector3 facingAngle, string identifier = "")
			{
				this.roomName = roomName;
				this.spawnPosition = spawnPosition;
				this.facingAngle = facingAngle;
				this.identifier = identifier;
			}
		}

		private static List<SceneData> data;

		// Generates scene & spawn data
		private static void MakeData()
		{
			/*
			 * Fluffy: D1, S1, checkpoint, lake, autumn climb, warp garden, pepperpain, fancy1, fancy2, star, coast
			 * Coast: D2, vault, painful plain, slope, fluffy, star, changing tent, checkpoint
			 */

			/*
			 * deep1  - Autumn Climb 
				deep2  - The Vault 
				deep3  - Painful Plain 
				deep4  - Farthest Shore 
				deep5  - Scrap Yard 
				deep6  - Brutal Oasis 
				deep7  - Former Colossus 
				deep8  - Sand Crucible 
				deep9  - Ocean Castle
				deep10 - Promenade Path 
				deep11 - Maze of Steel 
				deep12 - Wall of Text 
				deep13 - Lost City of Avlopp 
				deep14 - Northern End 
				deep15 - Moon Garden 
				deep16 - Nowhere 
				deep17 - Cave of Mystery 
				deep18 - Somewhere 
				deep19 - Test Room 
				deep20 - Ludo City 
				deep21 - Abyssal Plain 
				deep22 - Place From Younger Days 
				deep23 - Abandoned House
				deep24 - House in DW (no name?)
				deep25 - Unnamed dark room in DW
				deep26 - bad dream heheheh
			 */

			// Overworld
			data = new List<SceneData>
			{
				new SceneData("Fluffy Fields", // User-friendly scene name (output)
					"FluffyFields", // Real scene name (logic)
					SceneType.Overworld, // Type (output)
					new List<string> { "FluffyFields", "Fluffy", "FF", "Fields" }, // Valid names for scene (input)
					new List<RoomData>
					{
						// Real room name, spawn position, facing direction, optional name for room (eg. "boss")
						{ new RoomData("A", new Vector3(5.5f, 0f, -36.5f), new Vector3(0f, 0f, 0f)) },
						{ new RoomData("B", new Vector3(31.5f, 0f, -58f), new Vector3(0f, 270f, 0f)) },
						{ new RoomData("C", new Vector3(60f, 0f, -40.5f), new Vector3(0f, 180f, 0f)) },
					},
					new List<SpawnData>
					{
						// User-friendly spawn name (output), real spawn name (logic), valid names for spawn (input), room name (logic)
						{ new SpawnData("Pillow Fort entrance", "PillowFortOutside", new List<string> { "PillowFort", "PF", "D1" }, "A") },
						{ new SpawnData("Checkpoint", "RestorePt1", new List<string> { "checkpoint", "restorepoint", "cp", "safezone" }, "A") },
						{ new SpawnData("Sunken Labyrinth entrance", "SunkenLabyrinthOutside", new List<string> { "SunkenLabyrinth", "SL", "S1" }, "B") },
						{ new SpawnData("Warp Garden", "CaveP", new List<string> { "WarpGarden", "garden", "warps" }, "C") },
						{ new SpawnData("Autumn Climb entrance", "CaveA", new List<string> { "AutumnClimb", "AC", "Deep1" }, "A") },
						{ new SpawnData("Dream World", "DreamWorldOutside", new List<string> { "DreamWorld", "DW" }, "C") }
					}
				),
				new SceneData("Sweetwater Coast", // User-friendly scene name (output)
					"CandyCoast", // Real scene name (logic)
					SceneType.Overworld, // Type (output)
					new List<string> { "SweetwaterCoast", "CandyCoast", "Coast", "Beach" }, // Valid names for scene (input)
					new List<RoomData>
					{
						// Real room name, spawn position, facing direction, optional name for room (eg. "boss")
						{ new RoomData("A", new Vector3(48.5f, 0f, -77.5f), new Vector3(0f, 270f, 0f)) },
						{ new RoomData("B", new Vector3(52.5f, 0f, -77.6f), new Vector3(0f, 90f, 0f)) },
						{ new RoomData("C", new Vector3(103.5f, 0f, -74.15f), new Vector3(0f, 90f, 0f)) },
					},
					new List<SpawnData>
					{
						// User-friendly spawn name (output), real spawn name (logic), valid names for spawn (input), room name (logic)
						{ new SpawnData("Sand Castle entrance", "SandCastleOutside", new List<string> { "SandCastle", "SC", "D2" }, "A") },
						{ new SpawnData("Checkpoint", "RestorePt1", new List<string> { "checkpoint", "restorepoint", "cp" }, "A") },
						{ new SpawnData("Changing Tent", "CaveQ", new List<string> { "ChangingTent", "OutfitTent", "tent", "beachtent" }, "C") },
						{ new SpawnData("The Vault entrance", "CaveE", new List<string> { "TheVault", "TV", "vault", "Deep2" }, "A") },
						{ new SpawnData("Painful Plain entrance", "CaveO", new List<string> { "PainfulPlain", "pain", "Deep3" }, "B") },
						{ new SpawnData("Western Sweetwater Coast", "CaveG", new List<string> { "west", "western" }, "A") },
						{ new SpawnData("Northern Sweetwater Coast", "CaveA", new List<string> { "north", "northern" }, "A") },
						{ new SpawnData("Southern Sweetwater Coast", "CaveK", new List<string> { "south", "southern" }, "A") },
						{ new SpawnData("Eastern Sweetwater Coast", "RestorePt3", new List<string> { "east", "eastern" }, "B") }
					}
				),
				new SceneData("Fancy Ruins", // User-friendly scene name (output)
					"FancyRuins", // Real scene name (logic)
					SceneType.Overworld, // Type (output)
					new List<string> { "FancyRuins", "Fancy", "FR", "FR2" }, // Valid names for scene (input)
					new List<RoomData>
					{
						// Real room name, spawn position, facing direction, optional name for room (eg. "boss")
						{ new RoomData("A", new Vector3(25.5f, 0.37f, -36.5f), new Vector3(0f, 0f, 0f)) },
						{ new RoomData("B", new Vector3(25.5f, 0.37f, -40.5f), new Vector3(0f, 180f, 0f)) },
						{ new RoomData("C", new Vector3(73.98f, 0f, -40.5f), new Vector3(0f, 180f, 0f)) },
					},
					new List<SpawnData>
					{
						// User-friendly spawn name (output), real spawn name (logic), valid names for spawn (input), room name (logic)
						{ new SpawnData("Machine Fortress entrance", "MachineFortressOutside", new List<string> { "MachineFortress", "MF", "S2" }, "A") },
						{ new SpawnData("Farthest Shore entrance", "CaveF", new List<string> { "FarthestShore", "FS", "Deep4" }, "A") },
						{ new SpawnData("Scrap Yard entrance", "CaveR", new List<string> { "ScrapYard", "SY", "Deep5" }, "C") },
						{ new SpawnData("Checkpoint", "RestorePt1", new List<string> { "checkpoint", "restorepoint", "cp" }, "A") },
						{ new SpawnData("Southern Fancy Ruins", "CaveJ", new List<string> { "south", "southern" }, "B") },
						{ new SpawnData("Northern Fancy Ruins", "CaveB", new List<string> { "north", "northern" }, "A") }
					}
				),
				new SceneData("Fancy Ruins", // User-friendly scene name (output)
					"FancyRuins2", // Real scene name (logic)
					SceneType.Overworld, // Type (output)
					new List<string> { "FancyRuins", "Fancy", "FR", "FR2" }, // Valid names for scene (input)
					new List<RoomData>
					{
						// Real room name, spawn position, facing direction, optional name for room (eg. "boss")
						{ new RoomData("A", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)) },
					},
					new List<SpawnData>
					{
						// User-friendly spawn name (output), real spawn name (logic), valid names for spawn (input), room name (logic)
						{ new SpawnData("Art Exhibit entrance", "ArtExhibitOutside", new List<string> { "ArtExhibit", "art", "exhibit", "AE", "D3" }, "A") }
					}
				),
				new SceneData("", // User-friendly scene name (output)
					"", // Real scene name (logic)
					SceneType.Overworld, // Type (output)
					new List<string> { "" }, // Valid names for scene (input)
					new List<RoomData>
					{
						// Real room name, spawn position, facing direction, optional name for room (eg. "boss")
						{ new RoomData("A", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)) },
					},
					new List<SpawnData>
					{
						// User-friendly spawn name (output), real spawn name (logic), valid names for spawn (input), room name (logic)
						{ new SpawnData("", "", new List<string> { "" }, "") }
					}
				),
			};
		}

		// Returns SceneData
		public static SceneData GetSceneData(string scene)
		{
			if (data == null || data.Count < 1) MakeData();

			for (int i = 0; i < data.Count; i++)
			{
				SceneData sceneData = data[i];

				if (Core.DoStringsMatch(sceneData.realSceneName, scene))
				{
					return sceneData;
				}

				for (int j = 0; j < sceneData.otherNames.Count; j++)
				{
					string otherName = sceneData.otherNames[j];

					if (Core.DoStringsMatch(otherName, scene))
					{
						return sceneData;
					}
				}
			}

			DebugManager.LogToFile("No SceneData was found for scene " + scene + ". Returning null.", LogType.Error);
			return null;
		}

		// Returns SpawnData
		public static SpawnData GetSpawnData(string scene, string spawn)
		{
			if (data == null || data.Count < 1) MakeData();

			SceneData sceneData = GetSceneData(scene);

			if (sceneData != null)
			{
				for (int i = 0; i < sceneData.spawns.Count; i++)
				{
					SpawnData spawnData = sceneData.spawns[i];

					if (Core.DoStringsMatch(spawnData.realSpawnName, spawn))
					{
						return spawnData;
					}

					for (int j = 0; j < spawnData.otherNames.Count; j++)
					{
						string otherName = spawnData.otherNames[j];

						if (Core.DoStringsMatch(otherName, spawn))
						{
							return spawnData;
						}
					}
				}
			}

			DebugManager.LogToFile("No SpawnData was found for spawn " + spawn + ". Returning null.", LogType.Error);
			return null;
		}
	}
}