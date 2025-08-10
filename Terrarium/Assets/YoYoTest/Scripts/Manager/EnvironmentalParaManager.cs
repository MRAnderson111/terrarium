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
    }

    // Update is called once per frame
    void Update()
    {
        // 数据更新现在通过事件处理
    }
}
