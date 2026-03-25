using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    #region 玩家血量相关
    public Image HealthBar;
    private float MaxHealth =100f;
    private float CurrentHealth ;
    #endregion
    private void Start()
    {
        HealthBarEnable();
        CurrentHealth = 100f;
        HealthBar.fillAmount = CurrentHealth/MaxHealth;
        EventCenter.Instance.EventTrigger("玩家当前血量更新", CurrentHealth);
        
        EventCenter.Instance.EventTrigger("玩家血量达到上限",false);
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
