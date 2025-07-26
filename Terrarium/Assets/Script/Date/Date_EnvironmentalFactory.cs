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
    [SerializeField] private float sunlightChangeAmount = 5f; // 每次按键改变的阳光强度
    [SerializeField] private float maxLightIntensity = 3f;    // 最大灯光强度
    
    [Header("温度控制设置")]
    [SerializeField] private float temperatureChangeAmount = 5f; // 每次按键改变的温度 (测试阶段调大)
    [SerializeField] private float minTemperature = 0f;         // 最低温度
    [SerializeField] private float maxTemperature = 40f;        // 最高温度
    [SerializeField] private Color warmColor = new Color(1f, 0.5f, 0.2f); // 更明显的暖色温 (橙红色)
    [SerializeField] private Color coldColor = new Color(0.2f, 0.5f, 1f); // 更明显的冷色温 (蓝色)
    
    // 环境因素的属性访问器
    public static float Sunlight { get; private set; } = 50f;
    public static float Humidity { get; private set; } = 50f;
    public static float WaterSource { get; private set; } = 50f;
    public static float Temperature { get; private set; } = 25f;
    
    // 灯光引用
    private Light sunlightSource;
    private Light temperatureLight;
    
    void Start()
    {
        // 查找灯光
        FindSunlightSource();
        FindTemperatureLight();
        
        // 初始化环境数据
        InitializeEnvironmentalData();
        
        Debug.Log($"环境工厂初始化完成 - 阳光: {Sunlight}, 湿度: {Humidity}, 水源: {WaterSource}, 温度: {Temperature}°C");
    }
    
    void FindSunlightSource()
    {
        // 查找场景中名为"Sunlight"的GameObject上的Light组件
        GameObject sunlightObject = GameObject.Find("Sunlight");
        if (sunlightObject != null)
        {
            sunlightSource = sunlightObject.GetComponent<Light>();
            if (sunlightSource != null)
            {
                Debug.Log("找到阳光灯光源，已建立连接");
            }
            else
            {
                Debug.LogWarning("找到名为'Sunlight'的对象，但没有Light组件");
            }
        }
        else
        {
            Debug.LogWarning("未找到名为'Sunlight'的灯光对象");
        }
    }
    
    void FindTemperatureLight()
    {
        Debug.Log("开始查找TemperatureLight对象...");
        
        // 查找场景中名为"TemperatureLight"的GameObject上的Light组件
        GameObject temperatureLightObject = GameObject.Find("TemperatureLight");
        if (temperatureLightObject != null)
        {
            Debug.Log($"找到TemperatureLight对象: {temperatureLightObject.name}");
            temperatureLight = temperatureLightObject.GetComponent<Light>();
            if (temperatureLight != null)
            {
                Debug.Log("找到温度灯光源，已建立连接");
                Debug.Log($"初始灯光颜色: {temperatureLight.color}");
            }
            else
            {
                Debug.LogWarning("找到名为'TemperatureLight'的对象，但没有Light组件");
            }
        }
        else
        {
            Debug.LogWarning("未找到名为'TemperatureLight'的灯光对象");
        }
    }
    
    void InitializeEnvironmentalData()
    {
        // 将Inspector中设置的值同步到静态属性
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
        // 删除按键控制功能
        // HandleSunlightControl();
        // HandleTemperatureControl();
        
        // 实时更新环境数据（如果Inspector中的值发生变化）
        UpdateEnvironmentalData();
    }
    
    // 删除阳光控制方法
    /*
    void HandleSunlightControl()
    {
        // 按Z键增加阳光强度
        if (Input.GetKeyDown(KeyCode.Z))
        {
            sunlight += sunlightChangeAmount;
            sunlight = Mathf.Clamp(sunlight, 0f, 100f);
            Debug.Log($"阳光强度增加到: {sunlight}");
        }
        
        // 按X键减少阳光强度
        if (Input.GetKeyDown(KeyCode.X))
        {
            sunlight -= sunlightChangeAmount;
            sunlight = Mathf.Clamp(sunlight, 0f, 100f);
            Debug.Log($"阳光强度减少到: {sunlight}");
        }
    }
    */
    
    // 删除温度控制方法
    /*
    void HandleTemperatureControl()
    {
        // 按C键增加温度
        if (Input.GetKeyDown(KeyCode.C))
        {
            temperature += temperatureChangeAmount;
            temperature = Mathf.Clamp(temperature, minTemperature, maxTemperature);
            Debug.Log($"温度上升到: {temperature}°C");
        }
        
        // 按V键减少温度
        if (Input.GetKeyDown(KeyCode.V))
        {
            temperature -= temperatureChangeAmount;
            temperature = Mathf.Clamp(temperature, minTemperature, maxTemperature);
            Debug.Log($"温度下降到: {temperature}°C");
        }
    }
    */
    
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
            
            Debug.Log($"环境数据更新 - 阳光: {Sunlight}, 湿度: {Humidity}, 水源: {WaterSource}, 温度: {Temperature}°C");
        }
    }
    
    void UpdateSunlightIntensity()
    {
        if (sunlightSource != null)
        {
            // 将阳光强度(0-100)映射到灯光强度(0-maxLightIntensity)
            float targetIntensity = (Sunlight / 100f) * maxLightIntensity;
            sunlightSource.intensity = targetIntensity;
            
            Debug.Log($"灯光强度更新为: {targetIntensity}");
        }
        else
        {
            Debug.LogWarning("sunlightSource 引用为空，无法更新灯光强度！");
        }
    }
    
    void UpdateTemperatureLight()
    {
        Debug.Log($"UpdateTemperatureLight 被调用，temperatureLight是否为空: {temperatureLight == null}");
        
        if (temperatureLight != null)
        {
            // 将温度映射到色温 (0-40°C 映射到冷色-暖色)
            float normalizedTemp = (Temperature - minTemperature) / (maxTemperature - minTemperature);
            normalizedTemp = Mathf.Clamp01(normalizedTemp);
            
            Debug.Log($"标准化温度值: {normalizedTemp}");
            
            // 插值计算色温颜色
            Color targetColor = Color.Lerp(coldColor, warmColor, normalizedTemp);
            temperatureLight.color = targetColor;
            
            Debug.Log($"温度灯光色温更新为: {targetColor} (温度: {Temperature}°C)");
            Debug.Log($"灯光当前颜色: {temperatureLight.color}");
        }
        else
        {
            Debug.LogWarning("temperatureLight 引用为空，无法更新灯光颜色！");
        }
    }
    
    // 公共方法用于UI更新环境数据
    public static void SetSunlight(float value)
    {
        if (instance != null)
        {
            instance.sunlight = Mathf.Clamp(value, 0f, 100f);
            Sunlight = instance.sunlight;
            instance.UpdateSunlightIntensity();
            Debug.Log($"阳光强度设置为: {value}");
        }
        else
        {
            Debug.LogWarning("Date_EnvironmentalFactory instance 为空！");
        }
    }
    
    public static void SetHumidity(float value)
    {
        if (instance != null)
        {
            instance.humidity = Mathf.Clamp(value, 0f, 100f);
            Humidity = instance.humidity;
        }
    }
    
    public static void SetWaterSource(float value)
    {
        if (instance != null)
        {
            instance.waterSource = Mathf.Clamp(value, 0f, 100f);
            WaterSource = instance.waterSource;
        }
    }
    
    public static void SetTemperature(float value)
    {
        if (instance != null)
        {
            instance.temperature = Mathf.Clamp(value, 0f, 40f);
            Temperature = instance.temperature;
            instance.UpdateTemperatureLight();
        }
    }
    
    // 单例实例
    private static Date_EnvironmentalFactory instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 防止场景切换时被销毁
            Debug.Log("Date_EnvironmentalFactory 单例初始化完成");
        }
        else if (instance != this)
        {
            Debug.LogWarning("发现重复的 Date_EnvironmentalFactory，销毁多余实例");
            Destroy(this); // 只销毁脚本组件，不销毁整个GameObject
            return;
        }
    }
}
