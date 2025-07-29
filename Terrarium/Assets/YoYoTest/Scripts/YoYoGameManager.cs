using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoYoGameManager : MonoBehaviour
{
    // 单例实例
    private static YoYoGameManager _instance;

    // 公共访问点
    public static YoYoGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试在场景中找到现有的StageManager
                _instance = FindObjectOfType<YoYoGameManager>();

                // 如果场景中没有，创建一个新的GameObject并添加StageManager组件
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("YoYoGameManager");
                    _instance = singletonObject.AddComponent<YoYoGameManager>();

                    // 确保在场景切换时不被销毁
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

    }

    [ContextMenu("StartGame")]
    public void StartGame()
    {
        Events.OnGameStart.Invoke();
    }

    [ContextMenu("EndGame")]
    public void EndGame()
    {
        Events.OnGameEnd.Invoke();
    }
}
