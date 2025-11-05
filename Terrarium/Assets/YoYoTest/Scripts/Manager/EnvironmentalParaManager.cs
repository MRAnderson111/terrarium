using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Sunshine, temperature, humidity
[System.Serializable]
public struct EnvironmentalData
{
    public float sunshine;
    public float temperature;
    public float humidity;
}

public class EnvironmentalParaManager : MonoBehaviour
{
    // 单例实例
    private static EnvironmentalParaManager instance;
    
    // 公开的静态属性，用于访问单例实例
    public static EnvironmentalParaManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 查找场景中已存在的实例
                instance = FindObjectOfType<EnvironmentalParaManager>();
                
                // 如果场景中没有实例，创建一个新对象
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("EnvironmentalParaManager");
                    instance = singletonObject.AddComponent<EnvironmentalParaManager>();
                }
            }
            return instance;
        }
    }

    // 环境数据变化事件
    public static event System.Action<EnvironmentalData> OnEnvironmentalDataChanged;

    public List<Light> environmentalDataChangedListeners = new List<Light>();
    
    [Header("灯光控制参数")]
    [SerializeField]
    private float minColorTemperature = 1000f; // 色温最小值 (K)
    [SerializeField]
    private float maxColorTemperature = 10000f; // 色温最大值 (K)
    [SerializeField]
    private float minLightIntensity = 0f; // 光照强度最小值
    [SerializeField]
    private float maxLightIntensity = 5f; // 光照强度最大值
    
    // 公开属性，用于访问色温范围
    public float MinColorTemperature
    {
        get { return minColorTemperature; }
        set { minColorTemperature = value; UpdateLightingFromEnvironmentalData(); }
    }
    
    public float MaxColorTemperature
    {
        get { return maxColorTemperature; }
        set { maxColorTemperature = value; UpdateLightingFromEnvironmentalData(); }
    }
    
    // 公开属性，用于访问光照强度范围
    public float MinLightIntensity
    {
        get { return minLightIntensity; }
        set { minLightIntensity = value; UpdateLightingFromEnvironmentalData(); }
    }
    
    public float MaxLightIntensity
    {
        get { return maxLightIntensity; }
        set { maxLightIntensity = value; UpdateLightingFromEnvironmentalData(); }
    }

    // 公开的环境数据属性，可以通过单例访问
    public EnvironmentalData EnvironmentalData
    {
        get { return environmentalData; }
        set
        {
            if (environmentalData.sunshine != value.sunshine ||
                environmentalData.temperature != value.temperature ||
                environmentalData.humidity != value.humidity)
            {
                environmentalData = value;
                OnEnvironmentalDataChanged?.Invoke(environmentalData);
                UpdateLightingFromEnvironmentalData();
            }
        }
    }
    
    // 公开的属性，可以直接访问和修改环境参数
    public float Sunshine
    {
        get { return environmentalData.sunshine; }
        set
        {
            if (environmentalData.sunshine != value)
            {
                EnvironmentalData newEnvData = environmentalData;
                newEnvData.sunshine = value;
                EnvironmentalData = newEnvData;
            }
        }
    }
    
    public float Temperature
    {
        get { return environmentalData.temperature; }
        set
        {
            if (environmentalData.temperature != value)
            {
                EnvironmentalData newEnvData = environmentalData;
                newEnvData.temperature = value;
                EnvironmentalData = newEnvData;
            }
        }
    }
    
    public float Humidity
    {
        get { return environmentalData.humidity; }
        set
        {
            if (environmentalData.humidity != value)
            {
                EnvironmentalData newEnvData = environmentalData;
                newEnvData.humidity = value;
                EnvironmentalData = newEnvData;
            }
        }
    }
    
    [SerializeField]
    private EnvironmentalData environmentalData = new EnvironmentalData();
    
    // Awake方法在Start之前调用，用于初始化单例
    void Awake()
    {
        // 如果已经存在实例，销毁当前对象
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // 设置当前实例
        instance = this;
        
        // 确保对象在场景切换时不会被销毁
        DontDestroyOnLoad(gameObject);
        
        // 初始化时更新所有灯光
        UpdateLightingFromEnvironmentalData();
    }

    // 根据环境数据更新灯光属性
    private void UpdateLightingFromEnvironmentalData()
    {
        foreach (Light light in environmentalDataChangedListeners)
        {
            if (light != null)
            {
                // 根据温度参数计算色温 (假设温度范围0-1映射到色温范围)
                float normalizedTemperature = Mathf.Clamp01(environmentalData.temperature);
                float colorTemperature = Mathf.Lerp(minColorTemperature, maxColorTemperature, normalizedTemperature);
                
                // 根据光照参数计算光照强度 (假设光照范围0-1映射到强度范围)
                float normalizedSunshine = Mathf.Clamp01(environmentalData.sunshine);
                float lightIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, normalizedSunshine);
                
                // 应用灯光属性
                light.colorTemperature = colorTemperature;
                light.intensity = lightIntensity;
            }
        }
    }
    
    // 添加灯光到监听列表
    public void AddLightListener(Light light)
    {
        if (light != null && !environmentalDataChangedListeners.Contains(light))
        {
            environmentalDataChangedListeners.Add(light);
            // 立即应用当前环境数据到新添加的灯光
            UpdateSingleLight(light);
        }
    }
    
    // 从监听列表移除灯光
    public void RemoveLightListener(Light light)
    {
        if (light != null && environmentalDataChangedListeners.Contains(light))
        {
            environmentalDataChangedListeners.Remove(light);
        }
    }
    
    // 更新单个灯光的属性
    private void UpdateSingleLight(Light light)
    {
        if (light != null)
        {
            // 根据温度参数计算色温
            float normalizedTemperature = Mathf.Clamp01(environmentalData.temperature);
            float colorTemperature = Mathf.Lerp(minColorTemperature, maxColorTemperature, normalizedTemperature);
            
            // 根据光照参数计算光照强度
            float normalizedSunshine = Mathf.Clamp01(environmentalData.sunshine);
            float lightIntensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, normalizedSunshine);
            
            // 应用灯光属性
            light.colorTemperature = colorTemperature;
            light.intensity = lightIntensity;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // 数据更新现在通过事件处理
    }
}
