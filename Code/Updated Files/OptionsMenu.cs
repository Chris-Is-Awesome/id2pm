using System.Collections.Generic;
using UnityEngine;
using ModStuff;
using ModStuff.UI;

public class OptionsMenu : MonoBehaviour
{
	[SerializeField]
	GameObject _layout;

	[SerializeField]
	bool _layoutIsPrefab;

	[SerializeField]
	SaverOwner _saver;

	[SerializeField]
	KeyConfigMenu _keyConfigMenu;

	GuiNode.OnVoidFunc onDone;

	public MenuImpl<OptionsMenu> menuImpl;

	bool showGameplay;

	bool showSaveFiles;

	bool hasCreatedModOptionsButton; // Added

	void Setup()
	{
		// Build all mod screens
		if (!VarHelper.IsAnticheatActive && SceneAndRoomHelper.GetLoadedScene().name != "MainMenu") UIScreen.BuildAllScreens();

		GuiBindInData inData = new GuiBindInData(null, null);
		GuiBindData data;
		if (this._layoutIsPrefab)
		{
			data = GuiNode.CreateAndConnect(this._layout, inData);
		}
		else
		{
			data = GuiNode.Connect(this._layout, inData);
		}
		this.menuImpl = new MenuImpl<OptionsMenu>(this);
		this.menuImpl.AddScreen(new OptionsMenu.MainScreen(this, "optionsRoot", data));
		this.menuImpl.AddScreen(new OptionsMenu.AudioScreen(this, "audioRoot", data));
		this.menuImpl.AddScreen(new OptionsMenu.VideoScreen(this, "videoRoot", data));
		this.menuImpl.AddScreen(new OptionsMenu.ResolutionScreen(this, "screenResRoot", data));
		this.menuImpl.AddScreen(new OptionsMenu.KeyConfigScreen(this, "keyConfigRoot", data));
		this.menuImpl.AddScreen(new OptionsMenu.QualityScreen(this, "qualityRoot", data));
		this.menuImpl.AddScreen(new OptionsMenu.GameplayScreen(this, "gameplayRoot", data));

		if (!VarHelper.IsAnticheatActive && SceneAndRoomHelper.GetLoadedScene().name != "MainMenu")
		{
			// Create mod menus
			menuImpl.AddScreen(new OptionsMenu.ModOptions(this, "modoptions", data));
		}
	}

	void Start()
	{
		if (this.menuImpl == null)
		{
			this.Setup();
		}
	}

	public void Show(bool showGameplay, bool showSaveFiles, GuiNode.OnVoidFunc onDone)
	{
		if (this.menuImpl == null)
		{
			this.Setup();
		}
		this.showGameplay = showGameplay;
		this.showSaveFiles = showSaveFiles;
		this.onDone = onDone;
		this.menuImpl.ShowFirst();
	}

	void Back(object ctx)
	{
		this.menuImpl.Hide();
		this.menuImpl.Reset();
		GuiNode.OnVoidFunc onVoidFunc = this.onDone;
		this.onDone = null;
		if (onVoidFunc != null)
		{
			onVoidFunc(ctx);
		}
	}

	// Added
	class ModOptions : MenuScreen<OptionsMenu>
	{
		UIScreen screen;

		public ModOptions(OptionsMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			screen = UIScreen.GetUIScreenComponent(Root);
			screen.BackButton.onInteraction += ClickedBack;
		}

		protected override bool DoShow(MenuScreen<OptionsMenu> previous)
		{
			// Add UI default values here
			screen.GetElement<UICheckBox>("runInBackground").Value = ModStuff.ModOptions.LoadOption("runInBackground");
			screen.GetElement<UICheckBox>("debugOverlay").Value = ModStuff.ModOptions.LoadOption("debugOverlay");

			return true;
		}

		void ClickedBack()
		{
			if (Previous == null)
			{
				Owner.menuImpl.Hide();
				return;
			}
			SwitchToBack();
		}
	}

	class MainScreen : MenuScreen<OptionsMenu>
	{
		public MainScreen(OptionsMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			MenuScreen<OptionsMenu>.BindClickEvent(data, "opt.btnBack", new GuiNode.OnVoidFunc(this.ClickedBack));
			base.BindSwitchClickEvent(data, "opt.btnGameplay", "gameplayRoot");
			base.BindSwitchClickEvent(data, "opt.btnConfig", "keyConfigRoot");
			base.BindSwitchClickEvent(data, "opt.btnVideo", "videoRoot");
			base.BindSwitchClickEvent(data, "opt.btnAudio", "audioRoot");
			//base.BindSwitchClickEvent(data, "key.btnDefault", "modoptions");
			//base.BindSwitchClickEvent(data, "Button4", "modoptions");
		}

		void ClickedBack(object ctx)
		{
			base.Owner.Back(ctx);
		}

		protected override bool DoShow(MenuScreen<OptionsMenu> previous)
		{
			GuiContentData guiContentData = new GuiContentData();
			guiContentData.SetValue("showGameplay", base.Owner.showGameplay);
			guiContentData.SetValue("showVideo", !base.Owner.showGameplay);
			guiContentData.SetValue("showSaveFiles", base.Owner.showSaveFiles);
			MainMenu.ApplyUIData(PlatformInfo.Current.GetDataForUI("options.main"), guiContentData);
			base.Root.ApplyContent(guiContentData, true);

			if (!VarHelper.IsAnticheatActive && !Owner.hasCreatedModOptionsButton)
			{
				Transform menu = Owner.transform;
				Transform layout = menu.transform.GetChild(1).GetChild(0);
				UIButton modOptionsButton = UIFactory.Instance.CreateButton(UIButton.ButtonType.Default, 0f, -5.5f, layout, "Mod Options");
				modOptionsButton.ScaleBackground(new Vector2(1.05f, 1.05f));
				modOptionsButton.NameTextMesh.fontSize = 40;
				modOptionsButton.onInteraction += delegate ()
				{
					StandardSwitch(null, "modoptions");
				};
				Owner.hasCreatedModOptionsButton = true;
			}

			return true;
		}
	}

	class AudioScreen : MenuScreen<OptionsMenu>
	{
		bool changed;

		public AudioScreen(OptionsMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			base.BindBackButton(data, "audio.btnBack");
			MenuScreen<OptionsMenu>.BindFloatEvent(data, "audio.sndLevel", new GuiNode.OnFloatFunc(this.ChangedSound));
			MenuScreen<OptionsMenu>.BindFloatEvent(data, "audio.musicLevel", new GuiNode.OnFloatFunc(this.ChangedMusic));
		}

		void ChangedSound(float value, object ctx)
		{
			SoundPlayer.Instance.SoundVolume = value;
			this.changed = true;
		}

		void ChangedMusic(float value, object ctx)
		{
			MusicPlayer.Instance.MusicVolume = value;
			this.changed = true;
		}

		void ChannelChange(float value, object ctx, string channel)
		{
			SoundPlayer.Instance.SetChannelVolume(channel, value);
			this.changed = true;
		}

		protected override bool DoShow(MenuScreen<OptionsMenu> previous)
		{
			GuiContentData guiContentData = new GuiContentData();
			SoundPlayer instance = SoundPlayer.Instance;
			guiContentData.SetValue("sndLevel", instance.SoundVolume);
			guiContentData.SetValue("musicLevel", MusicPlayer.Instance.MusicVolume);
			List<GuiContentData> list = new List<GuiContentData>();
			string[] allChannelNames = instance.GetAllChannelNames();
			for (int i = 0; i < allChannelNames.Length; i++)
			{
				string ch = allChannelNames[i];
				GuiContentData guiContentData2 = new GuiContentData();
				guiContentData2.SetValue("channelLevel", instance.GetChannelVolume(ch));
				guiContentData2.SetValue("channelName", "AudioChannel_" + ch);
				guiContentData2.SetValue("channelChangeEvent", new GuiNode.FloatBinding(delegate(float f, object o)
				{
					this.ChannelChange(f, o, ch);
				}, null));
				list.Add(guiContentData2);
			}
			guiContentData.SetValue("channelList", list);
			base.Root.ApplyContent(guiContentData, true);
			this.changed = false;
			return true;
		}

		protected override bool DoHide(MenuScreen<OptionsMenu> next)
		{
			if (this.changed)
			{
				IDataSaver globalStorage = base.Owner._saver.GlobalStorage;
				IDataSaver localSaver = globalStorage.GetLocalSaver("sound");
				localSaver.SaveFloat("musicVol", MusicPlayer.Instance.MusicVolume);
				SoundPlayer.Instance.Save(localSaver);
				base.Owner._saver.SaveGlobal();
			}
			return true;
		}
	}

	class VideoScreen : MenuScreen<OptionsMenu>
	{
		public VideoScreen(OptionsMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			base.BindBackButton(data, "video.back");
			base.BindSwitchClickEvent(data, "video.res", "screenResRoot");
			base.BindSwitchClickEvent(data, "video.quality", "qualityRoot");
		}

		protected override bool StorePrevious(MenuScreen<OptionsMenu> previous)
		{
			return previous is OptionsMenu.MainScreen;
		}

		protected override bool DoShow(MenuScreen<OptionsMenu> previous)
		{
			GuiContentData guiContentData = new GuiContentData();
			MainMenu.ApplyUIData(PlatformInfo.Current.GetDataForUI("options.video"), guiContentData);
			base.Root.ApplyContent(guiContentData, true);
			return true;
		}
	}

	class ResolutionScreen : MenuScreen<OptionsMenu>
	{
		Resolution wantedRes;

		bool wantedFull;

		bool changed;

		public ResolutionScreen(OptionsMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			base.BindBackButton(data, "res.back");
			MenuScreen<OptionsMenu>.BindClickEvent(data, "res.confirm", new GuiNode.OnVoidFunc(this.ClickedConfirm));
			MenuScreen<OptionsMenu>.BindBoolEvent(data, "res.fullscreen", new GuiNode.OnBoolFunc(this.ChangedFull));
		}

		static bool ResEqual(Resolution a, Resolution b)
		{
			return a.width == b.width && a.height == b.height;
		}

		static string ResToString(Resolution res)
		{
			return string.Concat(new string[]
			{
				res.width.ToString(),
				"x",
				res.height.ToString(),
				" @ ",
				res.refreshRate.ToString()
			});
		}

		void ChangedRes(object ctx)
		{
			this.wantedRes = (Resolution)ctx;
			this.changed = true;
		}

		void ChangedFull(bool value, object ctx)
		{
			this.wantedFull = value;
			this.changed = true;
		}

		void ClickedConfirm(object ctx)
		{
			if (this.changed)
			{
				Screen.SetResolution(this.wantedRes.width, this.wantedRes.height, this.wantedFull, this.wantedRes.refreshRate);
			}
			base.SwitchToBack();
		}

		static Resolution MakeRes(int width, int height, int refresh)
		{
			Resolution result = default(Resolution);
			result.width = width;
			result.height = height;
			result.refreshRate = refresh;
			return result;
		}

		static Resolution GetCurrentRes()
		{
			if (Screen.fullScreen)
			{
				return Screen.currentResolution;
			}
			return OptionsMenu.ResolutionScreen.MakeRes(Screen.width, Screen.height, 60);
		}

		static Resolution[] GetResolutions()
		{
			return Screen.resolutions;
		}

		List<GuiContentData> MakeResList(Resolution[] res)
		{
			List<GuiContentData> list = new List<GuiContentData>(res.Length);
			GuiNode.OnVoidFunc f = new GuiNode.OnVoidFunc(this.ChangedRes);
			for (int i = 0; i < res.Length; i++)
			{
				GuiContentData guiContentData = new GuiContentData();
				guiContentData.SetValue("resSelect", new GuiNode.VoidBinding(f, res[i]));
				guiContentData.SetValue("resName", OptionsMenu.ResolutionScreen.ResToString(res[i]));
				list.Add(guiContentData);
			}
			return list;
		}

		protected override bool DoShow(MenuScreen<OptionsMenu> previous)
		{
			GuiContentData guiContentData = new GuiContentData();
			Resolution[] resolutions = OptionsMenu.ResolutionScreen.GetResolutions();
			guiContentData.SetValue("screenResList", this.MakeResList(resolutions));
			int num = 0;
			Resolution currentRes = OptionsMenu.ResolutionScreen.GetCurrentRes();
			for (int i = resolutions.Length - 1; i >= 0; i--)
			{
				Resolution a = resolutions[i];
				if (OptionsMenu.ResolutionScreen.ResEqual(a, currentRes))
				{
					num = i;
					break;
				}
			}
			guiContentData.SetValue("screenRes", (float)num / (float)(resolutions.Length - 1));
			guiContentData.SetValue("resName", OptionsMenu.ResolutionScreen.ResToString(resolutions[num]));
			guiContentData.SetValue("fullscreen", Screen.fullScreen);
			this.wantedRes.width = Screen.width;
			this.wantedRes.height = Screen.height;
			this.wantedFull = Screen.fullScreen;
			this.changed = false;
			base.Root.ApplyContent(guiContentData, true);
			return true;
		}
	}

	class QualityScreen : MenuScreen<OptionsMenu>
	{
		int wantedLevel;

		bool changed;

		public QualityScreen(OptionsMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			base.BindBackButton(data, "quality.back");
			MenuScreen<OptionsMenu>.BindClickEvent(data, "quality.confirm", new GuiNode.OnVoidFunc(this.ClickedConfirm));
		}

		void ClickedConfirm(object ctx)
		{
			if (this.changed)
			{
				QualitySettings.SetQualityLevel(this.wantedLevel, true);
			}
			base.SwitchToBack();
		}

		void ClickedSetting(object ctx)
		{
			this.wantedLevel = (int)ctx;
			this.changed = true;
		}

		List<GuiContentData> MakeQualityList(string[] names)
		{
			GuiNode.OnVoidFunc f = new GuiNode.OnVoidFunc(this.ClickedSetting);
			List<GuiContentData> list = new List<GuiContentData>(names.Length);
			for (int i = 0; i < names.Length; i++)
			{
				GuiContentData guiContentData = new GuiContentData();
				guiContentData.SetValue("qualitySelect", new GuiNode.VoidBinding(f, i));
				guiContentData.SetValue("qualityName", names[i]);
				list.Add(guiContentData);
			}
			return list;
		}

		protected override bool DoShow(MenuScreen<OptionsMenu> previous)
		{
			GuiContentData guiContentData = new GuiContentData();
			guiContentData.SetValue("qualityList", this.MakeQualityList(QualitySettings.names));
			int qualityLevel = QualitySettings.GetQualityLevel();
			guiContentData.SetValue("videoQuality", (float)qualityLevel / (float)(QualitySettings.names.Length - 1));
			guiContentData.SetValue("qualityName", QualitySettings.names[qualityLevel]);
			this.changed = false;
			base.Root.ApplyContent(guiContentData, true);
			return true;
		}
	}

	class KeyConfigScreen : MenuScreen<OptionsMenu>
	{
		public KeyConfigScreen(OptionsMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
		}

		protected override bool DoShow(MenuScreen<OptionsMenu> previous)
		{
			base.Owner._keyConfigMenu.Show(MenuScreen<OptionsMenu>.GetRoot(previous), new GuiNode.OnVoidFunc(base.StandardBackClick));
			return false;
		}
	}

	class GameplayScreen : MenuScreen<OptionsMenu>
	{
		bool changed;

		bool cutscenes;

		bool mapHint;

		bool showTime;

		bool easyMode;

		public GameplayScreen(OptionsMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			base.BindBackButton(data, "game.back");
			MenuScreen<OptionsMenu>.BindBoolEvent(data, "game.cutscenes", new GuiNode.OnBoolFunc(this.ChangedCutscene));
			MenuScreen<OptionsMenu>.BindBoolEvent(data, "game.mapHint", new GuiNode.OnBoolFunc(this.ChangedMapHint));
			MenuScreen<OptionsMenu>.BindBoolEvent(data, "game.showTime", new GuiNode.OnBoolFunc(this.ChangedTime));
			MenuScreen<OptionsMenu>.BindBoolEvent(data, "game.easyMode", new GuiNode.OnBoolFunc(this.ChangedEasyMode));
		}

		void ChangedCutscene(bool value, object ctx)
		{
			this.cutscenes = value;
			this.changed = true;
		}

		void ChangedMapHint(bool value, object ctx)
		{
			this.mapHint = value;
			this.changed = true;
		}

		void ChangedTime(bool value, object ctx)
		{
			this.showTime = value;
			this.changed = true;
		}

		void ChangedEasyMode(bool value, object ctx)
		{
			this.easyMode = value;
			this.changed = true;
		}

		protected override bool DoShow(MenuScreen<OptionsMenu> previous)
		{
			this.changed = false;
			IDataSaver saverByPath = SaverOwner.GetSaverByPath("settings", base.Owner._saver.LocalStorage, true);
			if (saverByPath != null)
			{
				this.cutscenes = !SaverOwner.LoadSaveData("hideCutscenes", saverByPath, false);
				this.mapHint = !SaverOwner.LoadSaveData("hideMapHint", saverByPath, false);
				this.showTime = SaverOwner.LoadSaveData("showTime", saverByPath, false);
				this.easyMode = SaverOwner.LoadSaveData("easyMode", saverByPath, false);
			}
			else
			{
				this.cutscenes = true;
				this.mapHint = true;
				this.showTime = false;
				this.easyMode = false;
			}
			GuiContentData guiContentData = new GuiContentData();
			guiContentData.SetValue("showCutscenes", this.cutscenes);
			guiContentData.SetValue("showMapHint", this.mapHint);
			guiContentData.SetValue("showTime", this.showTime);
			guiContentData.SetValue("easyMode", this.easyMode);
			base.Root.ApplyContent(guiContentData, true);
			return true;
		}

		protected override bool DoHide(MenuScreen<OptionsMenu> next)
		{
			if (this.changed)
			{
				IDataSaver saverByPath = SaverOwner.GetSaverByPath("settings", base.Owner._saver.LocalStorage, false);
				saverByPath.SaveBool("hideCutscenes", !this.cutscenes);
				saverByPath.SaveBool("hideMapHint", !this.mapHint);
				saverByPath.SaveBool("showTime", this.showTime);
				saverByPath.SaveBool("easyMode", this.easyMode);
				base.Owner._saver.SaveLocal(false, false);
				this.EasyMode(this.easyMode);
			}
			return true;
		}

		void EasyMode(bool easy)
		{
			Entity entity = EntityTag.GetEntityByName("PlayerEnt");
			if (entity == null)
			{
				GameObject gameObject = GameObject.Find("PlayerEnt");
				if (gameObject != null)
				{
					entity = gameObject.GetComponent<Entity>();
				}
			}
			int value = (!easy) ? 0 : 1;
			entity.SetStateVariable("easyMode", value);
		}
	}
}
