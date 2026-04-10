using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceManager : MonoBehaviour
{
    
    public static SceManager Instance;

    void Awake()
    {
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
       
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string name, UnityAction fun)
    {
        SceneManager.LoadScene(name);
        fun?.Invoke();
    }

    public void LoadSceneAsync(string name, UnityAction fun)
    {
        StartCoroutine(RealLoadSceneAsync(name, fun));
    }

    private IEnumerator RealLoadSceneAsync(string name, UnityAction fun)
    {
       
        if (this == null) yield break;

        AsyncOperation ao = SceneManager.LoadSceneAsync(name);

        while (!ao.isDone)
        {
          
            if (EventCenter.Instance != null)
            {
                EventCenter.Instance.EventTrigger("进度条更新", ao.progress);
            }

            yield return null; // 
        }

        
        yield return null;

        
        fun?.Invoke();
    }
}

