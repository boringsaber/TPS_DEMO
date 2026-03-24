using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCenter : SingleMonoBase<EventCenter>
{
    private Dictionary<string, UnityAction<object>> callevent = new Dictionary<string, UnityAction<object>> ();
    /// <summary>
    /// Ìí¼Ó¼àÌý
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callFunc"></param>
    public void AddEventListener(string name, UnityAction<object> callFunc)
    {
        if (callevent.ContainsKey(name))
        {
            callevent[name] += callFunc;
        }
        else
        {
            callevent.Add(name, callFunc);
        }
    }
    /// <summary>
    /// ÊÂ¼þ´¥·¢
    /// </summary>
    public void EventTrigger(string name,object info)
    {
        if(callevent.ContainsKey(name))
        callevent[name].Invoke(info);
    }
    /// <summary>
    /// ÒÆ³ý¼àÌý
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventLister(string name,UnityAction<object> action)
    {
        if (callevent.ContainsKey(name))
            callevent[name] -= action;
    }
    /// <summary>
    /// Çå¿Õ
    /// </summary>
    public void Clear()
    {
        callevent.Clear();
    }
}
