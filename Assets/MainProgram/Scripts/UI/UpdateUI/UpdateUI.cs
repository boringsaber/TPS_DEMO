using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image loadingBar;
    private float MaxLoading = 100;
    private float CurrentLoading = 0;
    private void Awake()
    {
       
        
       
    }
    private void Start()
    {
       EventCenter.Instance.AddEventListener("进度条更新",ProgressBarUpdate);
    }
    /// <summary>
    /// 更新进度条
    /// </summary>
    /// <param name="progress"></param>
    private void ProgressBarUpdate(object progress)
    {
        
        CurrentLoading =(float) progress;
        loadingBar.fillAmount = (CurrentLoading);
        text.text = (int)(CurrentLoading*100)+"/"+MaxLoading;
        
    }

}
