using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ABUpdateMgr : MonoBehaviour
{
    private static ABUpdateMgr instance;
    private Dictionary<string, ABInfo> remoteABInfo = new Dictionary<string, ABInfo>();
    private Dictionary<string, ABInfo> localABInfo = new Dictionary<string, ABInfo>();
    private List<string> downLoadList = new List<string>();
    private string serverIP = "ftp://10.18.225.6";
    public async void DownLoadABFile(UnityAction<bool> overCallBack,UnityAction<string> updatePro)
    {
        
        string localPath = Application.persistentDataPath + "/";
        bool isOver=false;
        //下载成功的列表
        List<string> tempList = new List<string>();
        //重新下载最大次数
        int redownLoadMaxNum = 5;
        //下载成功的资源数
        int downLoadOverNum=0;
        //这一次下载需要下多少个资源
        int downLoadMaxNum = downLoadList.Count;
        while (downLoadList.Count>0&&redownLoadMaxNum>0)
        {
            for (int i = 0; i < downLoadList.Count; i++)
            {
                isOver = false;
                await Task.Run
                    (() => { isOver= DownLoadFile(downLoadList[i], localPath + downLoadList[i]); });
                if (isOver)
                {
                    updatePro(++downLoadOverNum+"/"+ downLoadMaxNum);
                    
                    tempList.Add(downLoadList[i]);
                    EventCenter.Instance.EventTrigger("热更新进度条最大值更新", downLoadMaxNum);
                    EventCenter.Instance.EventTrigger("热更新进度条当前值更新", downLoadOverNum);
                }
            }
            for (int i = 0; i < tempList.Count; i++)
            {
                downLoadList.Remove(tempList[i]);
            }
                --redownLoadMaxNum;
        }
        overCallBack(downLoadList.Count==0);
        if (!isOver)
        {
            print("网络有问题,下载失败");
        }
    }
    public static ABUpdateMgr Instance
    {
        get 
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ABUpdateMgr");
                instance = obj.AddComponent < ABUpdateMgr> ();

            }
            return instance;
        }
    }
   

    public async void DownloadABCompareFile(UnityAction<bool> overCallBack)
    {
        string PDPath = Application.persistentDataPath;
        bool isOver = false;
        int reDownloadMaxnum = 5;
        while(!isOver&&reDownloadMaxnum>0)
        {
            await Task.Run(() => { isOver= DownLoadFile("ABCompareInfo.txt",PDPath+"/ABCompareInfo_TMP.txt");});
            --reDownloadMaxnum;
        }
        overCallBack?.Invoke(isOver);
      
        
    }
    public void CheckUpdate(UnityAction<bool> overCallBack,UnityAction<string> updateInfoCallBack)
    {
        remoteABInfo.Clear();
        localABInfo.Clear();
        downLoadList.Clear();
        DownloadABCompareFile((isOver) => {
            updateInfoCallBack("开始更新资源");
            if (isOver)
            {
                updateInfoCallBack("对比文件下载结束");
                string remoteInfo = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
                updateInfoCallBack("解析远端对比文件");
                GetRemoteABCompareFileInfo(remoteInfo, remoteABInfo);
                updateInfoCallBack("解析远端对比文件完成");
                updateInfoCallBack("开始对比");
                GetlocalABCompareFileInfo((isOver) => {
                    if (isOver)
                    { 
                        updateInfoCallBack("解析本地对比文件完成");
                        foreach (var abName in remoteABInfo.Keys)
                        {
                            if (!localABInfo.ContainsKey(abName))
                                downLoadList.Add(abName);
                            else
                            {
                                if (localABInfo[abName].md5 != remoteABInfo[abName].md5)
                                {
                                    downLoadList.Add(abName);
                                }
                                localABInfo.Remove(abName);
                            }
                        }
                        updateInfoCallBack("对比完成");
                        updateInfoCallBack("删除无用的AB包文件");
                        foreach (var abName in localABInfo.Keys)
                        {
                            if (File.Exists(Application.persistentDataPath + "/" + abName))
                                File.Delete(Application.persistentDataPath + "/" + abName);
                        }
                        updateInfoCallBack("下载和更新AB包");
                        DownLoadABFile((isOver) => {
                            if (isOver)
                            {
                                updateInfoCallBack("更新本地AB包文件为最新");
                                File.WriteAllText(Application.persistentDataPath + "ABCompareInfo.txt", remoteInfo);
                            }
                            overCallBack(true);
                        },updateInfoCallBack);
                    }
                    else
                    {
                        overCallBack(false);
                    }
                   
                });
            }
            else
            {
                overCallBack(false);
            }
        });
    }
    /// <summary>
    /// 获取下载下来的AB包信息
    /// </summary>
    public void GetRemoteABCompareFileInfo(string info,Dictionary<string,ABInfo> ABInfo)
    { 
            //string info = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
            string[] strs = info.Split("|");
            string[] infos = null;
            for (int i = 0; i < strs.Length; i++)
            {
                infos = strs[i].Split(' ');
                ABInfo.Add(infos[0], new ABInfo(infos[0], infos[1], infos[2]));
            }
            print("远端AB包对比文件 内容获取结束"); 
    }

    public void GetlocalABCompareFileInfo(UnityAction<bool> overCallBack)
    {
        if (File.Exists(Application.persistentDataPath + "/ABCompareInfo.txt"))
        {
            StartCoroutine(GetlocalABCompareFileInfo("file:///"+Application.persistentDataPath + "/ABCompareInfo.txt",overCallBack));
            
        }
        else if (File.Exists(Application.streamingAssetsPath + "/ABCompareInfo.txt"))
        {
            string path =
#if UNITY_ANDROID
Application.streamingAssets;
#else  
                "file:///" + Application.streamingAssetsPath;
#endif

           StartCoroutine(GetlocalABCompareFileInfo(path+ "/ABCompareInfo.txt",overCallBack)) ;
            
        }
        else
        {
            overCallBack(true);
        }
    }
    private IEnumerator GetlocalABCompareFileInfo(string filePath,UnityAction<bool> overCallBack)
    {
        UnityWebRequest req = UnityWebRequest.Get(filePath);
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        { 
            GetRemoteABCompareFileInfo(req.downloadHandler.text, localABInfo);
            
            overCallBack(true);
        }
        else
        {
            overCallBack(false);
        }
        
    }
    private bool DownLoadFile(string fileName,string localPath)
    {
        try
        {
            string pInfo =
#if UNITY_IOS
"IOS";
#elif UNITY_ANDROID
"Android";
#else
"PC";
#endif
        FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP+ "/AB/"+pInfo+"/" + fileName)) as FtpWebRequest;
        NetworkCredential n = new NetworkCredential("test", "123456");
        req.Credentials = n;
        req.Proxy = null;
        req.KeepAlive = false;
        req.Method = WebRequestMethods.Ftp.DownloadFile;
        req.UseBinary = true;
        FtpWebResponse res = req.GetResponse() as FtpWebResponse;
        Stream downloadStream = res.GetResponseStream();
        
            using (FileStream file = File.Create(localPath))
            {
                byte[] bytes = new byte[2048];
                int contengLength = downloadStream.Read(bytes, 0, bytes.Length);
                while (contengLength != 0)
                {
                    file.Write(bytes, 0, contengLength);
                    contengLength = downloadStream.Read(bytes, 0, bytes.Length);
                }
                file.Close();
                downloadStream.Close();
                print(fileName + "下载成功");
                return true;
            }
        }
        catch(Exception ex) 
        {
            print(fileName + "下载失败" + ex.Message);
            return false;
        }
        
    }
    private void OnDestroy()
    {
        instance = null;
    }
    public class ABInfo
    {
        public string name;
        public long size;
        public string md5;
        public ABInfo(string name, string size, string md5)
        {
            this.name = name;
            this.size = long.Parse(size);
            this.md5 = md5;
        } 
    }
    
}
