using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using ModStuff;

[AddComponentMenu("Ittle 2/Level/Level Room")]
[ExecuteInEditMode]
public class LevelRoom : MonoBehaviour
{
	static List<LevelRoom> currentRooms = new List<LevelRoom>();

	[SerializeField]
	LevelRoot _levelRoot;

	[SerializeField]
	Bounds _bounds = default(Bounds);

	[SerializeField]
	bool _lockBounds;

	[SerializeField]
	bool _lockEdit;

	[SerializeField]
	GeneralInterpolator _interpolator;

	[SerializeField]
	bool _doTransition = true;

	[SerializeField]
	bool _isDummyRoom;

	TileData[] savedTiles;

	bool isActive;

	bool isInited;

	bool hasStarted;

	bool deactivateOnStart;

	Vector3 importantPoint;

	public static LevelRoom GetRoomForPosition(Vector3 pos, List<LevelRoom> rooms = null)
	{
		if (rooms == null)
		{
			rooms = LevelRoom.currentRooms;
		}
		for (int i = 0; i < rooms.Count; i++)
		{
			LevelRoom levelRoom = rooms[i];
			if (levelRoom.Bounds.Contains(pos))
			{
				return levelRoom;
			}
		}
		return null;
	}

	public static void SetCurrentActiveRoom(LevelRoom room, bool forceReset = false)
	{
		for (int i = 0; i < LevelRoom.currentRooms.Count; i++)
		{
			LevelRoom levelRoom = LevelRoom.currentRooms[i];
			if (levelRoom.IsDummy)
			{
				if (!levelRoom.IsActive)
				{
					levelRoom.SetRoomActive(true, false);
				}
			}
			else if (levelRoom != room || forceReset)
			{
				levelRoom.SetRoomActive(false, true);
			}
		}
		if (room != null)
		{
			room.SetRoomActive(true, true);
		}
	}

	public event LevelRoom.OnEventFunc OnInited;
	public event LevelRoom.OnEventFunc OnActivated;
	public event LevelRoom.OnEventFunc OnDeactivated;
	public event LevelRoom.OnEventFunc OnDeactivateDone;

	public Vector3 ImportantPoint
	{
		get
		{
			return this.importantPoint;
		}
	}

	public void SetImportantPoint(Vector3 P)
	{
		this.importantPoint = P;
	}

	public bool IsInited
	{
		get
		{
			return this.isInited;
		}
	}

	public bool IsDummy
	{
		get
		{
			return this._isDummyRoom;
		}
	}

	void Awake()
	{
		if (Application.isPlaying)
		{
			LevelRoom.currentRooms.Add(this);
			this.importantPoint = base.transform.position;
		}
	}

	void OnDestroy()
	{
		LevelRoom.currentRooms.Remove(this);
	}

	void Start()
	{
		if (Application.isPlaying)
		{
			this.hasStarted = true;
			if (this.deactivateOnStart)
			{
				this.SetRoomActive(false, true);
			}
		}
	}

	public bool IsActive
	{
		get
		{
			return this.isActive;
		}
	}

	public bool LockEdit
	{
		get
		{
			return this._lockEdit;
		}
		set
		{
			this._lockEdit = value;
		}
	}

	public string RoomName
	{
		get
		{
			return base.name;
		}
	}

	public LevelRoot LevelRoot
	{
		get
		{
			if (this._levelRoot == null)
			{
				this._levelRoot = TransformUtility.FindInParents<LevelRoot>(base.transform);
			}
			return this._levelRoot;
		}
	}

	public IDataSaver RoomStorage
	{
		get
		{
			return this.LevelRoot.LevelStorage.GetLocalSaver(this.RoomName);
		}
	}

	public void SetInterpolator(ColorInterpolator p)
	{
		this._interpolator = p;
	}

	public void StartTransition(bool hide)
	{
		if (!hide)
		{
			this.SetRoomActive(true, !this._doTransition);
		}
		if (this._doTransition)
		{
			base.gameObject.SetActive(true);
			if (this._interpolator != null)
			{
				this._interpolator.StartInterpolation(!hide);
			}
		}
	}

	public void UpdateTransition(float t)
	{
		if (this._doTransition && this._interpolator != null)
		{
			this._interpolator.Interpolate(t);
		}
	}

	public void FinishTransition(bool hide)
	{
		if (this._doTransition && this._interpolator != null)
		{
			this._interpolator.FinishInterpolation(!hide, this._doTransition);
		}
		if (hide)
		{
			this.SetRoomActive(false, !this._doTransition);
		}
	}

	void SetHierActive(bool active)
	{
		if (this._doTransition && base.gameObject.activeInHierarchy != active)
		{
			base.gameObject.SetActive(active);
		}
	}

	void SetRoomActive(bool active, bool withTransition)
	{
		if (!this.hasStarted && !active)
		{
			this.deactivateOnStart = true;
			return;
		}
		if (!this.isInited)
		{
			this.isInited = true;
			try
			{
				if (this.OnInited != null)
				{
					this.OnInited();
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("Error in room init callback: " + ex.Message + "\n" + ex.StackTrace);
			}
		}
		bool flag = this.isActive;
		this.isActive = active;
		if (active)
		{
			this.SetHierActive(active);
			if (this._interpolator != null && withTransition)
			{
				this._interpolator.FinishInterpolation(true, this._doTransition);
			}
			try
			{
				if (!flag && this.OnActivated != null)
				{
					this.OnActivated();
					EventListener.RoomLoad(this, true); // Invoke custom event
				}
			}
			catch (Exception ex2)
			{
				UnityEngine.Debug.LogError("Error in room activation callback: " + ex2.Message + "\n" + ex2.StackTrace);
			}
		}
		else if (flag)
		{
			try
			{
				if (this.OnDeactivated != null)
				{
					this.OnDeactivated();
				}
			}
			catch (Exception ex3)
			{
				UnityEngine.Debug.LogError("Error in room deactivation callback: " + ex3.Message + "\n" + ex3.StackTrace);
			}
			try
			{
				if (this.OnDeactivateDone != null)
				{
					this.OnDeactivateDone();
					EventListener.RoomLoad(this, false); // Invoke custom event
				}
			}
			catch (Exception ex4)
			{
				UnityEngine.Debug.LogError("Error in final room deactivation callback: " + ex4.Message + "\n" + ex4.StackTrace);
			}
			this.SetHierActive(active);
		}
		else
		{
			this.SetHierActive(active);
		}
	}

	public TileData[] GetTiles(string layer = null)
	{
		if (!string.IsNullOrEmpty(layer))
		{
			Transform transform = base.transform.Find(layer);
			if (transform != null)
			{
				return transform.GetComponentsInChildren<TileData>();
			}
		}
		if (this.savedTiles == null || this.savedTiles.Length == 0)
		{
			this.savedTiles = base.GetComponentsInChildren<TileData>();
		}
		return this.savedTiles;
	}

	public void MarkAsChanged()
	{
		this.savedTiles = null;
	}

	public void UpdateBounds(Vector3 tilePos, float tileSize)
	{
		if (!this._lockBounds)
		{
			// Decomp fix
			Bounds bounds = new Bounds(tilePos, Vector3.one * (tileSize * 0.5f));
			/*
			Bounds bounds;
			bounds..ctor(tilePos, Vector3.one * (tileSize * 0.5f));
			*/
			this._bounds.Encapsulate(bounds);
		}
	}

	public void RecalculateBounds(Vector3 padding)
	{
		TileData[] tiles = this.GetTiles(null);
		if (tiles.Length > 0)
		{
			// Decomp fix
			Bounds bounds = new Bounds(tiles[0].transform.position, Vector3.one);
			/*
			Bounds bounds;
			bounds..ctor(tiles[0].transform.position, Vector3.one);
			*/
			foreach (TileData tileData in tiles)
			{
				if (!(tileData == null))
				{
					Vector3 position = tileData.transform.position;
					// Decomp fix
					Bounds bounds2 = new Bounds(position, Vector3.zero);
					/*
					Bounds bounds2;
					bounds2..ctor(position, Vector3.zero);
					*/
					Renderer[] componentsInChildren = tileData.GetComponentsInChildren<Renderer>(true);
					for (int j = componentsInChildren.Length - 1; j >= 0; j--)
					{
						bounds2.Encapsulate(componentsInChildren[j].bounds);
					}
					bounds.Encapsulate(bounds2);
				}
			}
			bounds.size += padding;
			bounds.center -= base.transform.position;
			this._bounds = bounds;
		}
	}

	public TileData GetTileInRange(Vector3 P, float range, string layer = null, float yScale = 1f, Type ignoreType = null)
	{
		float num = range * range;
		TileData[] tiles = this.GetTiles(layer);
		for (int i = tiles.Length - 1; i >= 0; i--)
		{
			if (tiles[i] != null)
			{
				if (ignoreType == null || !(tiles[i].GetComponent(ignoreType) != null))
				{
					Vector3 vector = P - tiles[i].transform.position;
					vector.y *= yScale;
					if (vector.sqrMagnitude < num)
					{
						return tiles[i];
					}
				}
			}
		}
		return null;
	}

	public Transform GetTileLayer(string layer)
	{
		Transform transform = base.transform;
		if (!string.IsNullOrEmpty(layer))
		{
			transform = base.transform.Find(layer);
			if (transform == null)
			{
				GameObject gameObject = new GameObject(layer);
				transform = gameObject.transform;
				transform.parent = base.transform;
				transform.localPosition = Vector3.zero;
				transform.localScale = Vector3.one;
				transform.localRotation = Quaternion.identity;
			}
		}
		return transform;
	}

	public Transform GetTileParent(string group = null, string layer = null)
	{
		Transform transform = this.GetTileLayer(layer);
		if (!string.IsNullOrEmpty(group))
		{
			Transform transform2 = transform.Find(group);
			if (transform2 == null)
			{
				GameObject gameObject = new GameObject(group);
				transform2 = gameObject.transform;
				transform2.parent = transform;
				transform2.localPosition = Vector3.zero;
				transform2.localScale = Vector3.one;
				transform2.localRotation = Quaternion.identity;
			}
			transform = transform2;
		}
		return transform;
	}

	public Bounds Bounds
	{
		get
		{
			Bounds bounds = this._bounds;
			bounds.center += base.transform.position;
			return bounds;
		}
	}

	public delegate void OnEventFunc();
}
