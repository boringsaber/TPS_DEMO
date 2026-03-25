using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverUI : MonoBehaviour
{
    private Button tryAgain;
    private Button backToStartMenu;
 
    private void Awake()
    {
        tryAgain = transform.Find("try again").GetComponent<Button>();
        backToStartMenu= transform.Find("BackToStart").GetComponent<Button>();
    }
    void Start()
    {
        tryAgain.onClick.AddListener(LoadGame);
        backToStartMenu.onClick.AddListener(BackToStartMenu);
        gameObject.SetActive(false);
        
    }
    /// <summary>
    /// 重新加载游戏
    /// </summary>
    private void LoadGame()
    {
         Time.timeScale = 1;
        SceManager.Instance.LoadSceneAsync("Game",null);
       
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 回到开始界面
    /// </summary>
    private void BackToStartMenu()
    {
        Time.timeScale = 1;
        SceManager.Instance.LoadSceneAsync("StartUI", null);
        
    }


}

