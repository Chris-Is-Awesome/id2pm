﻿using System;
using UnityEngine;
using ModStuff;

public class LevelTimerOverlay : OverlayWindow
{
	[SerializeField]
	string _levelTimer;

	[SerializeField]
	string _format;

	[SerializeField]
	TextMesh _text;

	float oldTime = -1E+10f;

	void Update()
	{
		LevelTime instance = LevelTime.Instance;
		if (instance != null)
		{
			float time = instance.GetTime(this._levelTimer);
			if (time != this.oldTime)
			{
				this.oldTime = time;

				if (!VarHelper.IsAnticheatActive)
				{
					DebugCommandHandler commandHandler = DebugCommandHandler.Instance;

					// If stopwatch is active, prevent text from being updated by vanilla text
					if (commandHandler.stopwatchCommand == null || commandHandler.stopwatchCommand.isActive) return;
				}

				this._text.text = StringUtility.ConvertToTime(time, this._format);
			}
		}
	}
}
