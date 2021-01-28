using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ModStuff.Utility;

public class DebugMenu : MonoBehaviour
{
	[SerializeField]
	float _blinkTime = 0.5f;

	[SerializeField]
	GameObject _layout;

	[SerializeField]
	MappedInput _input;

	[SerializeField]
	GameVersion _version;

	GuiWindow menuRoot;

	string currentText;

	DebugMenu.OnDoneFunc onDone;

	MappedInput.ButtonEventListener charListener;

	MappedInput.ButtonEventListener dirListener;

	TextInput.Listener textListener;

	List<string> prevComs = new List<string>();

	int currPrevPos;

	float blinkTimer;

	bool hasCaret;

	int ignoreFirstFramesDestroyUnity;

	DebugCommandHandler commandHandler = DebugCommandHandler.Instance;

	void Setup()
	{
		GuiBindInData inData = new GuiBindInData(null, null);
		GuiBindData guiBindData = GuiNode.Connect(this._layout, inData);
		this.menuRoot = guiBindData.GetTracker<GuiWindow>("debugRoot");
		guiBindData.GetTrackerEvent<IGuiOnclick>("debug.done").onclick = new GuiNode.OnVoidFunc(this.ClickedDone);
		guiBindData.GetTrackerEvent<IGuiOnclick>("debug.back").onclick = new GuiNode.OnVoidFunc(this.ClickedCancel);
		GuiContentData guiContentData = new GuiContentData();
		guiContentData.SetValue("version", this._version.GetVersion());
		this.menuRoot.ApplyContent(guiContentData, true);
	}

	void Start()
	{
		if (this.menuRoot == null)
		{
			this.Setup();
		}
	}

	void ParseResultString(string input)
	{
		string[] words = input.Split(new char[] { ' ' }); // Split input into separate words. Words[0] is the command, every word after is an argument
		
		// Check if command given
		if (words.Length > 0)
		{
			string command = words[0];
			DebugCommandHandler.CommandFunc commandFunc;

			// If command is valid
			if (commandHandler.allCommands.TryGetValue(command, out commandFunc))
			{
				string[] arguments = words.Skip(1).ToArray(); // Get command arguments
				commandFunc(arguments); // Invoke command
			}
			// If command invalid
			else
			{
				OutputText("ERROR: '" + command + "' is not a command!");
			}
		}
	}

	void OnGetTextInput(bool success, string value)
	{
		if (success && !string.IsNullOrEmpty(value))
		{
			this.currentText = value;
			this.ClickedDone(null);
		}
		else
		{
			this.currentText = string.Empty;
			this.ClickedCancel(null);
		}
	}

	void ClickedDone(object ctx)
	{
		this.ParseResultString(this.currentText);
		this.prevComs.Add(this.currentText);
		this.currPrevPos = this.prevComs.Count;
		this.currentText = string.Empty;
		this.UpdateValue(true);
		this.ShowTextInput();
	}

	void ClickedCancel(object ctx)
	{
		this.Hide();
	}

	void ShowTextInput()
	{
		if (this.textListener != null && this.textListener.IsActive)
		{
			this.textListener.Stop();
		}
		this.textListener = TextInput.Instance.GetText("command", string.Empty, new TextInput.OnGotStringFunc(this.OnGetTextInput));
	}

	void MoveCurrCom(int dir)
	{
		if (this.prevComs.Count > 0)
		{
			this.currPrevPos = Mathf.Clamp(this.currPrevPos + dir, 0, this.prevComs.Count - 1);
			this.currentText = this.prevComs[this.currPrevPos];
			this.UpdateValue(true);
		}
	}

	void GotKeyDir(Vector2 dir, bool repeat)
	{
		if (!repeat)
		{
			if (dir.y > 0.5f)
			{
				this.MoveCurrCom(-1);
			}
			else if (dir.y < -0.5f)
			{
				this.MoveCurrCom(1);
			}
		}
	}

	void UpdateValue(bool withCaret)
	{
		GuiContentData guiContentData = new GuiContentData();
		this.hasCaret = false;
		if (withCaret)
		{
			guiContentData.SetValue("currentValue", this.currentText + "|");
			this.hasCaret = true;
		}
		else
		{
			guiContentData.SetValue("currentValue", this.currentText);
		}
		this.menuRoot.ApplyContent(guiContentData, true);
		this.blinkTimer = this._blinkTime;
	}

	public void OutputText(string info)
	{
		GuiContentData guiContentData = new GuiContentData();
		guiContentData.SetValue("currentInfo", info);
		this.menuRoot.ApplyContent(guiContentData, true);
	}

	void GotChar(char c)
	{
		bool flag;
		string a = EnterNameMenu.UpdateInputString(c, this.currentText, out flag, true, false);
		if (a != this.currentText)
		{
			this.currentText = a;
			this.UpdateValue(true);
		}
		if (flag && this.currentText.Length > 0)
		{
			this.ClickedDone(null);
		}
	}

	void Update()
	{
		if (this.ignoreFirstFramesDestroyUnity > 0)
		{
			this.ignoreFirstFramesDestroyUnity--;
			if (this.ignoreFirstFramesDestroyUnity == 0)
			{
				this.charListener = this._input.RegisterCharEvent(new MappedInput.CharEventFunc(this.GotChar), -1);
			}
			return;
		}
		this.blinkTimer -= Time.deltaTime;
		if (this.blinkTimer <= 0f)
		{
			this.UpdateValue(!this.hasCaret);
		}
	}

	public void Show(GuiWindow prev, DebugMenu.OnDoneFunc onDone)
	{
		if (this.menuRoot == null)
		{
			this.Setup();
		}
		this.onDone = onDone;
		this.ignoreFirstFramesDestroyUnity = 2;
		this.currentText = string.Empty;
		this.dirListener = this._input.RegisterMoveDir(new MappedInput.MoveDirEventFunc(this.GotKeyDir), -1);
		this.menuRoot.Show(null, prev);
		this.UpdateValue(true);
		this.blinkTimer = this._blinkTime;
		this.ShowTextInput();
	}

	public void Hide()
	{
		DebugMenu.OnDoneFunc onDoneFunc = this.onDone;
		this.onDone = null;
		this.ClearListeners();
		this.menuRoot.Hide(null);
		if (onDoneFunc != null)
		{
			onDoneFunc();
		}
	}

	void ClearListeners()
	{
		if (this.charListener != null)
		{
			this.charListener.Stop();
			this.charListener = null;
		}
		if (this.dirListener != null)
		{
			this.dirListener.Stop();
			this.dirListener = null;
		}
	}

	void OnDestroy()
	{
		this.ClearListeners();
	}

	public delegate void OnDoneFunc();
}