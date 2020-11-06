using System;
using System.Collections.Generic;
using System.Reflection;
using Kent.Boogaart.KBCsv;
using UnityEngine;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ColumnMappingAttribute : Attribute
{
    private static readonly Dictionary<Type, Action<ColumnMappingAttribute, HeaderRecord>> s_inits
         = new Dictionary<Type, Action<ColumnMappingAttribute, HeaderRecord>>();
    static ColumnMappingAttribute()
    {
        var methods = typeof(ColumnMappingAttribute).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
        foreach (var methodInfo in methods)
        {
            ParameterInfo[] parameters;
            if (methodInfo.Name.StartsWith("Init")
                && !methodInfo.Name.Equals("Init")
                && (parameters = methodInfo.GetParameters()).Length == 2)
            {
                var tname = methodInfo.Name.Substring(4, methodInfo.Name.Length - 4);
                var type = Type.GetType(tname, false);
                if (type == null)
                    type = Type.GetType("System." + tname, false);
                if (type == null)
                { 
                    if (tname.EndsWith("Array"))
                    {
                        type = Type.GetType("System." + tname.Substring(0, tname.Length - 5) + "[]", false);
                    }
                }
                if (type != null
                    && parameters[0].ParameterType == typeof(ColumnMappingAttribute)
                    && parameters[1].ParameterType == typeof(HeaderRecord))
                {
                    if (!s_inits.ContainsKey(type))
                    {
                        s_inits.Add(
                            type,
                            (Action<ColumnMappingAttribute, HeaderRecord>)
                            Delegate.CreateDelegate(typeof(Action<ColumnMappingAttribute, HeaderRecord>), methodInfo)
                            );
                    }
                }
                else
                {
                    Debug.LogError("ColumnMappingAttribute Failed,type = null" + tname);
                }
            }
        }
        
    }

    private bool isinited;
    private string[] m_header;
    private Func<DataRecord, object> getObject;
    public FieldInfo TargetField;

    public object ImportParam;

    public string Header
    {
        get
        {
            if (m_header == null || m_header.Length == 0)
            {
                return "";
            }
            else
            {
                return m_header[0];
            }
        }
    }

    public ColumnMappingAttribute()
        : this(null)
    {

    }

    public ColumnMappingAttribute(params string[] header)
    {
        m_header = header;
        if (m_header == null)
            m_header = new string[0];
    }

    public void ParseTo(DataRecord record, object target)
    {
        if (getObject == null) return;
        var fvalue = getObject(record);
        
        this.TargetField.SetValue(target, fvalue);
    }

    public void Init(HeaderRecord headers)
    {
        if (this.isinited)
            throw new InvalidOperationException();

        Action<ColumnMappingAttribute, HeaderRecord> initHandler;
        if (s_inits.TryGetValue(this.TargetField.FieldType, out initHandler))
            initHandler(this, headers);
        else
			Debug.LogError("typeof:" + this.TargetField.DeclaringType + ", with field:" + this.TargetField.Name + " not support");

        this.isinited = true;
    }

    private static void InitString(ColumnMappingAttribute attribute, HeaderRecord record)
    {
        int index = SearchHeadIndex(attribute, record);
       
        if (index != -1)
            attribute.getObject = r => 
            {   var str = r[index];
                if (str.Trim().Equals("0") || str.Trim().Equals('0'))
                {
                    return string.Empty;
                }
                return str;
            };
        else
            attribute.getObject = r => string.Empty;
    }

    private static void InitInt32Array(ColumnMappingAttribute attribute, HeaderRecord record)
    { 
        int index = SearchHeadIndex(attribute, record);
       
        if (index != -1)

            attribute.getObject = r =>
                                      {
                                          var str = r[index];
                                          //var strs = str.Split('_');
										  var strs = str.Split('|');
                                          var count = strs.Length;
                                          int[] values = new int[count];
                                          int id;
                                          for (int i = 0; i < count; i++)
                                          {
                                              if (string.IsNullOrEmpty(strs[i]))
                                              {
                                                  values[i] = 0;
                                                  continue;
                                              }
                                              if (int.TryParse(strs[i], out id))
                                              {
                                                  values[i] = id;
                                              }
                                              else
                                              {
												  Debug.LogError("InitInt32Array int.TryParse(strs[i],out id Failed,strs:" + str + ":" + attribute.TargetField.Name + ":Count is" + count);
                                              }
                                          }
                                          return values;
                                      };
        else
            attribute.getObject = r => string.Empty;
    }

    private static void InitStringArray(ColumnMappingAttribute attribute, HeaderRecord record)
    {
        int index = SearchHeadIndex(attribute, record); 

        if (index != -1)

            attribute.getObject = r =>
            { 
                var str = r[index];
                if (null == str || string.Empty == str || "" == str) 
                {
                    return null;
                } 
                //var strs = str.Split('_');
				var strs = str.Split('|');
                var count = strs.Length;
                string[] values = new string[count]; 
                for (int i = 0; i < count; i++)
                {
                    if (string.IsNullOrEmpty(strs[i]))
                    {
                        values[i] = "";
                        continue;
                    }

                    if (strs[i].Trim().Equals("0") || strs[i].Trim().Equals('0'))
                    {
                        values[i] = "";
                        continue;
                    }

                    values[i] = strs[i];  
                }
                return values;
            };
        else
            attribute.getObject = r => string.Empty;
    }

    private static void InitSingleArray(ColumnMappingAttribute attribute, HeaderRecord record)
    {
        int index = SearchHeadIndex(attribute, record);
         if (index != -1)

            attribute.getObject = r =>
            {
                var str = r[index];
                //var strs = str.Split('_');
				var strs = str.Split('|');
                var count = strs.Length;
                float[] values = new float[count];
                float id;
                for (int i = 0; i < count; i++)
                {
                    if (string.IsNullOrEmpty(strs[i]))
                    {
                        values[i] = 0f;
                        continue;
                    }
                    if (float.TryParse(strs[i], out id))
                    {
                        values[i] = id;
                    }
                    else
                    {
						Debug.LogError("InitSingleArray float.TryParse(strs[i],out id Failed,strs:"+str+":"+attribute.TargetField.Name+":Count is "+count);
                    }
                }
                return values;
            };
        else
            attribute.getObject = r => string.Empty;
    }

    private static void InitBoolean(ColumnMappingAttribute attribute, HeaderRecord record)
    {
        var importP = attribute.ImportParam as string;
        if (importP != null)
        {
            int index = SearchHeadIndex(attribute, record);

            if (index != -1)
                attribute.getObject = r => string.Equals(r[index], importP, StringComparison.InvariantCultureIgnoreCase);
            else
                attribute.getObject = r => false;
        }
        else
            ConverToInternal(2, attribute, record);
    }

    private static void InitInt32(ColumnMappingAttribute attribute, HeaderRecord record)
    { ConverToInternal(1, attribute, record); }

    private static void InitInt64(ColumnMappingAttribute attribute, HeaderRecord record)
    { ConverToInternal(4, attribute, record); }

    private static void InitSingle(ColumnMappingAttribute attribute, HeaderRecord record)
    { 
        ConverToInternal(3, attribute, record);
    }   
    private static int SearchHeadIndex(ColumnMappingAttribute attribute, HeaderRecord record)
    {
        var searchs = new List<string>();
        if (attribute.m_header.Length == 0)
            searchs.Add(attribute.TargetField.Name);
        else
            searchs.AddRange(attribute.m_header);

        var index = -1;
        foreach (var search in searchs)
        {
            if ((index = record.IndexOf(search)) != -1)
                return index;
        }
        return index;
    }

    /// <summary>
    ///    1:int; 2:bool; 3:float
    /// </summary>
    private static void ConverToInternal(int t_type, ColumnMappingAttribute attribute, HeaderRecord record)
    {
        int index = SearchHeadIndex(attribute, record);

        switch (t_type)
        {
            case 1: 
                {
                    var type = typeof(int);
                    if (index != -1)
                        attribute.getObject = r =>
                                                  {
                                                      try { return Convert.ChangeType(r[index], type); }
                                                      catch (Exception) { return default(int); }
                                                  };
                    else
                        attribute.getObject = r => default(int);
                    break;
                }
            case 2: 
                {
                    var type = typeof(bool);
                    if (index != -1)
                        attribute.getObject = r =>
                                                  {
                                                      try { return Convert.ChangeType(r[index], type); }
                                                      catch (Exception) { return default(bool); }
                                                  };
                    else
                        attribute.getObject = r => default(bool);
                    break;
                }
            case 3: 
                {
                    var type = typeof(float);
                    if (index != -1)
                        attribute.getObject = r =>
                                                  {
                                                      try { return Convert.ChangeType(r[index], type); }
                                                      catch (Exception) { return default(float); }
                                                  };
                    else
                        attribute.getObject = r => default(float);
                    break;
                }
            case 4:
                {
                    var type = typeof(long);
                    if (index != -1)
                        attribute.getObject = r =>
                        {
                            try { return Convert.ChangeType(r[index], type); }
                            catch (Exception) { return default(long); }
                        };
                    else
                        attribute.getObject = r => default(long);
                    break;
                }
        }
    }
     
     
    private static void ConverToInternal2<T>(ColumnMappingAttribute attribute, HeaderRecord record)
    {
        int index = SearchHeadIndex(attribute, record);

        var type = typeof(T);
        if (index != -1)
            attribute.getObject = r =>
            {
                try { return Convert.ChangeType(r[index], type); }
                catch (Exception) { return default(T); }
            };
        else
            attribute.getObject = r => default(T);
    }
}