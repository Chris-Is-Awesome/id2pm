using System;
using System.Collections.Generic;
using UnityEngine;
using ModStuff;

public class PauseMenu : EntityOverlayWindow
{
	[SerializeField]
	GameObject _layout;

	[SerializeField]
	bool _layoutIsPrefab;

	[SerializeField]
	OptionsMenu _options;

	[SerializeField]
	CardsMenu _cards;

	[SerializeField]
	bool _optionsIsPrefab = true;

	[SerializeField]
	MapWindow _mapWindow;

	[SerializeField]
	DebugMenu _debugMenu;

	[SerializeField]
	SaverOwner _saver;

	[SerializeField]
	string _quitScene;

	[SerializeField]
	string _defaultHintStr;

	[SerializeField]
	VariableInfoData _itemInfoData;

	[SerializeField]
	FadeEffectData _fadeEffect;

	[SerializeField]
	KeyCode _debugStartCode = KeyCode.LeftControl;

	[SerializeField]
	KeyCode[] _debugStartSequence;

	MenuImpl<PauseMenu> menuImpl;

	OptionsMenu realOpts;

	Entity currEnt;

	ObjectUpdater.PauseTag pauseTag;

	List<KeyCode> debugSequence;

	List<KeyCode> uniqueDebugCodeSet;

	MapWindow mapWindow;

	DebugCommandHandler commandHandler = DebugCommandHandler.Instance;

	OptionsMenu GetOptions()
	{
		if (this.realOpts == null)
		{
			if (this._optionsIsPrefab)
			{
				this.realOpts = GameObjectUtility.TransformInstantiate<OptionsMenu>(this._options, this._layout.transform.parent);
			}
			else
			{
				this.realOpts = this._options;
			}
		}
		return this.realOpts;
	}

	void Setup()
	{
		GuiBindInData inData = new GuiBindInData(null, null);
		PrefabReplacer[] componentsInChildren = this._layout.GetComponentsInChildren<PrefabReplacer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Apply();
		}
		GuiBindData data;
		if (this._layoutIsPrefab)
		{
			data = GuiNode.CreateAndConnect(this._layout, inData);
		}
		else
		{
			data = GuiNode.Connect(this._layout, inData);
		}
		this.menuImpl = new MenuImpl<PauseMenu>(this);
		this.menuImpl.AddScreen(new PauseMenu.MainScreen(this, "pauseRoot", data));
		this.menuImpl.AddScreen(new PauseMenu.OptionsScreen(this, "optionsRoot", data));
		this.menuImpl.AddScreen(new PauseMenu.MapScreen(this, "mapRoot", data));
		this.menuImpl.AddScreen(new PauseMenu.InfoScreen(this, "infoRoot", data));
		this.menuImpl.AddScreen(new PauseMenu.ItemScreen(this, "itemRoot", data));
		this.menuImpl.AddScreen(new PauseMenu.CardsScreen(this, "cardsRoot", data));
		if (this._debugMenu != null)
		{
			this.menuImpl.AddScreen(new PauseMenu.DebugScreen(this, "debugRoot", data));
		}
		PerPlatformData.DebugCodeData debugCode = PlatformInfo.Current.DebugCode;
		if (debugCode != null && debugCode.useOverride)
		{
			this._debugStartCode = debugCode.startCode;
			this._debugStartSequence = debugCode.sequence;
		}
	}

	protected override void Start()
	{
		base.Start();
		if (this.menuImpl == null)
		{
			this.Setup();
		}
	}

	protected override void DoShow(Entity ent, string arg, string arg2)
	{
		if (this.menuImpl == null)
		{
			this.Setup();
		}
		this.currEnt = ent;
		if (arg == "info")
		{
			this.menuImpl.SwitchToScreen("infoRoot", null);
		}
		else if (arg == "map")
		{
			this.menuImpl.SwitchToScreen("mapRoot", null);
		}
		else if (arg == "cards")
		{
			this.menuImpl.SwitchToScreen("cardsRoot", arg2);
		}
		else
		{
			this.menuImpl.ShowFirst();
		}
		this.pauseTag = ObjectUpdater.Instance.RequestPause(null);
	}

	protected override void DoHide()
	{
		if (this.mapWindow != null && this.mapWindow.IsActive)
		{
			this.mapWindow.Hide();
			this.mapWindow = null;
		}
		this.menuImpl.Hide();
		this.menuImpl.Reset();
		if (this.pauseTag != null)
		{
			this.pauseTag.Release();
			this.pauseTag = null;
		}
	}

	protected override void OnDestroy()
	{
		if (this.pauseTag != null)
		{
			this.pauseTag.Release();
		}
		base.OnDestroy();
	}

	void UpdateDebug()
	{
		// Open debug menu with custom key
		if (Input.GetKey(commandHandler.keyToOpenDebugMenu))
		{
			menuImpl.SwitchToScreen("debugRoot", null);
			commandHandler.debugMenu = _debugMenu;
		}
		else if (Input.GetKey(this._debugStartCode))
		{
			if (this.debugSequence == null)
			{
				this.debugSequence = new List<KeyCode>();
			}
			if (this.uniqueDebugCodeSet == null)
			{
				this.uniqueDebugCodeSet = new List<KeyCode>();
				for (int i = this._debugStartSequence.Length - 1; i >= 0; i--)
				{
					if (!this.uniqueDebugCodeSet.Contains(this._debugStartSequence[i]))
					{
						this.uniqueDebugCodeSet.Add(this._debugStartSequence[i]);
					}
				}
			}
			for (int j = this.uniqueDebugCodeSet.Count - 1; j >= 0; j--)
			{
				if (Input.GetKeyDown(this.uniqueDebugCodeSet[j]))
				{
					this.debugSequence.Add(this.uniqueDebugCodeSet[j]);
				}
			}
			while (this.debugSequence.Count > this._debugStartSequence.Length)
			{
				this.debugSequence.RemoveAt(0);
			}
			if (this.debugSequence.Count != this._debugStartSequence.Length)
			{
				return;
			}
			for (int k = 0; k < this.debugSequence.Count; k++)
			{
				if (this.debugSequence[k] != this._debugStartSequence[k])
				{
					return;
				}
			}
			this.debugSequence = null;
			this.menuImpl.SwitchToScreen("debugRoot", null);
		}
		else
		{
			this.debugSequence = null;
		}
	}

	void Update()
	{
		if (this._debugMenu != null)
		{
			this.UpdateDebug();
		}
	}

	class MainScreen : MenuScreen<PauseMenu>
	{
		public MainScreen(PauseMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			MenuScreen<PauseMenu>.BindClickEvent(data, "pause.back", new GuiNode.OnVoidFunc(this.ClickedBack));
			MenuScreen<PauseMenu>.BindClickEvent(data, "pause.quit", new GuiNode.OnVoidFunc(this.ClickedQuit));
			base.BindSwitchClickEvent(data, "pause.hint", "infoRoot");
			base.BindSwitchClickEvent(data, "pause.item", "itemRoot");
			MenuScreen<PauseMenu>.BindClickEvent(data, "pause.warp", new GuiNode.OnVoidFunc(this.ClickedWarp));
			base.BindSwitchClickEvent(data, "pause.options", "optionsRoot");
			base.BindSwitchClickEvent(data, "pause.map", "mapRoot");
		}

		void ClickedBack(object ctx)
		{
			base.Owner.Hide();
		}

		void DoQuit()
		{
			base.Owner.Hide();
			Utility.LoadLevel(base.Owner._quitScene);
		}

		void ClickedQuit(object ctx)
		{
			base.MenuImpl.Hide();
			bool saveDone = false;
			bool fadeDone = base.Owner._fadeEffect == null;
			OverlayFader.OnDoneFunc onDone = delegate()
			{
				if (saveDone && fadeDone)
				{
					this.DoQuit();
				}
			};
			if (base.Owner._fadeEffect != null)
			{
				OverlayFader.StartFade(base.Owner._fadeEffect, true, delegate()
				{
					fadeDone = true;
					onDone();
				}, null);
			}
			base.Owner._saver.SaveAll(true, delegate(bool success, string error)
			{
				saveDone = true;
				onDone();
			});
		}

		void ClickedWarp(object ctx)
		{
			PlayerRespawner activeInstance = PlayerRespawner.GetActiveInstance();
			if (activeInstance != null)
			{
				base.Owner.Hide();
				activeInstance.ForceRespawn();
			}
		}

		protected override bool DoShow(MenuScreen<PauseMenu> previous)
		{
			GuiContentData guiContentData = new GuiContentData();
			guiContentData.SetValue("showMap", base.Owner._mapWindow.CanShow(base.Owner.currEnt));
			bool flag = !PlayerRespawner.RespawnInhibited() && PlayerRespawner.GetActiveInstance() != null;
			guiContentData.SetValue("canWarp", flag);
			if (flag)
			{
				IDataSaver saver = base.Owner._saver.GetSaver("/local/start", true);
				if (saver != null)
				{
					guiContentData.SetValue("hasWarpTgt", true);
					guiContentData.SetValue("warpTgt", saver.LoadData("level"));
				}
				else
				{
					guiContentData.SetValue("hasWarpTgt", false);
				}
			}
			base.Root.ApplyContent(guiContentData, true);
			return true;
		}
	}

	class OptionsScreen : MenuScreen<PauseMenu>
	{
		public OptionsScreen(PauseMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
		}

		protected override bool DoShow(MenuScreen<PauseMenu> previous)
		{
			base.Owner.GetOptions().Show(true, false, new GuiNode.OnVoidFunc(base.StandardBackClick));
			return false;
		}
	}

	class DebugScreen : MenuScreen<PauseMenu>
	{
		public DebugScreen(PauseMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
		}

		protected override bool DoShow(MenuScreen<PauseMenu> previous)
		{
			base.Owner._debugMenu.Show(MenuScreen<PauseMenu>.GetRoot(previous), new DebugMenu.OnDoneFunc(base.SwitchToBack));
			return false;
		}
	}

	class MapScreen : MenuScreen<PauseMenu>
	{
		public MapScreen(PauseMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
		}

		protected override bool DoShow(MenuScreen<PauseMenu> previous)
		{
			if (base.Owner._mapWindow.CanShow(base.Owner.currEnt))
			{
				MapWindow pooledWindow = OverlayWindow.GetPooledWindow<MapWindow>(base.Owner._mapWindow);
				pooledWindow.Show(base.Owner.currEnt, null, new EntityOverlayWindow.OnDoneFunc(this.ClickedBack), null);
				GuiSelectionHandler component = base.Owner.GetComponent<GuiSelectionHandler>();
				if (component != null)
				{
					component.enabled = false;
				}
				base.Owner.mapWindow = pooledWindow;
			}
			else
			{
				Debug.LogWarning("No map for " + Utility.GetCurrentSceneName());
				base.SwitchToBack();
			}
			return false;
		}

		void ClickedBack()
		{
			if (base.Previous == null)
			{
				base.Owner.Hide();
			}
			else
			{
				base.SwitchToBack();
			}
		}

		protected override bool DoHide(MenuScreen<PauseMenu> next)
		{
			GuiSelectionHandler component = base.Owner.GetComponent<GuiSelectionHandler>();
			if (component != null)
			{
				component.enabled = true;
			}
			base.Owner.mapWindow = null;
			return true;
		}
	}

	class InfoScreen : MenuScreen<PauseMenu>
	{
		public InfoScreen(PauseMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			MenuScreen<PauseMenu>.BindClickEvent(data, "info.back", new GuiNode.OnVoidFunc(this.ClickedBack));
		}

		string GetHintString()
		{
			HintSystem weakInstance = HintSystem.WeakInstance;
			if (weakInstance != null)
			{
				HintSystem.HintData currentHint = weakInstance.GetCurrentHint();
				if (currentHint != null)
				{
					return currentHint.String;
				}
			}
			return base.Owner._defaultHintStr;
		}

		protected override bool DoShow(MenuScreen<PauseMenu> previous)
		{
			GuiContentData guiContentData = new GuiContentData();
			guiContentData.SetValue("infoStr", this.GetHintString());
			int stateVariable = base.Owner.currEnt.GetStateVariable("outfit");
			guiContentData.SetValue("playerSuit", stateVariable);
			base.Root.ApplyContent(guiContentData, true);
			return true;
		}

		void ClickedBack(object ctx)
		{
			if (base.Previous == null)
			{
				base.Owner.Hide();
			}
			else
			{
				base.SwitchToBack();
			}
		}
	}

	class ItemScreen : MenuScreen<PauseMenu>
	{
		public ItemScreen(PauseMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
			base.BindBackButton(data, "item.back");
			base.BindSwitchClickEvent(data, "item.cards", "cardsRoot");
		}

		void ClickedItem(object ctx)
		{
			GuiContentData guiContentData = new GuiContentData();
			VariableInfoData.RealValueData realValueData = ctx as VariableInfoData.RealValueData;
			if (realValueData != null)
			{
				guiContentData.SetValue("currItemPic", realValueData.icon);
				guiContentData.SetValue("currItemName", realValueData.name);
				guiContentData.SetValue("currItemDesc", realValueData.desc);
				guiContentData.SetValue("hasItem", true);
				guiContentData.SetValue("currItemHasCount", realValueData.hasCount);
				guiContentData.SetValue("currItemCount", realValueData.count);
			}
			else
			{
				guiContentData.SetValue("hasItem", false);
			}
			base.Root.ApplyContent(guiContentData, true);
		}

		List<GuiContentData> MakeItemList(IExprContext ctx, VariableInfoData info)
		{
			List<GuiContentData> list = new List<GuiContentData>();
			List<VariableInfoData.RealValueData> allData = info.GetAllData(ctx);
			GuiNode.OnVoidFunc f = new GuiNode.OnVoidFunc(this.ClickedItem);
			for (int i = 0; i < allData.Count; i++)
			{
				GuiContentData guiContentData = new GuiContentData();
				VariableInfoData.RealValueData realValueData = allData[i];
				if (realValueData != null && realValueData.icon != null)
				{
					guiContentData.SetValue("itemPic", realValueData.icon);
					guiContentData.SetValue("hasCount", realValueData.hasCount);
					guiContentData.SetValue("itemCount", realValueData.count);
				}
				else
				{
					guiContentData.SetValue("itemPic", false);
					realValueData = null;
				}
				guiContentData.SetValue("itemTag", realValueData);
				guiContentData.SetValue("itemEvent", new GuiNode.VoidBinding(f, realValueData));
				guiContentData.SetValue("itemEnabled", realValueData != null && realValueData.icon != null);
				list.Add(guiContentData);
			}
			return list;
		}

		protected override bool DoShow(MenuScreen<PauseMenu> previous)
		{
			GuiContentData guiContentData = new GuiContentData();
			guiContentData.SetValue("itemList", this.MakeItemList(base.Owner.currEnt, base.Owner._itemInfoData));
			guiContentData.SetValue("hasItem", false);
			guiContentData.SetValue("playerSuit", base.Owner.currEnt.GetStateVariable("outfit"));
			guiContentData.SetValue("hasCards", base.Owner._cards.ShouldShow());
			base.Root.ApplyContent(guiContentData, true);
			return true;
		}

		protected override bool StorePrevious(MenuScreen<PauseMenu> previous)
		{
			return !(previous is PauseMenu.CardsScreen);
		}
	}

	class CardsScreen : MenuScreen<PauseMenu>
	{
		public CardsScreen(PauseMenu owner, string root, GuiBindData data) : base(owner, root, data)
		{
		}

		protected override bool DoShow(MenuScreen<PauseMenu> previous)
		{
			base.Owner._cards.Show(delegate
			{
				this.ClickedBack(null);
			}, base.ShowParams as string);
			return true;
		}

		void ClickedBack(object ctx)
		{
			if (base.Previous == null)
			{
				base.Owner.Hide();
			}
			else
			{
				base.SwitchToBack();
			}
		}
	}
}
