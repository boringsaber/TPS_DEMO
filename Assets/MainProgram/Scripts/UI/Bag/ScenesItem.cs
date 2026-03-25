using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesItem : MonoBehaviour
{
    public Item item;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!BagDisplayUI.Instance.mainItem.itemList.Contains(item) && !BagDisplayUI.ItemDic.ContainsKey(item.name))
            {
                BagDisplayUI.Instance.mainItem.itemList.Add(item);
            }
        BagDisplayUI.ItemDic[item.name].itemNum++;
        BagDisplayUI.updateItemToUI();
        Destroy(this.gameObject);
        }
       
    }
}
