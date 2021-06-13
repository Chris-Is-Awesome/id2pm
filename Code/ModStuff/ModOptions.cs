namespace ModStuff
{
	public static class ModOptions
	{
		public static void SaveOption(string option, bool value)
		{
			SaveManager.SaveToPrefs(option, value.ToString());
		}

		public static bool LoadOption(string option)
		{
			return bool.Parse(SaveManager.LoadFromPrefs<string>(option).ToString());
		}

		public static void DeleteOption(string option)
		{
			SaveManager.DeletePref(option);
		}
	}
}