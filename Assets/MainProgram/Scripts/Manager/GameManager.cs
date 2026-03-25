using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : SingleMonoBase<GameManager>
{
    public PlayerModel[] playerModels;
    public GameObject OverScene;
    [HideInInspector]
    public bool IsBackPackOpen=false;
    private void Start()
    {
        EventCenter.Instance.AddEventListener("ÕÊº“À¿Õˆ", OnPlayerDie);
        ResManager.Instance.LoadABResAsync<GameObject>("model", "Cube", (ob) =>
        {
            ob.transform.position = new Vector3(-3, 0, 4);

        }
           );
        PlayerController.input.Enable();
        StartCoroutine(SpawnZombies());

        //print(Application.persistentDataPath);
    }

    private IEnumerator SpawnZombies()
    {
        for (int i = 0; i < 4; i++)
        { 
            ResManager.Instance.LoadABResAsync<GameObject>("model", "Enemy", (ob) =>
            {
                ob.transform.position = new Vector3(1, -2, 18+i);

            }
            );
        }
        
        yield return  new WaitForSeconds(10);
        for (int i = 0; i < 4; i++)
        {
            ResManager.Instance.LoadABResAsync<GameObject>("model", "Enemy", (ob) =>
            {
                ob.transform.position = new Vector3(1, -2, 18+i);

            }
            );
        }
    }

    private void OnPlayerDie(object info)
    {
        OverScene.SetActive(true);
        Time.timeScale = 0;
        PlayerController.input.Disable();
        Cursor.lockState = CursorLockMode.None;
    }

   
}
