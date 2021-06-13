using System;
using System.Collections.Generic;
using UnityEngine;
using ModStuff;

[AddComponentMenu("Ittle 2/Entity/Drop handler")]
public class EntityDropHandler : EntityHitListener
{
	[SerializeField]
	int _dropsForSuper = 30;

	[SerializeField]
	int _noHitsForSuper = 10;

	[SerializeField]
	int _dropsForMaster = 60;

	[SerializeField]
	DropTable _masterTable;

	EntityDropHandler.State state;

	protected override void DoActivate()
	{
		this.state = default(EntityDropHandler.State);
	}

	protected override void DoWriteSaveData(IDataSaver local, IDataSaver level)
	{
		local.SaveInt("dt_master", this.state.masterCounter);
		local.SaveInt("dt_drops", this.state.dropCounter);
	}

	protected override void DoReadSaveData(IDataSaver local, IDataSaver level)
	{
		this.state.masterCounter = local.LoadInt("dt_master");
		this.state.dropCounter = local.LoadInt("dt_drops");
	}

	void ResetHitCounter()
	{
		this.state.noHitCounter = 0;

		SaveManager.SaveToPrefs("droptables/noHitCount", 0); // Reset noHitCount
	}

	void ResetMasterCounter()
	{
		this.state.masterCounter = 0;
	}

	void ResetMainCounters()
	{
		this.state.dropCounter = (this.state.noHitCounter = 0);
	}

	public override bool HandleHit(ref HitData data, ref HitResult inResult)
	{
		if (data.IsDamageMoreThan(0f))
		{
			this.ResetHitCounter();
		}
		return true;
	}

	public void AddDropTransformer(EntityDropHandler.IDropTransformer transformer)
	{
		if (this.state.transformers == null)
		{
			this.state.transformers = new List<EntityDropHandler.IDropTransformer>();
		}
		int priority = transformer.Priority;
		for (int i = 0; i < this.state.transformers.Count; i++)
		{
			if (this.state.transformers[i].Priority < priority)
			{
				this.state.transformers.Insert(i, transformer);
				return;
			}
		}
		this.state.transformers.Add(transformer);
	}

	public void RemoveDropTransformer(EntityDropHandler.IDropTransformer transformer)
	{
		if (this.state.transformers == null)
		{
			return;
		}
		this.state.transformers.Remove(transformer);
	}

	DropTable GetTable(EntityDroppable droppable)
	{
		if (this.state.masterCounter >= this._dropsForMaster && this._masterTable != null)
		{
			this.ResetMasterCounter();
			return this._masterTable;
		}
		if (this.state.dropCounter >= this._dropsForSuper || this.state.noHitCounter >= this._noHitsForSuper)
		{
			this.ResetMainCounters();
			return droppable.SuperTable;
		}
		return droppable.MainTable;
	}

	public ItemBase HandleItemDrop(EntityDroppable droppable)
	{
		bool forceDrop = false;
		if (this.state.transformers != null)
		{
			for (int i = 0; i < this.state.transformers.Count; i++)
			{
				EntityDropHandler.IDropTransformer dropTransformer = this.state.transformers[i];
				if (dropTransformer.CancelDrop(this, droppable))
				{
					return null;
				}
				if (dropTransformer.ForceDrop(this, droppable))
				{
					forceDrop = true;
				}
			}
		}
		this.state.dropCounter = this.state.dropCounter + 1;
		this.state.noHitCounter = this.state.noHitCounter + 1;
		this.state.masterCounter = this.state.masterCounter + 1;
		DropTable table = this.GetTable(droppable);
		if (table == null)
		{
			return null;
		}
		ItemBase itemBase = droppable.AdvanceTable(table, this.owner.DropContext, forceDrop);

		EventListener.EntityDrop(state.masterCounter, state.dropCounter, state.noHitCounter, table, owner.DropContext, itemBase); // Invoke event

		if (this.state.transformers != null)
		{
			for (int j = 0; j < this.state.transformers.Count; j++)
			{
				itemBase = this.state.transformers[j].Transform(itemBase, this, droppable);
			}
		}
		return itemBase;
	}

	public interface IDropTransformer
	{
		int Priority { get; }

		bool CancelDrop(EntityDropHandler handler, EntityDroppable droppable);

		bool ForceDrop(EntityDropHandler handler, EntityDroppable droppable);

		ItemBase Transform(ItemBase item, EntityDropHandler handler, EntityDroppable droppable);
	}

	struct State
	{
		public int masterCounter;

		public int dropCounter;

		public int noHitCounter;

		public List<EntityDropHandler.IDropTransformer> transformers;
	}
}
