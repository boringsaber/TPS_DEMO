using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleMonoBase<GameManager>
{
    public PlayerModel[] playerModels;
    public GameObject OverScene;
    private void Start()
    {
        EventCenter.Instance.AddEventListener("ÕÊº“À¿Õˆ", OnPlayerDie);
        //ResManager.Instance.LoadABResAsync<GameObject>("model","Enemy",null);
        print(EnemyBase.isAttack);
    }

    private void OnPlayerDie(object info)
    {
        OverScene.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }
}
