using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnvirmentEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 订阅环境数据变化事件
        EnvironmentalParaManager.OnEnvironmentalDataChanged += OnEnvironmentalDataChangedHandler;
    }

    // 环境数据变化事件处理方法
    private void OnEnvironmentalDataChangedHandler(EnvironmentalData data)
    {
        // 记录每个数值的变化
        // Debug.Log($"环境数据变化 - 阳光: {data.sunshine}, 温度: {data.temperature}, 湿度: {data.humidity}");
    }

    // 当组件被销毁时取消订阅事件
    private void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
        EnvironmentalParaManager.OnEnvironmentalDataChanged -= OnEnvironmentalDataChangedHandler;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
