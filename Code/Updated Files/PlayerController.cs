using System;
using System.Collections.Generic;
using UnityEngine;
using ModStuff;

public class PlayerController : ControllerBase, IUpdatable, IPauseListener, IBaseUpdateable
{
	[SerializeField]
	UpdatableLayer _updateLayer;

	[SerializeField]
	Entity _player;

	[SerializeField]
	Camera _mainCam;

	[SerializeField]
	float _touchDeadZone = 0.1f;

	[SerializeField]
	MappedInput _input;

	[SerializeField]
	InputButton _xAxis;

	[SerializeField]
	InputButton _yAxis;

	[SerializeField]
	float _deadZone = 0.35f;

	[SerializeField]
	InputButton _left;

	[SerializeField]
	InputButton _right;

	[SerializeField]
	InputButton _up;

	[SerializeField]
	InputButton _down;

	[SerializeField]
	InputButton _rollButton;

	[SerializeField]
	PlayerController.AttackButtonData[] _attacks;

	List<PlayerController.RelaseAction> releaseActions = new List<PlayerController.RelaseAction>();

	Vector3 lastDir;

	bool isMoving;

	PrioMouseHandler.Tag mouseTag;

	bool isTouched;

	bool forceTouch;

	Vector2 touchdir;

	EntityAction rollAction;

	EntityAction.ActionData savedData;

	List<PlayerController.BtnState> btnSignals = new List<PlayerController.BtnState>();

	protected override UpdatableLayer GetUpdatableLayer()
	{
		return this._updateLayer;
	}

	public override void ControlEntity(Entity ent)
	{
		this._player = ent;
		this.releaseActions = new List<PlayerController.RelaseAction>();
	}

	public override void ReleaseEntity(Entity ent)
	{
		if (this._player == ent)
		{
			this._player = null;
			this.releaseActions.Clear();
		}
	}

	public override bool HasEntity(Entity ent)
	{
		return this._player == ent;
	}

	public void SetEnabled(bool enable)
	{
		base.enabled = enable;
		if (!enable && this.isMoving)
		{
			this.isMoving = false;
			this._player.StopMoving();
		}
	}

	public bool GetKeyForAction(string name, out InputButton key)
	{
		if (name == "roll")
		{
			key = this._rollButton;
			return true;
		}
		for (int i = 0; i < this._attacks.Length; i++)
		{
			if (this._attacks[i].attackName == name)
			{
				key = this._attacks[i].button;
				return true;
			}
		}
		key = null;
		return false;
	}

	void Awake()
	{
		if (this._player == null)
		{
			this._player = base.GetComponent<Entity>();
		}
		if (this._mainCam == null)
		{
			GameObject gameObject = GameObject.Find("Main Camera");
			if (gameObject != null)
			{
				this._mainCam = gameObject.GetComponent<Camera>();
			}
		}
		if (PlatformInfo.Current.AllowMouseInput)
		{
			this.mouseTag = PrioMouseHandler.GetHandler(this._input).GetListener(0, new PrioMouseHandler.MouseDownFunc(this.MouseDown), null, new PrioMouseHandler.MouseUpFunc(this.MouseUp));
		}
	}

	void OnDestroy()
	{
		if (this.mouseTag != null)
		{
			this.mouseTag.Stop();
			this.mouseTag = null;
		}
	}

	static Vector3 ScreenToViewport(Vector3 P)
	{
		P.x /= (float)Screen.width;
		P.y /= (float)Screen.height;
		return P;
	}

	static Vector3 WorldToViewport(Camera cam, Vector3 P)
	{
		return cam.WorldToViewportPoint(P);
	}

	bool MouseDown(int btn, Vector2 P)
	{
		if (!this.isTouched)
		{
			this.isTouched = true;
			return true;
		}
		return false;
	}

	void MouseUp(int btn)
	{
		this.isTouched = false;
	}

	public void SignalMouseDir(Vector2 D)
	{
		this.touchdir = D;
		this.forceTouch = true;
	}

	public void SignalMouseUp()
	{
		this.forceTouch = false;
	}

	bool GetTouchDir(out Vector2 dir)
	{
		if (this.isTouched)
		{
			Vector3 vector = PlayerController.ScreenToViewport(this.mouseTag.Pos);
			Vector3 vector2 = PlayerController.WorldToViewport(this._mainCam, this._player.WorldPosition);
			dir = vector - vector2;
			return true;
		}
		dir = Vector2.zero;
		return false;
	}

	bool GetMoveDir(out Vector2 dir)
	{
		if (this.forceTouch)
		{
			dir = this.touchdir;
			if (dir.sqrMagnitude > 1f)
			{
				dir = dir.normalized;
			}
			this.forceTouch = false;
			return true;
		}
		Vector2 vector;
		if (this.GetTouchDir(out vector))
		{
			Vector2 vector2 = vector;
			vector2.x *= (float)(Screen.width / Screen.height);
			float sqrMagnitude = vector2.sqrMagnitude;
			float num = this._touchDeadZone * this._touchDeadZone;
			if (sqrMagnitude > num && sqrMagnitude < num * 6f)
			{
				dir = vector.normalized;
				return true;
			}
		}
		vector.x = this._input.PressedValue(this._right) - this._input.PressedValue(this._left);
		vector.y = this._input.PressedValue(this._up) - this._input.PressedValue(this._down);
		if (vector.sqrMagnitude > 0.1f)
		{
			dir = vector;
			return true;
		}
		// Decomp fix
		vector = new Vector2(this._input.GetAxisValue(this._xAxis), this._input.GetAxisValue(this._yAxis));
		/*
		vector..ctor(this._input.GetAxisValue(this._xAxis), this._input.GetAxisValue(this._yAxis));
		*/
		if (vector.sqrMagnitude > this._deadZone * this._deadZone)
		{
			if (vector.sqrMagnitude > 1f)
			{
				vector.Normalize();
			}
			dir = vector;
			return true;
		}
		dir = Vector2.zero;
		return false;
	}

	EntityAction.ActionData GetFreeData()
	{
		if (this.savedData == null)
		{
			this.savedData = new EntityAction.ActionData();
		}
		return this.savedData;
	}

	public void SignalPress(InputButton btn)
	{
		for (int i = 0; i < this.btnSignals.Count; i++)
		{
			if (this.btnSignals[i].btn == btn)
			{
				return;
			}
		}
		this.btnSignals.Add(new PlayerController.BtnState(btn));
	}

	public void SignalRelease(InputButton btn)
	{
		for (int i = this.btnSignals.Count - 1; i >= 0; i--)
		{
			if (this.btnSignals[i].btn == btn)
			{
				this.btnSignals.RemoveAt(i);
				return;
			}
		}
	}

	bool CheckButtonDown(InputButton btn)
	{
		if (this.btnSignals.Count > 0)
		{
			for (int i = this.btnSignals.Count - 1; i >= 0; i--)
			{
				PlayerController.BtnState btnState = this.btnSignals[i];
				if (btnState.btn == btn && btnState.down)
				{
					return true;
				}
			}
		}
		return this._input.IsDown(btn);
	}

	bool CheckButtonPressed(InputButton btn)
	{
		if (this.btnSignals.Count > 0)
		{
			for (int i = this.btnSignals.Count - 1; i >= 0; i--)
			{
				if (this.btnSignals[i].btn == btn)
				{
					return true;
				}
			}
		}
		return this._input.IsPressed(btn);
	}

	void UpdateRoll(Vector3 V)
	{
		if (this.CheckButtonDown(this._rollButton) && V.sqrMagnitude > 0.001f)
		{
			EntityAction.ActionData freeData = this.GetFreeData();
			freeData.dir = V.normalized;
			if (this._player.DoAction("roll", freeData))
			{
				this.rollAction = this._player.GetAction("roll");
			}
		}
		else if (this.rollAction != null)
		{
			if (this.rollAction.IsReleasable() && !this.CheckButtonPressed(this._rollButton))
			{
				this.rollAction.ReleaseAction();
			}
			if (this.rollAction.IsActive)
			{
				if (V.sqrMagnitude > 0.001f)
				{
					EntityAction.ActionData freeData2 = this.GetFreeData();
					freeData2.dir = V.normalized;
					this.rollAction.UpdateData(freeData2);
				}
			}
			else
			{
				this.rollAction = null;
			}
		}
	}

	static bool IsRectilinear(Vector3 V)
	{
		return Mathf.Abs(V.x) > 0.9f || Mathf.Abs(V.z) > 0.9f;
	}

	void IUpdatable.UpdateObject()
	{
		if (this._player == null)
		{
			return;
		}
		if (this._player.InactiveOrDead)
		{
			this.ReleaseEntity(this._player);
			return;
		}
		EventListener.PlayerUpdate(); // Invoke custom event
		Vector3 zero = Vector3.zero;
		Vector2 vector;
		if (this.GetMoveDir(out vector))
		{
			// Decomp fix
			zero = new Vector3(vector.x, 0, vector.y);
			/*
			zero..ctor(vector.x, 0f, vector.y);
			*/
			if (vector.magnitude < 0.0625f)
			{
				zero.Normalize();
				if (this.rollAction == null || !this.rollAction.IsActive)
				{
					this._player.TurnTo(zero, 1080f);
				}
				if (this.isMoving)
				{
					this.isMoving = false;
					this._player.StopMoving();
				}
			}
			else
			{
				bool flag = this.isMoving;
				this.isMoving = this._player.SetMoveDirection(zero, true);
				this.UpdateRoll(zero);
				if (!flag || !PlayerController.IsRectilinear(zero) || Vector3.Dot(zero, this.lastDir) < 0.7f)
				{
					this.lastDir = zero;
				}
				else
				{
					this.lastDir = Vector3.RotateTowards(this.lastDir, zero, 12.566371f * Time.deltaTime, 10f * Time.deltaTime);
				}
			}
		}
		else
		{
			this.UpdateRoll(Vector3.zero);
			bool flag2 = this.isMoving;
			this.isMoving = false;
			if (flag2)
			{
				this._player.TurnTo(this.lastDir, 0f);
				this._player.StopMoving();
			}
		}
		for (int i = this.releaseActions.Count - 1; i >= 0; i--)
		{
			PlayerController.RelaseAction relaseAction = this.releaseActions[i];
			if (!this.CheckButtonPressed(relaseAction.button))
			{
				this._player.ReleaseAction(relaseAction.action);
				this.releaseActions.RemoveAt(i);
			}
		}
		for (int j = 0; j < this._attacks.Length; j++)
		{
			if (this.CheckButtonDown(this._attacks[j].button))
			{
				EntityAction.ActionData freeData = this.GetFreeData();
				freeData.dir = zero;
				AttackAction attack = this._player.GetAttack(this._attacks[j].attackName);
				if (attack != null && this._player.Attack(this._attacks[j].attackName, freeData) && attack.IsReleasable())
				{
					this.releaseActions.Add(new PlayerController.RelaseAction(attack, this._attacks[j].button));
				}
			}
		}
		for (int k = this.btnSignals.Count - 1; k >= 0; k--)
		{
			PlayerController.BtnState value = this.btnSignals[k];
			value.down = false;
			this.btnSignals[k] = value;
		}
	}

	void IPauseListener.PauseChanged(bool pause)
	{
		if (pause && this._player != null)
		{
			this._player.StopMoving();
			for (int i = this.releaseActions.Count - 1; i >= 0; i--)
			{
				PlayerController.RelaseAction relaseAction = this.releaseActions[i];
				this._player.ReleaseAction(relaseAction.action);
				this.releaseActions.RemoveAt(i);
			}
		}
	}

	[Serializable]
	public class AttackButtonData
	{
		public string attackName;

		public InputButton button;
	}

	struct RelaseAction
	{
		public EntityAction action;

		public InputButton button;

		public RelaseAction(EntityAction action, InputButton button)
		{
			this.action = action;
			this.button = button;
		}
	}

	struct BtnState
	{
		public InputButton btn;

		public bool down;

		public BtnState(InputButton btn)
		{
			this.btn = btn;
			this.down = true;
		}
	}
}
