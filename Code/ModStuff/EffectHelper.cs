using UnityEngine;

namespace ModStuff
{
	public static class EffectHelper
	{
		public enum FadeType
		{
			Circle,
			Flash,
			Fullscreen,
		}

		public static FadeEffectData MakeFadeEffect(FadeType type, Color? color, float outTime, float inTime)
		{
			string fadeType;

			switch (type)
			{
				case FadeType.Flash:
					fadeType = "AdditiveFade";
					break;
				case FadeType.Fullscreen:
					fadeType = "ScreenFade";
					break;
				default:
					fadeType = "ScreenCircleWipe";
					break;
			}

			FadeEffectData fadeData = new FadeEffectData
			{
				_faderName = fadeType,
				_targetColor = color ?? Color.black,
				_fadeOutTime = outTime,
				_fadeInTime = inTime,
				_useScreenPos = true
			};

			return fadeData;
		}
	}
}