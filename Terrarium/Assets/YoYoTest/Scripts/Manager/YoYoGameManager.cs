using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoYoGameManager : MonoBehaviour
{
    // 单例实例
    private static YoYoGameManager _instance;
    public TextAsset infoTestJson;

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
    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // 如果已经存在实例，销毁当前重复的实例
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // 设置当前实例为单例
        _instance = this;
        DontDestroyOnLoad(gameObject);
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
            LoadInfoTestJson();
        }

        // if(Input.GetKeyDown(KeyCode.A))
        // {
        //     LoadInfoTestJson();
        // }

    }

    void LoadInfoTestJson()
    {
        // 在场景中查找ObjectSelectUI组件
        ObjectSelectUI objectSelectUI = FindObjectOfType<ObjectSelectUI>();
        if (objectSelectUI != null)
        {
            // 将JSON数据传递给ObjectSelectUI
            objectSelectUI.LoadFromJson(infoTestJson);
            Debug.Log("成功加载JSON数据到ObjectSelectUI");
        }
        else
        {
            Debug.LogError("在场景中未找到ObjectSelectUI组件");
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
