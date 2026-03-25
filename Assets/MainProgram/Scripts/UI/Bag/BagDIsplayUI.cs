using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BagDisplayUI : SingleMonoBase<BagDisplayUI>
{
    public MainItem mainItem;
    public Grid gridPrefab;
    public GameObject myBag;
    [HideInInspector]
    public static Dictionary<string , Item> ItemDic = new Dictionary<string,Item>();
    #region 介绍窗口
    
    public TextMeshProUGUI introText;
    public UnityEngine.UI.Button UsingButton;
    #endregion

    private string selectedItemName;

    #region 道具效果
    private  float HPPoRecover=25.0f;
    private float DamagePoRecover = -25.0f;
    #endregion

    private bool isDead = false;
    private bool isOverHP = false;
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener("点击物品弹出介绍信息", DisplayInfo);
        EventCenter.Instance.AddEventListener("选中的对应物品", UpdateSelectedItemName);
        
        EventCenter.Instance.AddEventListener("玩家血量达到上限", (isoverhp) => { isOverHP = (bool)isoverhp; });
        EventCenter.Instance.AddEventListener("玩家血量脱离上限", (isoverhp) => { isOverHP = (bool)isoverhp; });
        EventCenter.Instance.AddEventListener("玩家死亡", (isdead) => { isDead = (bool)isdead; });
        EventCenter.Instance.AddEventListener("玩家存活", (isdead) => { isDead = (bool)isdead; });
        updateItemToUI();
        UnityEngine.Cursor.lockState = CursorLockMode.None;
       

    }
    private void Start()
    {
        UsingButton.onClick.AddListener(UseThing);
        gameObject.SetActive(false);
    }
    private void Update()
    {
      
    }
    /// <summary>
    /// 更新获取选中物品名字
    /// </summary>
    /// <param name="info"></param>
    private void UpdateSelectedItemName(object info)
    {
        selectedItemName = (string)info;
    }
    /// <summary>
    /// 使用道具
    /// </summary>
    private void UseThing()
    {
        if (ItemDic[selectedItemName].itemNum > 0)
        {
            print(selectedItemName);
            switch (selectedItemName)
            {
                case "HPPotion":
                    if (isOverHP != true && isDead == false)
                    {
                    HPPotionUse();
                    
                    }
                   
                    break;
                case "DamagePotion":
                    if (isDead == false)
                    {
                    DamagePotionUse();
                    
                    }
                   
                    break;
                default:
                    break;
            } 
        }
        
    }
    /// <summary>
    /// 恢复药水效果
    /// </summary>
    private void HPPotionUse()
    {
        
        EventCenter.Instance.EventTrigger("玩家血量更新", HPPoRecover);
        ItemDic["HPPotion"].itemNum--;
        updateItemToUI();
        
        
    }
    /// <summary>
    /// 毒药效果
    /// </summary>
    private void DamagePotionUse()
    {
        
        EventCenter.Instance.EventTrigger("玩家血量更新",DamagePoRecover);
        ItemDic["DamagePotion"].itemNum--;
        updateItemToUI();
        
        
    }
   
    private void DisplayInfo(object info)
    {
        introText.text = info.ToString();
    }
    
    protected override void Awake()
    {
        base.Awake();
       
    }
    /// <summary>
    /// 向背包中插入新物品
    /// </summary>
    /// <param name="item"></param>
    public static void insertItemToUI(Item item)
    {
        Grid grid = Instantiate(BagDisplayUI.Instance.gridPrefab, BagDisplayUI.Instance.myBag.transform);
        grid.GridImage.sprite = item.itemImage;
        grid.GridText.text = item.itemNum.ToString();
        grid.Info = item.itemInfo;
        grid.selectedItem = item;
        if(!ItemDic.ContainsKey(item.itemName))
        ItemDic.Add(item.itemName, item);
    }
    /// <summary>
    /// 更新背包UI中的物品
    /// </summary>
    public static void updateItemToUI()
    {
       
        for (int i = 0; i < BagDisplayUI.Instance.myBag.transform.childCount; i++)
        { 
            Destroy(BagDisplayUI.Instance.myBag.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < BagDisplayUI.Instance.mainItem.itemList.Count; i++)
        {
            if(BagDisplayUI.Instance.mainItem.itemList[i].itemNum>=1)
            insertItemToUI(BagDisplayUI.Instance.mainItem.itemList[i]);
            
        }
    }

    private void OnDisable()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
}
