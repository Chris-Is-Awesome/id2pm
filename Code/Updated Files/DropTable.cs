using System;
using UnityEngine;

[PersistentScriptableObject]
public class DropTable : ScriptableObject
{
	[SerializeField]
	public ItemBase[] _items; // Made public

	[SerializeField]
	bool _randomize;

	public bool IsEmpty
	{
		get
		{
			if (this._items == null || this._items.Length == 0)
			{
				return true;
			}
			for (int i = 0; i < this._items.Length; i++)
			{
				if (this._items[i] != null)
				{
					return false;
				}
			}
			return true;
		}
	}

	public int NumItems
	{
		get
		{
			return this._items.Length;
		}
	}

	public bool Randomize
	{
		get
		{
			return this._randomize;
		}
	}

	public ItemBase GetNextItem(DropTableContext context)
	{
		int num = context.NextPositionForTable(this);
		if (num >= 0 && num < this._items.Length)
		{
			return this._items[num];
		}
		return null;
	}
}
