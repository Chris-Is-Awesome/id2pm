using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ModStuff;

public class DebugMenu : MonoBehaviour
{
	[SerializeField]
	float _blinkTime = 0.5f;

	[SerializeField]
	GameObject _layout;

	[SerializeField]
	MappedInput _input;

	[SerializeField]
	FadeEffectData _fadeData;

	[SerializeField]
	SaverOwner _saver;

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

		// Modify the HUD to make it better
		ModifyUI();
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
			string command = words[0]; // Get command name
			DebugCommandHandler.CommandInfo commandToRun = DebugCommandHandler.Instance.GetCommand(command); // Get command
			
			// If command is valid
			if (commandToRun != null)
			{
				string[] arguments = words.Skip(1).ToArray(); // Get command arguments

				// Run command
				OutputText(DebugManager.LogToConsole(commandToRun.activationMethod(arguments)));

				// Debug output
				string output = "[Debug Console] Running command: '" + commandToRun.nameOfCommand + "' with " + arguments.Length + " argument(s)";
				if (arguments.Length > 0) output += "\n";
				for (int j = 0; j < arguments.Length; j++)
				{
					output += "Arg[" + j + "]: " + arguments[j];
					if (j < arguments.Length - 1) output += "\n";
				}
				DebugManager.LogToFile(output, LogType.Log, false);
			}
			// If command is not valid
			else
			{
				string output = "<in>" + command + "</in> is not a command. Use <out>help</out> to get list of commands";
				OutputText(DebugManager.LogToConsole(output, DebugManager.MessageType.Error));
			}

			// Format output
			TextMesh outputTextMesh = transform.Find("InfoValue").GetChild(0).GetComponent<TextMesh>();
			outputTextMesh.text = ModStuff.UI.UIText.AddLineBreaks(outputTextMesh.text, outputTextMesh);
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

	void ModifyUI()
	{
		// Change title
		Transform titleTrans = transform.Find("Title").Find("Text");
		TextMesh titleTextMesh = null;

		if (titleTrans != null) titleTextMesh = titleTrans.GetComponent<TextMesh>();
		if (titleTextMesh != null)
		{
			titleTextMesh.text = "E2D Debug Menu";
			titleTextMesh.color = Color.black;
			titleTextMesh.alignment = TextAlignment.Center;
			titleTextMesh.fontSize = 35;
			titleTextMesh.fontStyle = FontStyle.Normal;
		}

		// Remove version text
		Transform versionTrans = transform.Find("Version");
		TextMesh versionTextMesh = null;

		if (versionTrans != null) versionTextMesh = versionTrans.GetComponent<TextMesh>();
		if (versionTextMesh != null) versionTextMesh.text = string.Empty;

		// Move back button
		Transform backBtn = transform.Find("Back");
		if (backBtn != null) backBtn.localPosition = new Vector3(-2.5f, -5.10f, 0f);

		// Move cofnrim button
		Transform confirmBtn = transform.Find("Confirm");
		if (confirmBtn != null) confirmBtn.localPosition = new Vector3(2.5f, -5.10f, 0f);

		// Modify output text
		Transform outputText = transform.Find("InfoValue");
		if (outputText != null)
		{
			outputText.localPosition = new Vector3(0.3f, 5.3f, 0f);
			TextMesh outputTextMesh = outputText.GetChild(0).GetComponent<TextMesh>();
			outputTextMesh.fontSize = 20;
			outputTextMesh.alignment = TextAlignment.Left;
			outputTextMesh.anchor = TextAnchor.UpperLeft;
		}

		// Modify input field
		Transform inputField = transform.Find("StringValue");
		if (inputField != null)
		{
			inputField.GetComponentInChildren<NineSlice>().Size = new Vector2(5.5f, 0.65f);
			inputField.localPosition = new Vector3(-0.5f, -3.75f, 0f);
			inputField.Find("Text").localPosition = new Vector3(-4.33f, 0f, -0.18f);
		}

		// Create background for output field
		Transform outputBackgroundTrans = GameObject.Find("PauseOverlay").transform.Find("Pause").Find("Main").Find("Layout").Find("Background");
		Transform menu = transform;
		if (outputBackgroundTrans != null && menu != null)
		{
			Transform outputBackgroundObj = Instantiate(outputBackgroundTrans.gameObject, menu).transform;
			outputBackgroundObj.name = "DebugOutputBackground";
			outputBackgroundObj.GetComponentInChildren<NineSlice>().Size = new Vector2(7.10f, 3.75f);
			outputBackgroundObj.localPosition = new Vector3(-0.5f, 0.9f, 0f);
			outputBackgroundObj.localScale = Vector3.one * 2;
		}
	}

	public delegate void OnDoneFunc();
}