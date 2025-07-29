using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectUI : MonoBehaviour
{
    public  GameObject plantButtonGroup;
    public  GameObject animalButtonGroup;
    public  GameObject killerButtonGroup;

    





    private StageManager stageManager; // 缓存单例引用
    private float updateInterval = 0.1f; // 更新间隔（秒）
    private float lastUpdateTime;

    void Start()
    {
        stageManager = StageManager.Instance; // 缓存单例
        lastUpdateTime = Time.time;
    }

    void Update()
    {
      
    }
}
