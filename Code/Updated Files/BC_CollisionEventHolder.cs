using System;
using System.Collections.Generic;
using ModStuff;

public class BC_CollisionEventHolder
{
	List<IBC_CollisionEnterListener> colEnters;

	List<IBC_CollisionStayListener> colStays;

	List<IBC_CollisionExitListener> colExits;

	List<IBC_TriggerEnterListener> trigEnters;

	List<IBC_TriggerStayListener> trigStays;

	List<IBC_TriggerExitListener> trigExits;

	static List<T> Register<T>(IBC_CollisionEventListener obj, List<T> list) where T : IBC_CollisionEventListener
	{
		if (obj is T)
		{
			if (list == null)
			{
				list = new List<T>();
			}
			list.Add((T)((object)obj));
		}
		return list;
	}

	static bool Unregister<T>(IBC_CollisionEventListener obj, List<T> list) where T : IBC_CollisionEventListener
	{
		return obj is T && list != null && list.Remove((T)((object)obj));
	}

	public void RegisterListener(IBC_CollisionEventListener obj)
	{
		this.colEnters = BC_CollisionEventHolder.Register<IBC_CollisionEnterListener>(obj, this.colEnters);
		this.colStays = BC_CollisionEventHolder.Register<IBC_CollisionStayListener>(obj, this.colStays);
		this.colExits = BC_CollisionEventHolder.Register<IBC_CollisionExitListener>(obj, this.colExits);
		this.trigEnters = BC_CollisionEventHolder.Register<IBC_TriggerEnterListener>(obj, this.trigEnters);
		this.trigStays = BC_CollisionEventHolder.Register<IBC_TriggerStayListener>(obj, this.trigStays);
		this.trigExits = BC_CollisionEventHolder.Register<IBC_TriggerExitListener>(obj, this.trigExits);
	}

	public void UnregisterListener(IBC_CollisionEventListener obj)
	{
		BC_CollisionEventHolder.Unregister<IBC_CollisionEnterListener>(obj, this.colEnters);
		BC_CollisionEventHolder.Unregister<IBC_CollisionStayListener>(obj, this.colStays);
		BC_CollisionEventHolder.Unregister<IBC_CollisionExitListener>(obj, this.colExits);
		BC_CollisionEventHolder.Unregister<IBC_TriggerEnterListener>(obj, this.trigEnters);
		BC_CollisionEventHolder.Unregister<IBC_TriggerStayListener>(obj, this.trigStays);
		BC_CollisionEventHolder.Unregister<IBC_TriggerExitListener>(obj, this.trigExits);
	}

	public void SendCollisionEnter(BC_CollisionData col)
	{
		if (this.colEnters != null)
		{
			for (int i = 0; i < this.colEnters.Count; i++)
			{
				this.colEnters[i].OnCollisionEnter(col);
				EventListener.CollisionEnter(col); // Invoke custom event
			}
		}
	}

	public void SendCollisionStay(BC_CollisionData col)
	{
		if (this.colStays != null)
		{
			for (int i = 0; i < this.colStays.Count; i++)
			{
				this.colStays[i].OnCollisionStay(col);
				EventListener.CollisionStay(col); // Invoke custom event
			}
		}
	}

	public void SendCollisionExit(BC_CollisionData col)
	{
		if (this.colExits != null)
		{
			for (int i = 0; i < this.colExits.Count; i++)
			{
				this.colExits[i].OnCollisionExit(col);
				EventListener.CollisionExit(col); // Invoke custom event
			}
		}
	}

	public void SendTriggerEnter(BC_TriggerData data)
	{
		if (this.trigEnters != null)
		{
			for (int i = 0; i < this.trigEnters.Count; i++)
			{
				this.trigEnters[i].OnTriggerEnter(data);
				EventListener.TriggerEnter(data); // Invoke custom event
			}
		}
	}

	public void SendTriggerStay(BC_TriggerData data)
	{
		if (this.trigStays != null)
		{
			for (int i = 0; i < this.trigStays.Count; i++)
			{
				this.trigStays[i].OnTriggerStay(data);
				EventListener.TriggerStay(data); // Invoke custom event
			}
		}
	}

	public void SendTriggerExit(BC_TriggerData data)
	{
		if (this.trigExits != null)
		{
			for (int i = 0; i < this.trigExits.Count; i++)
			{
				this.trigExits[i].OnTriggerExit(data);
				EventListener.TriggerExit(data); // Invoke custom event
			}
		}
	}
}
