using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public Button StartButton;
    
    private void Awake()
    {
       
    }
    private void Start()
    {
        StartButton.onClick.AddListener(ChangeToGame);
        
    }
    private void ChangeToGame()
    {
        gameObject.SetActive(false);
        SceManager.Instance.LoadSceneAsync("Game", null);
    }
}
