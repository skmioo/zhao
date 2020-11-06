using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTest : MonoBehaviour 
{

	// Use this for initialization
	void Awake () 
	{
        TableCtrl.Instance.OnInit();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if (Input.GetMouseButtonDown(0)) 
		//{
		//	Table_Achievement  column = TableCtrl.Instance.GetTable_Achievement("2");
		//	MyDebug.Log("name :" + column.name);
		//}
	}
}
