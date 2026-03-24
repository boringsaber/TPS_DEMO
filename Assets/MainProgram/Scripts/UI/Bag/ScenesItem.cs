using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesItem : MonoBehaviour
{
    public Item item;
    public MainItem mainitem;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "MainCharacter")
        {
            if (!mainitem.itemList.Contains(item))
            {
                mainitem.itemList.Add(item);
            }
        }
        item.itemNum++;
        BagDisplayUI.updateItemToUI();
        Destroy(this.gameObject);
    }
}
