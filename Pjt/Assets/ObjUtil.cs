using System.Reflection;
using System;
public class ObjUtil 
{
    /// <summary>
    /// 通过字符串访问obj的属性
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public static object GetValue(object instance, string propertyName)
    { 
        return instance.GetType().InvokeMember(propertyName, BindingFlags.GetField | BindingFlags.GetProperty, null, instance, null); 
    } 

}
