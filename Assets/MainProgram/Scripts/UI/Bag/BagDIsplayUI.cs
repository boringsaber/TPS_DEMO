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
    private static Dictionary<string , Item> ItemDic = new Dictionary<string,Item>();
    #region 介绍窗口
    
    public TextMeshProUGUI introText;
    public UnityEngine.UI.Button UsingButton;
    #endregion

    private string selectedItemName;

    #region 道具效果
    private  float HPPoRecover=25.0f;
    private float DamagePoRecover = -25.0f;
    #endregion
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener("点击物品弹出介绍信息", DisplayInfo);
        EventCenter.Instance.AddEventListener("选中的对应物品", UpdateSelectedItemName);
        updateItemToUI();
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UsingButton.onClick.AddListener(UseThing);

    }
    private void Update()
    {
      
    }
    private void UpdateSelectedItemName(object info)
    {
        selectedItemName = (string)info;
    }
    private void UseThing()
    {
        if (ItemDic[selectedItemName].itemNum > 0)
        {
            switch (selectedItemName)
            {
                case "HPPotion":
                    HPPotionUse();
                    break;
                case "DamagePotion":
                    DamagePotionUse();
                    break;
                default:
                    break;
            } 
        }
        
    }

    private void HPPotionUse()
    {
        EventCenter.Instance.EventTrigger("玩家血量更新", HPPoRecover);
        ItemDic["HPPotion"].itemNum--;
        updateItemToUI();
    }
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
