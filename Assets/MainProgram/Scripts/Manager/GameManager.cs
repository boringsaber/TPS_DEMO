using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

    public class GameManager : SingleMonoBase<GameManager>
    {
        public PlayerModel[] playerModels;
        public GameObject OverScene;
        [HideInInspector]
        public List<GameObject> EnemyList = new List<GameObject>();
        [HideInInspector]
        public List<SaveManager.SerializeableVector3> EnemyPs = new List<SaveManager.SerializeableVector3>();
        [HideInInspector]
        public bool IsBackPackOpen = false;
        private SaveManager.dataToSave data;
    private bool isLoadButtonClicked = false;
       
        private void Start()
        {
        EventCenter.Instance.AddEventListener("读取按钮被点击", (ans) => { isLoadButtonClicked = true; });
        EventCenter.Instance.AddEventListener("玩家死亡", OnPlayerDie);
        ResManager.Instance.LoadABResAsync<GameObject>("model", "Cube", (ob) =>
            {
                ob.transform.position = new Vector3(-3, 0, 4);

            }
               );
            PlayerController.input.Enable();
        StartCoroutine(SpawnZombies());
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
       
        EnemyList.AddRange(enemyList);



        //print(Application.persistentDataPath);
    }
    private void Update()
    {


        GameObject[] enemylist = GameObject.FindGameObjectsWithTag("Enemy");
        EnemyList.Clear();
        EnemyList.AddRange(enemylist);

    }


    private IEnumerator SpawnZombies()
    {
        ZombieRelated();
        yield return new WaitForSeconds(10);
        ZombieRelated();
    }

        private void OnPlayerDie(object info)
        {
            OverScene.SetActive(true);
            Time.timeScale = 0;
            PlayerController.input.Disable();
            Cursor.lockState = CursorLockMode.None;
        }
    /// <summary>
    /// 生成僵尸相关
    /// </summary>
    private void ZombieRelated()
    {
        for (int i = 0; i < 4; i++)
        {
            ResManager.Instance.LoadABResAsync<GameObject>("model", "Enemy", (ob) =>
            {
                ob.transform.position = new Vector3(1, -2, 18 + i);
               
                if (isLoadButtonClicked == true)
                {
                    
                    EventCenter.Instance.EventTrigger("生成僵尸成功", true);
                   
                }

            });
        }
        
    }
    }

