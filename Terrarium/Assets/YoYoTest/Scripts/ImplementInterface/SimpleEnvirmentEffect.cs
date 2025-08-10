using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleEnvirmentEffect : MonoBehaviour
{
    private bool isSpeedSet = false;
    private float maxGrowthSpeed ;
    private IGrowth growthComponent;
    // Start is called before the first frame update
    void Start()
    {
        // 订阅环境数据变化事件
        EnvironmentalParaManager.OnEnvironmentalDataChanged += OnEnvironmentalDataChangedHandler;
        
        // 检查IGrowth接口的IsSpeedSet属性
        CheckGrowthSpeedStatus();
    }

    void Update()
    {
        // 只有当IsSpeedSet为false时才进行检查，一旦变为true就停止检查
        if (!isSpeedSet)
        {
            CheckGrowthSpeedStatus();
        }
    }

    // 检查生长速度设置状态
    private void CheckGrowthSpeedStatus()
    {
        // 获取IGrowth接口（从自己身上）
        if (TryGetComponent<IGrowth>(out growthComponent))
        {
            // 判断IsSpeedSet布尔属性是否为true
            if (growthComponent.IsSpeedSet && !isSpeedSet)
            {
                // 如果为true，则Log一个"哈哈哈"
                isSpeedSet = true;
                maxGrowthSpeed = growthComponent.GrowthSpeed;
                Debug.Log("哈哈哈");
                
                // 主动获取当前环境参数
                EnvironmentalData currentData = EnvironmentalParaManager.Instance.EnvironmentalData;
                
                // 使用环境数据手动设置生长速度
                growthComponent.GrowthSpeed = maxGrowthSpeed * currentData.sunshine;
            }
        }
    }

    // 环境数据变化事件处理方法
    private void OnEnvironmentalDataChangedHandler(EnvironmentalData data)
    {
        if(isSpeedSet)
        {
            growthComponent.GrowthSpeed = maxGrowthSpeed * data.sunshine;
        }
    }

    // 当组件被销毁时取消订阅事件
    private void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
        EnvironmentalParaManager.OnEnvironmentalDataChanged -= OnEnvironmentalDataChangedHandler;
    }


}
