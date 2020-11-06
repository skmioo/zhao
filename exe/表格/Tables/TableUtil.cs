using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TableUtil
{
    private static int defaultInt = -1;
    private static string defaultString = "-1";
    private static float defaultFloat = -1;
    private static int[] defaultIntArray = new int[] { -1 };
    private static string[] defaultStringArray = new string[] { "-1" };
    private static float[] defaultFloatArray = new float[] { -1 };


    public static bool CheckEmpty(int value)
    {
        return defaultInt.Equals(value);
    }

    public static bool CheckEmpty(float value)
    {
        return defaultFloat.Equals(value);
    }

    public static bool CheckEmpty(string value)
    {
        return defaultString.Equals(value);
    }

    public static bool CheckEmpty(int[] value)
    {
        bool ret = CompareArray(defaultIntArray,value);
        return ret;
    }

    public static bool CheckEmpty(float[] value)
    {
        return CompareArray(defaultFloatArray, value);
    }

    public static bool CheckEmpty(string[] value)
    {
        return CompareArray(defaultStringArray, value);
    }

    private static bool CompareArray(int[] bt1, int[] bt2)
    {
        var len1 = bt1.Length;
        var len2 = bt2.Length;
        if (len1 != len2)
        {
            return false;
        }
        for (var i = 0; i < len1; i++)
        {
            if (bt1[i] != bt2[i])
                return false;
        }
        return true;
    }

    private static bool CompareArray(float[] bt1, float[] bt2)
    {
        var len1 = bt1.Length;
        var len2 = bt2.Length;
        if (len1 != len2)
        {
            return false;
        }
        for (var i = 0; i < len1; i++)
        {
            if (bt1[i] != bt2[i])
                return false;
        }
        return true;
    }

    private static bool CompareArray(string[] bt1, string[] bt2)
    {
        var len1 = bt1.Length;
        var len2 = bt2.Length;
        if (len1 != len2)
        {
            return false;
        }
        for (var i = 0; i < len1; i++)
        {
            if (bt1[i] != bt2[i])
                return false;
        }
        return true;
    }


}
