using System;
using System.Diagnostics;
using UnityEngine;
using ModStuff;

public class EntityEventsOwner
{
	public event EntityEventsOwner.OnEventFunc DeathListener;
	public event EntityEventsOwner.OnDetailedDeathFunc DetailedDeathListener;
	public event EntityEventsOwner.OnEventFunc DeactivateListener;
	public event EntityEventsOwner.OnDamagedFunc DamageListener;
	public event EntityEventsOwner.OnRoomChangeEvent RoomChangeListener;
	public event EntityEventsOwner.OnRoomChangeEvent RoomChangeDoneListener;
	public event EntityEventsOwner.OnItemFunc ItemGetListener;
	public event EntityEventsOwner.OnVariableFunc VarSetListener;

	public void SendDied(Entity ent)
	{
		if (this.DeathListener != null)
		{
			this.DeathListener(ent);
			EventListener.EntityDeath(ent); // Invoke custom event
		}
	}

	public void SendDetailedDeath(Entity ent, Killable.DetailedDeathData data)
	{
		if (this.DetailedDeathListener != null)
		{
			this.DetailedDeathListener(ent, data);
		}
	}

	public void SendDeactivate(Entity ent)
	{
		if (this.DeactivateListener != null)
		{
			this.DeactivateListener(ent);
			EventListener.EntitySpawn(ent, false); // Invoke custom event
		}
	}

	public void SendDamaged(Entity ent, HitData data)
	{
		EventListener.DamageDone(ent, data); // Invoke custom event
		if (this.DamageListener != null)
		{
			this.DamageListener(ent, data);
		}
	}

	public void SendRoomChange(Entity ent, LevelRoom to, LevelRoom from, EntityEventsOwner.RoomEventData data)
	{
		if (this.RoomChangeListener != null)
		{
			this.RoomChangeListener(ent, to, from, data);
		}
	}

	public void SendRoomChangeDone(Entity ent, LevelRoom to, LevelRoom from, EntityEventsOwner.RoomEventData data)
	{
		if (this.RoomChangeDoneListener != null)
		{
			this.RoomChangeDoneListener(ent, to, from, data);
		}
	}

	public void SendItemGet(Entity ent, Item item)
	{
		if (this.ItemGetListener != null)
		{
			this.ItemGetListener(ent, item);
			EventListener.ItemGet(ent, item); // Invoke custom event
		}
	}

	public void SendVarSet(Entity ent, string var, int value)
	{
		if (this.VarSetListener != null)
		{
			this.VarSetListener(ent, var, value);
			EventListener.EntVarSave(ent, var, value); // Invoke custom event
		}
	}

	public struct RoomEventData
	{
		public Vector3 targetPos;

		public Vector3 targetDir;

		public RoomEventData(Vector3 targetPos, Vector3 targetDir)
		{
			this.targetPos = targetPos;
			this.targetDir = targetDir;
		}
	}

	public delegate void OnEventFunc(Entity ent);

	public delegate void OnDetailedDeathFunc(Entity ent, Killable.DetailedDeathData data);

	public delegate void OnDamagedFunc(Entity ent, HitData hitData);

	public delegate void OnRoomChangeEvent(Entity ent, LevelRoom to, LevelRoom from, EntityEventsOwner.RoomEventData data);

	public delegate void OnItemFunc(Entity ent, Item item);

	public delegate void OnVariableFunc(Entity ent, string var, int value);
}
