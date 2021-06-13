using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using ModStuff;

public class RealDataSaver : IDataSaver
{
	string name;

	Dictionary<string, string> values = new Dictionary<string, string>();

	Dictionary<string, RealDataSaver> frames = new Dictionary<string, RealDataSaver>();

	public static readonly RealDataSaver Dummy = new RealDataSaver("__DUMMY__");

	public RealDataSaver(string name)
	{
		this.name = name;
	}

	public string Name
	{
		get
		{
			return this.name;
		}
	}

	public void SaveData(string key, string value)
	{
		EventListener.FlagSaved(key, value); // Invoke custom event
		this.values[key] = value;
	}

	public void SaveBool(string key, bool value)
	{
		EventListener.FlagSaved(key, value); // Invoke custom event
		this.values[key] = ((!value) ? "0" : "1");
	}

	public void SaveInt(string key, int value)
	{
		EventListener.FlagSaved(key, value); // Invoke custom event
		this.values[key] = value.ToString();
	}

	public void SaveFloat(string key, float value)
	{
		EventListener.FlagSaved(key, value); // Invoke custom event
		this.values[key] = value.ToString(CultureInfo.InvariantCulture);
	}

	public string LoadData(string key)
	{
		string empty;
		if (!this.values.TryGetValue(key, out empty))
		{
			empty = string.Empty;
		}
		return empty;
	}

	public bool LoadBool(string key)
	{
		string a = this.LoadData(key);
		return a == "1" || a == "true";
	}

	public int LoadInt(string key)
	{
		int result = 0;
		int.TryParse(this.LoadData(key), out result);
		return result;
	}

	public float LoadFloat(string key)
	{
		float result = 0f;
		float.TryParse(this.LoadData(key), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
		return result;
	}

	public string[] GetAllDataKeys()
	{
		string[] array = new string[this.values.Count];
		int num = 0;
		foreach (KeyValuePair<string, string> keyValuePair in this.values)
		{
			array[num++] = keyValuePair.Key;
		}
		return array;
	}

	public string[] GetLocalSaverNames()
	{
		string[] array = new string[this.frames.Count];
		int num = 0;
		foreach (KeyValuePair<string, RealDataSaver> keyValuePair in this.frames)
		{
			array[num++] = keyValuePair.Key;
		}
		return array;
	}

	public bool HasData(string key)
	{
		return this.values.ContainsKey(key);
	}

	public bool HasLocalSaver(string name)
	{
		return this.frames.ContainsKey(name);
	}

	public IDataSaver GetLocalSaver(string name)
	{
		RealDataSaver realDataSaver;
		if (!this.frames.TryGetValue(name, out realDataSaver))
		{
			realDataSaver = new RealDataSaver(name);
			this.frames.Add(name, realDataSaver);
		}
		return realDataSaver;
	}

	public IDataSaver GetEmptyLocalSaver(string name)
	{
		RealDataSaver realDataSaver = new RealDataSaver(name);
		this.frames[name] = realDataSaver;
		return realDataSaver;
	}

	public void ClearLocalSaver(string name)
	{
		this.frames.Remove(name);
	}

	public void ClearValue(string name)
	{
		this.values.Remove(name);
	}

	public void SignalWrite(DataSaver.OnDoneFunc onDone)
	{
		this.Write();
	}

	public void Write()
	{
		Debug.LogWarning("Saving sub-data not supported");
	}

	public void SignalRead(DataSaver.OnDoneFunc onDone)
	{
		this.Read();
	}

	public void Read()
	{
		Debug.LogWarning("Reading sub-data not supported");
	}

	public RealDataSaver.Data GetSaveData()
	{
		RealDataSaver.Data data = new RealDataSaver.Data(this.name);
		data.values = new Dictionary<string, string>(this.values);
		foreach (KeyValuePair<string, RealDataSaver> keyValuePair in this.frames)
		{
			if (!keyValuePair.Value.IsEmpty())
			{
				data.frames.Add(keyValuePair.Key, keyValuePair.Value.GetSaveData());
			}
		}
		return data;
	}

	public void CopyFromSaveData(RealDataSaver.Data data)
	{
		Dictionary<string, RealDataSaver> dictionary = new Dictionary<string, RealDataSaver>(data.frames.Count);
		foreach (KeyValuePair<string, RealDataSaver.Data> keyValuePair in data.frames)
		{
			RealDataSaver realDataSaver = new RealDataSaver(keyValuePair.Key);
			realDataSaver.CopyFromSaveData(keyValuePair.Value);
			dictionary.Add(keyValuePair.Key, realDataSaver);
		}
		this.name = data.name;
		this.values = data.values;
		this.frames = dictionary;
	}

	public bool IsEmpty()
	{
		if (this.values.Count > 0)
		{
			return false;
		}
		foreach (KeyValuePair<string, RealDataSaver> keyValuePair in this.frames)
		{
			if (!keyValuePair.Value.IsEmpty())
			{
				return false;
			}
		}
		return true;
	}

	public class Data
	{
		public string name;

		public Dictionary<string, string> values = new Dictionary<string, string>();

		public Dictionary<string, RealDataSaver.Data> frames = new Dictionary<string, RealDataSaver.Data>();

		public Data(string name)
		{
			this.name = name;
		}
	}
}
