using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TableCtrl : CSharpSingletion<TableCtrl>
{
	#region 常量与字段
	private static TableCtrl _instance;	public readonly Dictionary<string, Table_Output> Table_OutputDic = new Dictionary<string, Table_Output>();

	public bool IsInitOk = false;
	#endregion

	#region 方法
	public void OnInit()
	{
		Init("Tables/Txt/");
	}

	public void Init(string url)
	{
		Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();
		if (Directory.Exists(url))
		{
			DirectoryInfo direction = new DirectoryInfo(url);
			FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
			int len = files.Length;
			if (len == 0)
			{
				Debug.LogError("ERROR PATH[" + url + "]");
				return;
			}

			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Name.EndsWith(".meta"))
				{
					continue;
				}
				data.Add(files[i].Name.Split('.')[0].ToLower(), AuthGetFileData(files[i].DirectoryName + "/" + files[i].Name));
			}

			TableCtrl.Instance.ParseData(data);

		}
		IsInitOk = true;
	}

	public byte[] AuthGetFileData(string fileUrl)
	{
		FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
		byte[] buffur = new byte[fs.Length];
		fs.Read(buffur, 0, buffur.Length);
		fs.Close();
		return buffur;
	}
	private void ParseDataToVO<T>(Dictionary<string, T> dic, Dictionary<string, byte[]> data)
	{
		string name =  typeof(T).ToString().ToLower().Replace("table_","");
		try
		{
		byte[] vos = null;
		data.TryGetValue(name, out vos);
		if (null == vos)
		{
			Debug.LogError("table" + name +"is TryGetValue  ERROR !");
			return;
		}
		T[]  tarr = CsvImporter.Parser<T>(vos);

		string id = "";
		T vo;
		for (int i = 0, count = tarr.Length; i < count; i++)
		{
			vo = tarr[i];
			id = ObjUtil.GetValue(vo, "id").ToString();

			if (!dic.ContainsKey(id + ""))
			{
				dic.Add(id + "", vo);
				continue;
			}
			Debug.Log(name + " table.ID " + id +" is duplicated!");
		}
		}catch(Exception e) {
		Debug.LogError("=存在问题的配置表 ：" + name );
		}
	}

	public void  ParseData(object obj)
	{
		Dictionary<string, byte[]> data = obj as Dictionary<string, byte[]>;
		ParseDataToVO<Table_Output>(Table_OutputDic, data);

		data.Clear();
		data = null;
	}

	#region 获取各表数据
	public Table_Output GetTable_Output(string key)
	{
		Table_Output tmp;
		if (Table_OutputDic.TryGetValue(key, out tmp))
		{
			return tmp;
		}
		return tmp;
	}

	#endregion
	#endregion
}