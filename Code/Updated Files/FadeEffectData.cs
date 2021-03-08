using System;
using UnityEngine;

[PersistentScriptableObject]
public class FadeEffectData : ScriptableObject
{
	[SerializeField]
	public Color _targetColor = Color.black; // Made public

	[SerializeField]
	public float _fadeOutTime = 0.5f; // Made public

	[SerializeField]
	public float _fadeInTime = 1f; // Made public

	[SerializeField]
	public string _faderName; // Made public

	[SerializeField]
	public bool _useScreenPos; // Made public

	public Color StartColor
	{
		get
		{
			Color targetColor = this._targetColor;
			targetColor.a = 0f;
			return targetColor;
		}
	}

	public Color TargetColor
	{
		get
		{
			return this._targetColor;
		}
	}

	public float FadeOutTime
	{
		get
		{
			return this._fadeOutTime;
		}
	}

	public float FadeInTime
	{
		get
		{
			return this._fadeInTime;
		}
	}

	public string FaderName
	{
		get
		{
			return this._faderName;
		}
	}

	public bool UseScreenPos
	{
		get
		{
			return this._useScreenPos;
		}
	}
}
