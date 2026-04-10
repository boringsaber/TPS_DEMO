using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    #region 玩家血量相关
    public Image HealthBar;
    private float MaxHealth =100f;
    private float CurrentHealth ;
    #endregion
    #region 玩家弹药相关
    public TextMeshProUGUI AmmoText;
    private int backAmmo=30;
    private int currentAmmo = 30;
    #endregion
    private void Start()
    {
        HealthBarEnable();
        CurrentHealth = 100f;
        HealthBar.fillAmount = CurrentHealth/MaxHealth;
        AmmoText.text = currentAmmo + "/" + backAmmo;
        EventCenter.Instance.EventTrigger("玩家当前血量更新", CurrentHealth);
        EventCenter.Instance.AddEventListener("当前子弹数量更新", AmmoUpdate);
        EventCenter.Instance.EventTrigger("玩家血量达到上限",false);
        EventCenter.Instance.AddEventListener("换弹",(none)=> {
            if (backAmmo > 0 && currentAmmo < 30 && currentAmmo + backAmmo >= 30)
            {
                backAmmo -= 30 - currentAmmo;
                currentAmmo = 30; ;
                AmmoText.text = currentAmmo + "/" + backAmmo;
            }
            else if (backAmmo > 0 && currentAmmo < 30 && currentAmmo + backAmmo < 30)
            {
                currentAmmo += backAmmo;
                AmmoText.text = currentAmmo + "/" + backAmmo;
            }
        });
    }
    private void AmmoUpdate(object info)
    {
        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            EventCenter.Instance.EventTrigger("有无子弹", false);
            AmmoText.text = currentAmmo + "/" + backAmmo;
            return;
        }
        if (currentAmmo > 0)
        {
            EventCenter.Instance.EventTrigger("有无子弹", true);
            currentAmmo -= (int)info;
            AmmoText.text = currentAmmo + "/" + backAmmo;

        }

    }
    /// <summary>
    /// 启用玩家血条
    /// </summary>
    private void HealthBarEnable()
    {
        EventCenter.Instance.AddEventListener("玩家最大血量更新", UpdateMaxHealth);
        EventCenter.Instance.AddEventListener("玩家血量更新", UpdateHealthBar);
        
        
    }
    private void UpdateMaxHealth(object info)
    {
        MaxHealth = (float)info;
    }
    /// <summary>
    /// 更新玩家血条回调
    /// </summary>
    /// <param name="info"></param>
    private void UpdateHealthBar(object info)
    {
       
         CurrentHealth += (float)info;
        
        HealthBar.fillAmount = CurrentHealth / MaxHealth; 
        print(CurrentHealth);
        if (CurrentHealth <= 0)
        {
            EventCenter.Instance.EventTrigger("玩家死亡", true);
            CurrentHealth = 0f;

        }
        else if (CurrentHealth >= 100)
        {
            EventCenter.Instance.EventTrigger("玩家血量达到上限", true);
            CurrentHealth = 100f;
            EventCenter.Instance.EventTrigger("玩家存活",false);
        }
        else if (CurrentHealth < 100&&CurrentHealth>0)
        {
            EventCenter.Instance.EventTrigger("玩家血量脱离上限", false);
            EventCenter.Instance.EventTrigger("玩家存活", false);
        }
    }
}
