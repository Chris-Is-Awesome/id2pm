using System;
using System.Collections.Generic;
using UnityEngine;

public class GenericGameEvents
{
	Dictionary<string, List<GenericGameEvents.EventFunc>> reggedEvents = new Dictionary<string, List<GenericGameEvents.EventFunc>>();

	public static readonly GenericGameEvents Instance = new GenericGameEvents();

	static GenericGameEvents.Tag AddToList(List<GenericGameEvents.EventFunc> list, GenericGameEvents.EventFunc func)
	{
		for (int i = 0; i < list.Count - 1; i++)
		{
			if (list[i] == null)
			{
				list[i] = func;
				return new GenericGameEvents.Tag(list, func);
			}
		}
		list.Add(func);
		return new GenericGameEvents.Tag(list, func);
	}

	public GenericGameEvents.Tag AddListener(string eventName, GenericGameEvents.EventFunc func)
	{
		List<GenericGameEvents.EventFunc> list;
		if (!this.reggedEvents.TryGetValue(eventName, out list))
		{
			list = new List<GenericGameEvents.EventFunc>();
			this.reggedEvents.Add(eventName, list);
		}
		if (!list.Contains(func))
		{
			return GenericGameEvents.AddToList(list, func);
		}
		return new GenericGameEvents.Tag(null, null);
	}

	static void DoSend(string name, List<GenericGameEvents.EventFunc> funcs, object data)
	{
		for (int i = 0; i < funcs.Count; i++)
		{
			try
			{
				if (funcs[i] != null)
				{
					funcs[i](data);
				}
			}
			catch (Exception ex)
			{
				/* Commented out to keep output log clean
				Debug.LogWarning("In event " + name);
				Debug.LogException(ex);
				*/
			}
		}
	}

	public void SendEvent(string eventName, object data = null)
	{
		List<GenericGameEvents.EventFunc> funcs;
		if (this.reggedEvents.TryGetValue(eventName, out funcs))
		{
			GenericGameEvents.DoSend(eventName, funcs, data);
		}
	}

	public class Tag
	{
		List<GenericGameEvents.EventFunc> list;

		GenericGameEvents.EventFunc func;

		public Tag(List<GenericGameEvents.EventFunc> list, GenericGameEvents.EventFunc func)
		{
			this.list = list;
			this.func = func;
		}

		public void Stop()
		{
			List<GenericGameEvents.EventFunc> list = this.list;
			GenericGameEvents.EventFunc eventFunc = this.func;
			this.list = null;
			this.func = null;
			if (list != null && eventFunc != null)
			{
				int num = list.IndexOf(eventFunc);
				if (num != -1)
				{
					list[num] = null;
				}
			}
		}
	}

	public delegate void EventFunc(object data);
}
