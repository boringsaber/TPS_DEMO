using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceManager : SingleMonoBase<SceManager>
{

    public void LoadScene(string name,UnityAction fun)
    {
        SceneManager.LoadScene(name);
        fun();
    }

    public void LoadSceneAsync(string name,UnityAction fun)
    {
        StartCoroutine(RealLoadSceneAsync(name,fun));
    }

    private IEnumerator RealLoadSceneAsync(string name,UnityAction fun)
    {

        AsyncOperation ao= SceneManager.LoadSceneAsync(name);
        while (!ao.isDone)
        {
            EventCenter.Instance.EventTrigger("进度条更新", ao.progress);
            yield return ao.progress;
        }
        fun();
    }
}
