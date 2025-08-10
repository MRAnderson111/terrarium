using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class MiddleEnvirmentEffect : MonoBehaviour
{
    private bool isSpeedSet = false;
    private bool isQuantityLimitSet = false;
    private float maxGrowthSpeed ;
    private int maxQuantityLimits;
    private IGrowth growthComponent;
    private IGetQuantityLimits quantityLimitsComponent;
    // Start is called before the first frame update
    void Start()
    {
        // 订阅环境数据变化事件
        EnvironmentalParaManager.OnEnvironmentalDataChanged += OnEnvironmentalDataChangedHandler;
        
        // 检查IGrowth接口的IsSpeedSet属性
        CheckGrowthSpeedStatus();
        
        // 检查IGetQuantityLimits接口的QuantityLimits属性
        CheckQuantityLimitsStatus();
    }

    void Update()
    {
        // 只有当IsSpeedSet为false时才进行检查，一旦变为true就停止检查
        if (!isSpeedSet)
        {
            CheckGrowthSpeedStatus();
        }
        
        // 只有当IsQuantityLimitSet为false时才进行检查，一旦变为true就停止检查
        if (!isQuantityLimitSet)
        {
            CheckQuantityLimitsStatus();
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
                growthComponent.GrowthSpeed = maxGrowthSpeed * CalculateOptimalValue(currentData.sunshine);
            }
        }
    }

    // 检查生长数量上限设置状态
    private void CheckQuantityLimitsStatus()
    {
        // 获取IGetQuantityLimits接口（从自己身上）
        if (TryGetComponent<IGetQuantityLimits>(out quantityLimitsComponent))
        {
            // 检查是否已经设置过数量上限
            if (!isQuantityLimitSet)
            {
                isQuantityLimitSet = true;
                maxQuantityLimits = quantityLimitsComponent.QuantityLimits;
                Debug.Log("生长数量上限已设置");
                
                // 主动获取当前环境参数
                EnvironmentalData currentData = EnvironmentalParaManager.Instance.EnvironmentalData;
                
                // 使用环境数据手动设置生长数量上限
                quantityLimitsComponent.QuantityLimits = Mathf.RoundToInt(maxQuantityLimits * CalculateOptimalValue(currentData.humidity));
                Debug.Log($"原始数量上限: {maxQuantityLimits}, 湿度倍数: {currentData.humidity}, 调整后数量上限: {quantityLimitsComponent.QuantityLimits}");
            }
        }
    }

    // 环境数据变化事件处理方法
    private void OnEnvironmentalDataChangedHandler(EnvironmentalData data)
    {
        if(isSpeedSet)
        {
            growthComponent.GrowthSpeed = maxGrowthSpeed * CalculateOptimalValue(data.sunshine);
        }
        
        if(isQuantityLimitSet)
        {
            // 直接修改数量上限
            quantityLimitsComponent.QuantityLimits = Mathf.RoundToInt(maxQuantityLimits * CalculateOptimalValue(data.humidity));
            Debug.Log($"环境变化调整数量上限: {quantityLimitsComponent.QuantityLimits}, 湿度: {data.humidity}");
        }
    }

    // 计算最优值，在0.5时达到峰值，超过0.5会下降，达到1时变为0
    private float CalculateOptimalValue(float environmentValue)
    {
        // 使用抛物线函数：y = 4 * x * (1 - x)
        // 当x=0时，y=0；当x=0.5时，y=1；当x=1时，y=0
        return 4 * environmentValue * (1 - environmentValue);
    }

    // 当组件被销毁时取消订阅事件
    private void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
        EnvironmentalParaManager.OnEnvironmentalDataChanged -= OnEnvironmentalDataChangedHandler;
    }


}
