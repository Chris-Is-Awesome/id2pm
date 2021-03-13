namespace ModStuff
{
	public static class VersionHelper
	{
		public static string ModVersion
		{
			get
			{
				string version = "v0.0";
				if (IsDevBuild) version += "_Dev";
				else if (IsAlphaBuild) version += "_Alpha";
				else if (IsBetaBuild) version += "_Beta";
				return  version;
			}
		}

		public static bool IsDevBuild
		{
			get
			{
				// TODO: Return true depending on Steam user currently playing?
				return true;
			}
		}

		public static bool IsChrisBuild
		{
			get
			{
				// TODO: Return true if options file has secret key in it
				return false;
			}
		}

		public static bool IsBetaBuild
		{
			get
			{
				return false;
			}
		}

		public static bool IsAlphaBuild
		{
			get
			{
				return false;
			}
		}
	}
}