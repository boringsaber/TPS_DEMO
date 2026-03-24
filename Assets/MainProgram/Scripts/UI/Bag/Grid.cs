using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour,IPointerClickHandler
{
    public UnityEngine.UI.Image  GridImage;
    public TextMeshProUGUI GridText;
    [HideInInspector]
    public string Info;
    [HideInInspector]
    public Item selectedItem;
   
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            
            EventCenter.Instance.EventTrigger("点击物品弹出介绍信息", Info);
            EventCenter.Instance.EventTrigger("选中的对应物品", selectedItem.itemName);
            print("点击物品");
        }
    }
}
