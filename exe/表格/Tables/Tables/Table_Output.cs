using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Table_Output
{
	/// <summary>
	///序号
	/// <summary>
	[ColumnMapping("id")]
		public int id;
	/// <summary>
	///电缆截面
	/// <summary>
	[ColumnMapping("CableSection")]
		public string CableSection;
	/// <summary>
	///电阻θn=80℃(Ω/km)
	/// <summary>
	[ColumnMapping("Resistance")]
		public float Resistance;
	/// <summary>
	///感抗(Ω/km)
	/// <summary>
	[ColumnMapping("InductiveReactance")]
		public float InductiveReactance;
	/// <summary>
	///单位电压降Δua%[%/(A·km)]
	/// <summary>
	[ColumnMapping("VoltageDrop")]
		public float[] VoltageDrop;
	/// <summary>
	///单位电压降系数Δua%[%/(A·km)]
	/// <summary>
	[ColumnMapping("VoltageDropCoefficient")]
		public float[] VoltageDropCoefficient;

	public override string ToString()
	{
		return base.ToString() + ": [ " + " (id : " + id + ")" + " (CableSection : " + CableSection + ")" + " (Resistance : " + Resistance + ")" + " (InductiveReactance : " + InductiveReactance + ")" + " (VoltageDrop : " + VoltageDrop + ")" + " (VoltageDropCoefficient : " + VoltageDropCoefficient + ")" + " ]";
	}
}
