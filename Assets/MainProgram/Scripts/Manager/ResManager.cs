using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResManager : SingleMonoBase<ResManager>
{
    private static AssetBundle mainAB = null;
    private static AssetBundleManifest manifest = null;
    private static Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
   
    private string PathUrl
    {
        get
        {
            return Application.persistentDataPath + "/";
        }
    }

    private string MainABName
    {
        get 
        {
#if UNITY_IOS
return "IOS";
#elif UNITY_ANDROID
return "Android";
#else
            return "PC";
#endif
        }
    }
    private void Start()
    {
        
    }
    /// <summary>
    /// 同步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        if (res is GameObject)
        {
            return GameObject.Instantiate(res);
        }
        else
        {
            return res;
        }
    }
    /// <summary>
    /// 异步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="callback"></param>
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        StartCoroutine(RealLoadAsycn(name, callback));
    }

    private IEnumerator RealLoadAsycn<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest res = Resources.LoadAsync<T>(name);
        yield return res;
        if (res.asset is GameObject)
        {
            callback(GameObject.Instantiate(res.asset) as T);
        }
        else
        {
            callback(res.asset as T);
        }
    }
    /// <summary>
    /// 加载AB包
    /// </summary>
    /// <param name="abName"></param>
    public void LoadAB(string abName)
    {
        //加载AB包
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        }
        //获取依赖包相关信息
        AssetBundle ab = null;
         string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //判断包是否加载过
            if (!abDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i],ab);
            }
        }
        //加载资源来源包
        //如果没有加载过 再加载
        if (!abDic.ContainsKey(abName) || abDic[abName]==null)
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName, ab);
        }
    }

    public void LoadABResAsync<T>(string abName, string resName, UnityAction<T> callBack)where T:Object
    {
        StartCoroutine(RealLoadABResAsync<T>(abName, resName, callBack));
    }

    private IEnumerator RealLoadABResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        LoadAB(abName);
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        if (abr.asset is GameObject)
        {
            callBack( Instantiate(abr.asset) as T);
            
           
        }
        else
            callBack(abr.asset as T);
    }

    public void UnLoad(string abName)
    {
        if (abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
            abDic.Remove(abName);
        }
    }

    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest = null;
    }
}
