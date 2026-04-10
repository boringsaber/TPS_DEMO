using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public Button StartButton;
    public Button LoadSaveButton;
    public Button QuitButton;
    
   
   
    private void Start()
    {
       
        StartButton.onClick.AddListener(ChangeToGame);
        LoadSaveButton.onClick.AddListener(ChangeToSaveLoadUI);
        QuitButton.onClick.AddListener(QuitGame);

    }
    private void ChangeToGame()
    {
        gameObject.SetActive(false);
        SceManager.Instance.LoadSceneAsync("Game", null);
    }
    private void QuitGame()
    {
        Application.Quit();
    }
    private void ChangeToSaveLoadUI()
    {
       
        SceManager.Instance.LoadSceneAsync("SaveSlot", null);

    }
}
