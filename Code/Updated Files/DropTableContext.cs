using System;
using System.Collections.Generic;

public class DropTableContext
{
	Dictionary<string, int> savedStates = new Dictionary<string, int>();

	public Dictionary<DropTable, DropTableContext.TableState> tables = new Dictionary<DropTable, DropTableContext.TableState>(); // Made public

	public int NextPositionForTable(DropTable table)
	{
		DropTableContext.TableState tableState;
		if (!this.tables.TryGetValue(table, out tableState))
		{
			int pos;
			if (!this.savedStates.TryGetValue(table.name, out pos))
			{
				pos = 0;
			}
			tableState = new DropTableContext.TableState(table, pos);
			this.tables.Add(table, tableState);
		}
		return tableState.AdvancePosition();
	}

	public void SaveState(IDataSaver saver)
	{
		foreach (KeyValuePair<DropTable, DropTableContext.TableState> keyValuePair in this.tables)
		{
			saver.SaveInt(keyValuePair.Key.name, keyValuePair.Value.Position);
		}
	}

	public void LoadState(IDataSaver saver)
	{
		this.tables = new Dictionary<DropTable, DropTableContext.TableState>();
		this.savedStates = new Dictionary<string, int>();
		string[] allDataKeys = saver.GetAllDataKeys();
		for (int i = 0; i < allDataKeys.Length; i++)
		{
			this.savedStates[allDataKeys[i]] = saver.LoadInt(allDataKeys[i]);
		}
	}

	public class TableState // Made public
	{
		DropTable table;

		int pos;

		List<int> randomIndices;

		public TableState(DropTable table, int pos = 0)
		{
			this.table = table;
			this.pos = pos;
			if (table.Randomize)
			{
				this.randomIndices = DropTableContext.TableState.Randomize(table.NumItems);
			}
		}

		static List<int> Randomize(int num)
		{
			List<int> list = new List<int>(num);
			while (--num >= 0)
			{
				list.Add(num);
			}
			ListUtility.Shuffle<int>(list);
			return list;
		}

		public int Position
		{
			get
			{
				return this.pos;
			}
		}

		public int AdvancePosition()
		{
			int num = this.pos++;
			if (this.randomIndices != null)
			{
				num = this.randomIndices[num];
			}
			if (this.pos >= this.table.NumItems)
			{
				this.pos = 0;
				if (this.table.Randomize)
				{
					this.randomIndices = DropTableContext.TableState.Randomize(this.table.NumItems);
				}
			}
			return num;
		}
	}
}
