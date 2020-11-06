using System;
using System.Collections.Generic;
using System.IO;
using Kent.Boogaart.KBCsv;

public static class CsvImporter
{
    public static T[] Parser<T>(byte[] data)
    { 
        var fhandles = GetFieldHandles(typeof(T));
        var reader = new StreamReader(new MemoryStream(data));

        reader.ReadLine();
        using (var csReader = new CsvReader(reader))
        {
            var heads = csReader.ReadHeaderRecord();
            foreach (var handle in fhandles)
            {
                handle.Init(heads); 
            }
  
            var rets = new List<T>();
            DataRecord record = csReader.ReadDataRecord();
			//skip line 3 
            while ((record = csReader.ReadDataRecord()) != null)
            {
                var item = Activator.CreateInstance<T>();

                foreach (var handle in fhandles)
                {
                    
                    handle.ParseTo(record, item); 
                }
                rets.Add(item);
            }

            return rets.ToArray();
        }
    }

    private static ColumnMappingAttribute[] GetFieldHandles(Type type)
    {
        var fields = type.GetFields();  
        var rets = new List<ColumnMappingAttribute>();

        foreach (var fieldInfo in fields)
        {
            if (Attribute.IsDefined(fieldInfo, typeof(ColumnMappingAttribute)))
            {
                var handle = (ColumnMappingAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(ColumnMappingAttribute));
                handle.TargetField = fieldInfo; 
                rets.Add(handle);
            }
        }

        return rets.ToArray();

    }

}