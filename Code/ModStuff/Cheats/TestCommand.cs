using ModStuff.Utility;

namespace ModStuff.Cheats
{
	public class TestCommand : Singleton<TestCommand>
	{
		public string RunCommand(string[] args)
		{
			return SaveManager.LoadFromPrefs<string>("test").ToString();
		}
	}
}