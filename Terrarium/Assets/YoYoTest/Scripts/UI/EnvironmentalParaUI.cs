using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentalParaUI : MonoBehaviour
{
    public Slider sunshineSlider;
    public Slider temperatureSlider;
    public Slider humiditySlider;
    
    // Start is called before the first frame update
    void Start()
    {
        if (sunshineSlider == null || temperatureSlider == null || humiditySlider == null)
        {
            Debug.LogWarning("环境参数界面的滑条没有绑定，已跳过初始化", this);
            return;
        }

        UpdateSlidersFromData();
        EnvironmentalParaManager.OnEnvironmentalDataChanged += OnEnvironmentalDataChanged;
        sunshineSlider.onValueChanged.AddListener(OnSunshineChanged);
        temperatureSlider.onValueChanged.AddListener(OnTemperatureChanged);
        humiditySlider.onValueChanged.AddListener(OnHumidityChanged);
    }
    
    // 当组件被销毁时，取消订阅事件和移除监听器
    void OnDestroy()
    {
        EnvironmentalParaManager.OnEnvironmentalDataChanged -= OnEnvironmentalDataChanged;
        if (sunshineSlider != null)
            sunshineSlider.onValueChanged.RemoveListener(OnSunshineChanged);
        if (temperatureSlider != null)
            temperatureSlider.onValueChanged.RemoveListener(OnTemperatureChanged);
        if (humiditySlider != null)
            humiditySlider.onValueChanged.RemoveListener(OnHumidityChanged);
    }
    
    // 根据环境数据更新Slider的值
    void UpdateSlidersFromData()
    {
        sunshineSlider.value = EnvironmentalParaManager.Instance.EnvironmentalData.sunshine;
        temperatureSlider.value = EnvironmentalParaManager.Instance.EnvironmentalData.temperature;
        humiditySlider.value = EnvironmentalParaManager.Instance.EnvironmentalData.humidity;
    }
    
    // 当环境数据改变时的回调
    void OnEnvironmentalDataChanged(EnvironmentalData envData)
    {
        UpdateSlidersFromData();
    }
    
    // 当阳光值改变时的回调
    void OnSunshineChanged(float value)
    {
        EnvironmentalParaManager.Instance.Sunshine = value;
    }
    
    // 当温度值改变时的回调
    void OnTemperatureChanged(float value)
    {
        EnvironmentalParaManager.Instance.Temperature = value;
    }
    
    // 当湿度值改变时的回调
    void OnHumidityChanged(float value)
    {
        EnvironmentalParaManager.Instance.Humidity = value;
    }
}
