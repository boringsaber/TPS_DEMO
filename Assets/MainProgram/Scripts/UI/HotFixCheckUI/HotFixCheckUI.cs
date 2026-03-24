using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotFixCheckUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image progressBar;
    private int currentProgress = 0;
    private int maxProgress=0 ;
    
    private void Awake()
    {
      
    }
    void Start()
    {
        print(Application.persistentDataPath);
        EventCenter.Instance.AddEventListener("热更新进度条最大值更新", (MaxProgress) => { maxProgress = (int)MaxProgress; });
        EventCenter.Instance.AddEventListener("热更新进度条当前值更新",UpdateBar);
        
        ABUpdateMgr.Instance.CheckUpdate((isOver) => {
            if (isOver)
            {
                print("检测更新结束,隐藏进度条");

            }
            else
            {
                print("网络出错");
            }
        }, (str) => {
            text.text = str;
        });
    }

    private void UpdateBar(object info)
    {
        if (info is int)
        {
            print(maxProgress);
            currentProgress = (int)info;
            if(currentProgress!=maxProgress)
            progressBar.fillAmount = currentProgress % maxProgress;
            else
            {
                progressBar.fillAmount = 1;
                SceManager.Instance.LoadSceneAsync("StartUI", null);
            }
        }
    }

    
}
