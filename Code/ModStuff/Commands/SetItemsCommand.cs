using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ModStuff.Commands
{
	public class SetItemsCommand : DebugCommand
	{
		[Serializable]
		public class ItemList
		{
			public List<ItemData> items;

			[Serializable]
			public class ItemData
			{
				public string friendlyName;
				public string realName;
				public int realValue = -1;
				public List<string> validNames;
				public int maxLevelOrCount = 999;
				public bool hasInfiniteCount;
				public bool isFinalLevelDev;
			}
		}

		public override string Activate(string[] args)
		{
			// If 1+ args given
			if (args.Length > 0)
			{
				ItemList itemList = GetItemList("ItemData.json");

				// If setting all items (excluding dev)
				if (IsValidArg(args[0], "all") || IsValidArg(args[0], "max"))
				{
					BulkAssignItems(itemList, "all");
					return DebugManager.LogToConsole("Obtained all items at max levels (excluding dev items)!", DebugManager.MessageType.Success);
				}

				// If adding all items (including dev)
				if (IsValidArg(args[0], "dev"))
				{
					BulkAssignItems(itemList, "dev");
					return DebugManager.LogToConsole("Obtained all items at max levels (including dev items!", DebugManager.MessageType.Success);
				}

				// If removing all items
				if (IsValidArg(args[0], "none"))
				{
					BulkAssignItems(itemList, "none");
					return DebugManager.LogToConsole("Removed all items.", DebugManager.MessageType.Success);
				}

				ItemList.ItemData item = GetItem(itemList, args[0]);

				// If item found
				if (item != null)
				{
					// If 2+ args given
					if (args.Length > 1)
					{
						// If valid number is given
						if (TryParseInt(args[1], out int value) && value >= 0)
						{
							// If item is infinite (key, shard, raft, etc.), no limit
							if (item.hasInfiniteCount)
							{
								SaveManager.SaveToEnt(item.realName, value);
								return DebugManager.LogToConsole("Set <in>" + item.friendlyName + "</in> count to <in>" + value + "</in> !", DebugManager.MessageType.Success);
							}

							// If item has a real value, use that instead of user-defined value
							if (item.realValue >= 0) value = item.realValue;

							// If level/count is above max for item, set to max
							if (value > item.maxLevelOrCount) value = item.maxLevelOrCount;

							SaveManager.SaveToEnt(item.realName, value);
							
							// If not removing item & not using realValue, normal output
							if (value > 0 && item.realValue < 0)
							{
								return DebugManager.LogToConsole("Obtained <in>" + item.friendlyName + "</in> at level <in>" + value + "</in> !", DebugManager.MessageType.Success);
							}

							// If not removing item, output that
							if (item.realValue >= 0)
							{
								return DebugManager.LogToConsole("Obtained <in>" + item.friendlyName + "</in> !", DebugManager.MessageType.Success);
							}

							// If removing item, output that
							return DebugManager.LogToConsole("Removed item <in>" + item.friendlyName + "</in> !", DebugManager.MessageType.Success);
						}

						// If valid number not given
						return DebugManager.LogToConsole("Argument <in>" + args[1] + "</in> is not a valid integer. Must not be a negative. Use <out>help setitems</out> for more info.", DebugManager.MessageType.Error);
					}

					// If no level/count given, default

					// If item has a realValue, use that for default
					if (item.realValue >= 0) SaveManager.SaveToEnt(item.realName, item.realValue);
					else
					{
						// If item is infinite (key, shard, raft, etc.) add 1 & change wording of output
						if (item.hasInfiniteCount)
						{
							SaveManager.SaveToEnt(item.realName, SaveManager.LoadFromEnt(item.realName) + 1);
							return DebugManager.LogToConsole("Added 1 <in>" + item.friendlyName + "</in> !", DebugManager.MessageType.Success);
						}
						else SaveManager.SaveToEnt(item.realName, 1);
					}

					// If item is finite, set to 1
					return DebugManager.LogToConsole("Obtained <in>" + item.friendlyName + "</in> !", DebugManager.MessageType.Success);
				}

				// If item not found
				return DebugManager.LogToConsole("Item <in>" + args[0] + "</in> was not found. Is there a typo? Use <out>help setitems</out> for more info.", DebugManager.MessageType.Error);
			}

			return DebugManager.LogToConsole("Argument is required. Use <out>help setitems</out> for more info.", DebugManager.MessageType.Error);
		}

		private void BulkAssignItems(ItemList itemList, string bulkType)
		{
			for (int i = 0; i < itemList.items.Count; i++)
			{
				ItemList.ItemData item = itemList.items[i];

				switch (bulkType.ToLower())
				{
					case "all":
						// If dev item, don't upgrade to last level
						if (item.isFinalLevelDev) SaveManager.SaveToEnt(item.realName, item.maxLevelOrCount - 1);
						else SaveManager.SaveToEnt(item.realName, item.maxLevelOrCount);
						break;
					case "dev":
						SaveManager.SaveToEnt(item.realName, item.maxLevelOrCount);
						break;
					case "none":
						SaveManager.SaveToEnt(item.realName, 0);
						break;
				}
			}
		}

		private ItemList.ItemData GetItem(ItemList itemList, string itemName)
		{
			for (int i = 0; i < itemList.items.Count; i++)
			{
				ItemList.ItemData item = itemList.items[i];

				// If item doesn't have extra names, use real name
				if (item.validNames.Count < 1)
				{
					if (IsValidArg(itemName, item.realName)) return item;
				}

				// If item name matches any valid names
				for (int j = 0; j < item.validNames.Count; j++)
				{
					if (IsValidArg(itemName, item.validNames[j])) return item;
				}
			}

			// If item not found
			return null;
		}

		private ItemList GetItemList(string localFilePath = "")
		{
			// If reading from local file (usually for testing)
			if (!string.IsNullOrEmpty(localFilePath))
			{
				string modDirectory = FileManager.GetModDirectoryPath();
				return JsonUtility.FromJson<ItemList>(File.ReadAllText(modDirectory + localFilePath));
			}

			// TODO: If reading from server, fetch file
			return null;
		}

		public static string GetHelp()
		{
			return "Coming soon...";
		}
	}
}