namespace ModStuff
{
	public static class VersionHelper
	{
		public static string ModVersion
		{
			get
			{
				string version = "v0.1";
				if (!IsPublicRelease)
				{
					if (IsDevBuild) version += "_Dev";
					else if (IsAlphaBuild) version += "_Alpha";
					else if (IsBetaBuild) version += "_Beta";
				}
				return  version;
			}
		}

		public static bool IsPublicRelease
		{
			get
			{
				return false;
			}
		}

		public static bool IsDevBuild
		{
			get
			{
				return false;
			}
		}

		public static bool IsBetaBuild
		{
			get
			{
				return true;
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