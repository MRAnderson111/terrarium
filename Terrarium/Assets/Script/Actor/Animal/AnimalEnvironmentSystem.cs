using System.Collections;
using UnityEngine;

/// <summary>
/// 动物环境适应系统 - 处理环境监测、适应性计算、环境死亡等
/// </summary>
public class AnimalEnvironmentSystem : MonoBehaviour, IAnimalEnvironment
{
    [Header("环境适应性设置")]
    [SerializeField] private float optimalTemperatureMin = 10f; // 适宜温度下限
    [SerializeField] private float optimalTemperatureMax = 35f; // 适宜温度上限
    [SerializeField] private float optimalHumidityMin = 30f;    // 适宜湿度下限
    [SerializeField] private float optimalHumidityMax = 90f;    // 适宜湿度上限
    
    [Header("环境死亡设置")]
    [SerializeField] private float extremeStressThreshold = 0.85f;  // 极端压力阈值
    [SerializeField] private float environmentalDeathTimeLimit = 15f; // 极端环境下存活时间限制
    
    // 环境适应性状态
    private bool isInOptimalEnvironment = true;
    private float environmentalStress = 0f;
    private float extremeStressTimer = 0f;
    private bool isDyingFromEnvironment = false;
    
    // 环境影响的修正系数
    private float environmentalSpeedModifier = 1f;
    private float environmentalReproductionModifier = 1f;
    
    // 缓存环境数据访问，避免重复反射调用
    private static System.Type envFactoryType;
    private static System.Reflection.PropertyInfo temperatureProperty;
    private static System.Reflection.PropertyInfo humidityProperty;
    
    // 属性实现
    public float EnvironmentalStress => environmentalStress;
    public bool IsInOptimalEnvironment => isInOptimalEnvironment;
    public bool IsDyingFromEnvironment => isDyingFromEnvironment;
    public float EnvironmentalSpeedModifier => environmentalSpeedModifier;
    public float EnvironmentalReproductionModifier => environmentalReproductionModifier;
    
    // 事件
    public System.Action OnEnvironmentalStatusChanged;
    public System.Action OnEnvironmentalDeath;
    public System.Action<float> OnStressLevelChanged;
    
    static AnimalEnvironmentSystem()
    {
        try
        {
            envFactoryType = System.Type.GetType("Date_EnvironmentalFactory");
            if (envFactoryType != null)
            {
                temperatureProperty = envFactoryType.GetProperty("Temperature");
                humidityProperty = envFactoryType.GetProperty("Humidity");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"初始化环境数据访问失败: {e.Message}");
        }
    }
    
    void Start()
    {
        // 开始环境监测
        StartCoroutine(MonitorEnvironmentalConditions());
    }
    
    public void CheckEnvironmentalConditions()
    {
        // 如果已经在死亡过程中，不再检查环境条件
        if (isDyingFromEnvironment) return;
        
        // 获取当前环境数据
        float currentTemperature = GetCurrentTemperature();
        float currentHumidity = GetCurrentHumidity();
        
        // 计算环境适应性
        float tempStress = CalculateTemperatureStress(currentTemperature);
        float humidityStress = CalculateHumidityStress(currentHumidity);
        
        // 综合环境压力
        float oldStress = environmentalStress;
        environmentalStress = (tempStress + humidityStress) / 2f;
        
        // 判断是否处于适宜环境
        bool wasOptimal = isInOptimalEnvironment;
        isInOptimalEnvironment = environmentalStress < 0.3f; // 压力小于30%认为是适宜环境
        
        // 计算环境影响修正系数
        CalculateEnvironmentalModifiers();
        
        // 如果环境状态发生变化，触发事件
        if (wasOptimal != isInOptimalEnvironment)
        {
            OnEnvironmentalStatusChanged?.Invoke();
        }
        
        // 如果压力值发生显著变化，触发事件
        if (Mathf.Abs(oldStress - environmentalStress) > 0.1f)
        {
            OnStressLevelChanged?.Invoke(environmentalStress);
        }
        
        // 检查极端环境压力
        if (environmentalStress >= extremeStressThreshold)
        {
            extremeStressTimer += 3f; // 每次检测间隔3秒
            
            if (extremeStressTimer >= environmentalDeathTimeLimit)
            {
                Debug.Log($"动物在极端环境下存活{environmentalDeathTimeLimit}秒后开始死亡");
                StartCoroutine(DieFromEnvironment());
                return;
            }
            else
            {
                Debug.Log($"动物承受极端环境压力 {extremeStressTimer:F1}/{environmentalDeathTimeLimit}秒");
            }
        }
        else
        {
            // 环境改善时重置计时器
            if (extremeStressTimer > 0f)
            {
                extremeStressTimer = 0f;
                Debug.Log("环境条件改善，动物脱离极端压力状态");
            }
        }
        
        Debug.Log($"动物环境检测 - 温度: {currentTemperature:F1}°C, 湿度: {currentHumidity:F1}%, 环境压力: {environmentalStress:F2}");
    }
    
    private IEnumerator MonitorEnvironmentalConditions()
    {
        while (gameObject != null && !isDyingFromEnvironment)
        {
            // 每3秒检测一次环境条件
            yield return new WaitForSeconds(3f);
            
            CheckEnvironmentalConditions();
        }
    }
    
    private float GetCurrentTemperature()
    {
        try
        {
            if (temperatureProperty != null)
                return (float)temperatureProperty.GetValue(null);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取温度数据失败: {e.Message}");
        }
        return 25f; // 默认温度
    }
    
    private float GetCurrentHumidity()
    {
        try
        {
            if (humidityProperty != null)
                return (float)humidityProperty.GetValue(null);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"获取湿度数据失败: {e.Message}");
        }
        return 60f; // 默认湿度
    }
    
    private float CalculateTemperatureStress(float temperature)
    {
        if (temperature >= optimalTemperatureMin && temperature <= optimalTemperatureMax)
            return 0f; // 适宜温度，无压力
        
        if (temperature < optimalTemperatureMin)
        {
            // 温度过低
            float deviation = optimalTemperatureMin - temperature;
            return Mathf.Clamp01(deviation / 20f); // 偏离20度为最大压力
        }
        else
        {
            // 温度过高
            float deviation = temperature - optimalTemperatureMax;
            return Mathf.Clamp01(deviation / 20f); // 偏离20度为最大压力
        }
    }
    
    private float CalculateHumidityStress(float humidity)
    {
        if (humidity >= optimalHumidityMin && humidity <= optimalHumidityMax)
            return 0f; // 适宜湿度，无压力
        
        if (humidity < optimalHumidityMin)
        {
            // 湿度过低
            float deviation = optimalHumidityMin - humidity;
            return Mathf.Clamp01(deviation / 40f); // 偏离40%为最大压力
        }
        else
        {
            // 湿度过高
            float deviation = humidity - optimalHumidityMax;
            return Mathf.Clamp01(deviation / 40f); // 偏离40%为最大压力
        }
    }
    
    private void CalculateEnvironmentalModifiers()
    {
        // 根据环境压力计算速度和繁衍修正系数
        environmentalSpeedModifier = Mathf.Lerp(1f, 0.3f, environmentalStress); // 最低降到30%
        environmentalReproductionModifier = Mathf.Lerp(1f, 0.1f, environmentalStress); // 最低降到10%
        
        Debug.Log($"环境修正系数 - 速度: {environmentalSpeedModifier:F2}, 繁衍: {environmentalReproductionModifier:F2}");
    }
    
    private IEnumerator DieFromEnvironment()
    {
        if (isDyingFromEnvironment) yield break;
        
        isDyingFromEnvironment = true;
        
        Debug.Log("动物开始因极端环境死亡");
        
        // 停止所有行为
        var movement = GetComponent<AnimalMovementSystem>();
        if (movement != null) movement.StopMovement();
        
        // 获取渲染器和材质
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            Material animalMaterial = meshRenderer.material;
            Color originalColor = animalMaterial.color;
            
            // 2秒内逐渐变透明（环境死亡比自然死亡更快）
            float fadeTime = 2f;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeTime);
                
                // 设置透明度，同时颜色变成灰色表示环境死亡
                Color newColor = new Color(0.5f, 0.5f, 0.5f, alpha);
                animalMaterial.color = newColor;
                
                yield return null;
            }
        }
        
        Debug.Log("动物因极端环境死亡");
        OnEnvironmentalDeath?.Invoke();
        
        // 销毁对象
        Destroy(gameObject);
    }
    
    // 公共方法
    public void SetOptimalTemperatureRange(float min, float max)
    {
        optimalTemperatureMin = min;
        optimalTemperatureMax = max;
    }
    
    public void SetOptimalHumidityRange(float min, float max)
    {
        optimalHumidityMin = min;
        optimalHumidityMax = max;
    }
    
    public string GetEnvironmentalStatus()
    {
        if (isDyingFromEnvironment)
            return "环境死亡中";
        else if (environmentalStress >= extremeStressThreshold)
            return "极端压力";
        else if (environmentalStress >= 0.6f)
            return "高压力";
        else if (environmentalStress >= 0.3f)
            return "中等压力";
        else
            return "适宜环境";
    }
}
