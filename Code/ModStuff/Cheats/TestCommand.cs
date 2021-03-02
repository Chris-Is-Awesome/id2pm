using UnityEngine;
using ModStuff.Utility;

namespace ModStuff.Cheats
{
	public class TestCommand : Singleton<TestCommand>
	{
		public string RunCommand(string[] args)
		{
			return WriteDataToFile(GameObject.Find("PlayerEnt").transform.position);
			//return SaveManager.LoadFromPrefs<string>("test").ToString();
		}

		private string WriteDataToFile(object data)
		{
			SaveManager.SaveToCustomFile(data, "test.txt", "", false);
			return "Successfully wrote test data to file!";
		}
	}
}