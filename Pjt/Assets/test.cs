using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class formulaStr
{
    public const string func1 = "Δu%=Δua%IL";
}

/// <summary>
/// 通用参数
/// </summary>
public class CableSectionData
{
    public float 电动机值;
    public float 线路长度值;
    public float 额定电压值;
    //cosφ
    public float 单位升压降系数;
    public float 单位升压降;
}

public class test : MonoBehaviour
{
    string[] formulas = { formulaStr.func1 };
    public Button btn;
    public InputField input;
    public string data;
    public Text tip;
    private Dictionary<string, Table_Output> datas;

    //函数公式
    public Dropdown formulasSelect;
    public InputField 电动机;
    public InputField 线路长度;
    public InputField 额定电压;

    //电缆参数选择
    public Dropdown CableSectionSelect;

    //cosφ
    public Dropdown cosSeltaSelect;

    private CableSectionData csData;

    private void Awake()
    {
        csData = new CableSectionData();
        ShowTip(string.Empty);
        InitTable();
        InitFormulas();
    }

    void Start()
    {
        btn.onClick.AddListener(onclick);
    }

    private void onclick()
    {
        Table_Output _Output = getOutputDataByCableSection(CableSectionSelect.captionText.text);
        if (_Output == null)
        {
            ShowTip("数据错误:" + CableSectionSelect.captionText.text);
            return;
        }
        else
        {
            ShowTip("计算成功");
        }
        StringBuilder sb = ReadData(_Output);
        switch (formulasSelect.captionText.text)
        {
            case formulaStr.func1:
                ShowOutPutData(fun1(sb));
                break;
        }
    }

    #region 公式

    public string fun1(StringBuilder sb)
    {
        float I = csData.电动机值 / (csData.单位升压降系数 * csData.额定电压值 * 0.001f * Mathf.Sqrt(3.0f));
        sb.Append("电流值I:            " + I + "A");
        sb.AppendLine();
        float calValue = csData.单位升压降 * I * (csData.线路长度值 * 0.001f);
        sb.Append("电压降Δu%:      " + calValue + "%");
        return sb.ToString();
    }

    #endregion

    #region 数据的读取
    private StringBuilder ReadData(Table_Output output)
    {
        StringBuilder sb = new StringBuilder();
        csData.电动机值 = float.Parse(电动机.text);
        sb.Append("电动机:             " + csData.电动机值 + "Kw");
        sb.AppendLine();
        csData.线路长度值 = float.Parse(线路长度.text);
        sb.Append("线路长度:          " + csData.线路长度值 + "m");
        sb.AppendLine();
        csData.额定电压值 = float.Parse(额定电压.text);
        sb.Append("额定电压:          " + csData.额定电压值 + "V");
        sb.AppendLine();
        sb.Append("所选电缆规格:   " + CableSectionSelect.captionText.text);
        sb.AppendLine();
        int cosSelta = cosSeltaSelect.value;
        csData.单位升压降系数 = output.VoltageDropCoefficient[cosSelta];
        csData.单位升压降 = output.VoltageDrop[cosSelta];
        sb.Append("cosφ:                " + csData.单位升压降系数 + "        对应单位升压降:    " + csData.单位升压降);
        sb.AppendLine();
        return sb;
    }


    public Table_Output getOutputDataByCableSection(string opt)
    {
        foreach (Table_Output _Output in datas.Values)
        {
            if (_Output.CableSection.Equals(opt))
                return _Output;
        }
        return null;
    }

    private void ShowOutPutData(string data)
    {
        input.text = data;
    }


    private void InitTable()
    {
#if UNITY_EDITOR
        TableCtrl.Instance.Init(Application.dataPath +"/Data");
#elif UNITY_STANDALONE_WIN
        TableCtrl.Instance.Init(System.Environment.CurrentDirectory + "/Data");
#endif


        datas = TableCtrl.Instance.Table_OutputDic;
        if (datas.Count > 0 && CableSectionSelect != null)
        {
            for (int i = 1; i < datas.Count + 1; i++)
            {
                OptionData option = new OptionData();
                option.text = datas[i.ToString()].CableSection;
                CableSectionSelect.options.Add(option);
            }
            Table_Output _Output = datas["1"];
            for (int i = 0; i < _Output.VoltageDropCoefficient.Length; i++)
            {
                OptionData option = new OptionData();
                option.text = _Output.VoltageDropCoefficient[i].ToString();
                cosSeltaSelect.options.Add(option);
            }
        }
    }

    void InitFormulas()
    {
        for (int i = 0; i < formulas.Length; i++)
        {
            OptionData option = new OptionData();
            option.text = formulas[i];
            formulasSelect.options.Add(option);
        }
    }

    private void ShowTip(string str)
    {
        if (tip != null)
        {
            tip.text = str;
        }
    }

    #endregion

}
