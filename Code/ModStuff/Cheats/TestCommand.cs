using ModStuff.Utility;

namespace ModStuff.Cheats
{
	public class TestCommand : Singleton<TestCommand>
	{
		public string RunCommand(string[] args)
		{
			return "This will return PlayerPrefs soon! Remind me to write ModSaver!";
		}
	}
}