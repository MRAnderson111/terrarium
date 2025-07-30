using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Date_EnvironmentalFactory : MonoBehaviour
{
    [Header("拟真自然环境四要素")]
    [SerializeField] private float sunlight = 50f;     // 阳光强度 (0-100)
    [SerializeField] private float humidity = 50f;     // 湿度 (0-100)
    [SerializeField] private float waterSource = 50f;  // 水源充足度 (0-100)
    [SerializeField] private float temperature = 25f;  // 温度 (摄氏度)
    
    [Header("阳光控制设置")]
    [SerializeField] private float maxLightIntensity = 3f;    // 最大灯光强度
    
    [Header("温度控制设置")]
    [SerializeField] private float minTemperature = 0f;         // 最低温度
    [SerializeField] private float maxTemperature = 40f;        // 最高温度
    [SerializeField] private Color warmColor = new Color(1f, 0.5f, 0.2f); // 更明显的暖色温 (橙红色)
    [SerializeField] private Color coldColor = new Color(0.2f, 0.5f, 1f); // 更明显的冷色温 (蓝色)
    
    // 环境因素的属性访问器
    public static float Sunlight = 50f;
    public static float Humidity = 50f;
    public static float WaterSource = 50f;
    public static float Temperature = 25f;
    
    public GameObject sunlightObject;
    public GameObject temperatureLightObject;
    public GameObject waterSourceObject;
    
    // 添加静态引用
    private static Date_EnvironmentalFactory instance;

    void Start()
    {
        // 设置单例引用
        instance = this;
        
        // 初始化环境数据
        InitializeEnvironmentalData();
        
        Debug.Log($"环境工厂初始化完成 - 阳光: {Sunlight}, 湿度: {Humidity}, 水源: {WaterSource}, 温度: {Temperature}°C");
    }
    
    void InitializeEnvironmentalData()
    {
        // 将Inspector中设置的值同到静态属性
        Sunlight = sunlight;
        Humidity = humidity;
        WaterSource = waterSource;
        Temperature = temperature;
        
        // 初始化灯光
        UpdateSunlightIntensity();
        UpdateTemperatureLight();
    }
    
    void Update()
    {
        UpdateEnvironmentalData();
    }
    
    
    void UpdateEnvironmentalData()
    {
        // 检查是否有变化并更新
        if (Sunlight != sunlight || Humidity != humidity || 
            WaterSource != waterSource || Temperature != temperature)
        {
            Sunlight = sunlight;
            Humidity = humidity;
            WaterSource = waterSource;
            Temperature = temperature;
            
            // 更新灯光
            UpdateSunlightIntensity();
            UpdateTemperatureLight();

        }
    }
    
    void UpdateSunlightIntensity()
    {
        if (sunlightObject != null)
        {
            // 将阳光强度(0-100)映射到灯光强度(0-maxLightIntensity)
            float targetIntensity = (Sunlight / 100f) * maxLightIntensity;
            sunlightObject.GetComponent<Light>().intensity = targetIntensity;
        }
    }
    
    void UpdateTemperatureLight()
    {     
        if (temperatureLightObject != null)
        {
            // 将温度映射到色温 (0-40°C 映射到冷色-暖色)
            float normalizedTemp = (Temperature - minTemperature) / (maxTemperature - minTemperature);
            normalizedTemp = Mathf.Clamp01(normalizedTemp);
            
            // 插值计算色温颜色
            Color targetColor = Color.Lerp(coldColor, warmColor, normalizedTemp);
            temperatureLightObject.GetComponent<Light>().color = targetColor;

        }
    }
    

    // 公共方法用于UI更新环境数据
    public static void SetSunlight(float value)
    {
        if (instance != null && instance.sunlightObject != null)
        {
            float targetIntensity = (value / 100f) * instance.maxLightIntensity;
            instance.sunlightObject.GetComponent<Light>().intensity = targetIntensity;
            instance.sunlight = value;
            instance.UpdateSunlightIntensity();
        }
    }
    
    public static void SetHumidity(float value)
    {
        Humidity = Mathf.Clamp(value, 0f, 100f);
        if (instance != null)
        {
            instance.humidity = Humidity;
        }
    }
    
    public static void SetWaterSource(float value)
    {
        if (instance != null && instance.waterSourceObject != null)
        {
            instance.waterSourceObject.GetComponent<Light>().intensity = Mathf.Clamp(value, 0f, 100f);
            WaterSource = value;
        }
    }
    
    public static void SetTemperature(float value)
    {
        if (instance != null && instance.temperatureLightObject != null)
        {
            Temperature = Mathf.Clamp(value, instance.minTemperature, instance.maxTemperature);
            instance.temperature = Temperature;
            instance.UpdateTemperatureLight();
        }
    }
}
