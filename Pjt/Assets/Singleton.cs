using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

//单例模式
public abstract class CSharpSingletion<T> where T : new()
{
	private static T _instance;
	private static readonly object _lockObj = new object();
    public static T Instance
    {
        get
        {
			if (_instance == null)
			{
				lock (_lockObj)
				{
					if (_instance == null)
						_instance = new T();
				}
			}
            return _instance;
        }
    }
}

public class MonoSingletion<T> : MonoBehaviour where T : MonoBehaviour
{
    private static string _monoSingletionName = "MonoSingletionRoot";
    private static GameObject _monoSingletionRoot;
    private static T _instance;

    public static bool ApplicationIsQuitting { get; private set; }

    //设置挂在该组件所在的gameobject
    public static void SetMountGameObject(GameObject go)
    {
        _monoSingletionRoot = go;
    }

    public void OnApplicationQuit()
    {
        ApplicationIsQuitting = true;
    }

    public static T Instance
    {
        get
        {
            if (ApplicationIsQuitting)
            {
                if (_instance != null)
                    return _instance;
                return null;
            }

            if (_monoSingletionRoot == null)//如果是第一次调用单例类型就查找所有单例类的总结点           
            {
                _monoSingletionRoot = GameObject.Find(_monoSingletionName);
                if (_monoSingletionRoot == null)//如果没有找到则创建一个所有继承MonoBehaviour单例类的节点                
                {
                    _monoSingletionRoot = new GameObject();
                    _monoSingletionRoot.name = _monoSingletionName;
                    DontDestroyOnLoad(_monoSingletionRoot);//防止被销毁               
                }
            }
            
            if (_instance == null)//为空表示第一次获取当前单例类            
            {
                _instance = _monoSingletionRoot.GetComponent<T>();
                if (_instance == null)//如果当前要调用的单例类不存在则添加一个                
                {
                    _instance = _monoSingletionRoot.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

}

