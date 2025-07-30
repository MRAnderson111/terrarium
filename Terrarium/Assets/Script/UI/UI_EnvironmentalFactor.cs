using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_EnvironmentalFactor : MonoBehaviour
{
    [Header("环境因素滑块")]
    [SerializeField] private Slider sunlightSlider;
    [SerializeField] private Slider humiditySlider;
    [SerializeField] private Slider temperatureSlider;
    
    [Header("显示文本")]
    [SerializeField] private TextMeshProUGUI sunlightText;
    [SerializeField] private TextMeshProUGUI humidityText;
    [SerializeField] private TextMeshProUGUI waterSourceText;
    [SerializeField] private TextMeshProUGUI temperatureText;
    
    void Start()
    {
        InitializeSliders();
        SetupSliderListeners();
    }
    
    void InitializeSliders()
    {
        // 设置滑块范围和初始值
        if (sunlightSlider != null)
        {
            sunlightSlider.minValue = 0f;
            sunlightSlider.maxValue = 100f;
            sunlightSlider.value = Date_EnvironmentalFactory.Sunlight;
        }
        
        if (humiditySlider != null)
        {
            humiditySlider.minValue = 0f;
            humiditySlider.maxValue = 100f;
            humiditySlider.value = Date_EnvironmentalFactory.Humidity;
        }
        
        if (temperatureSlider != null)
        {
            temperatureSlider.minValue = 0f;
            temperatureSlider.maxValue = 40f;
            temperatureSlider.value = Date_EnvironmentalFactory.Temperature;
        }
        
        // 初始化文本显示
        UpdateAllTexts();
    }
    
    void SetupSliderListeners()
    {
        // 添加滑块值变化监听器
        if (sunlightSlider != null)
            sunlightSlider.onValueChanged.AddListener(OnSunlightChanged);
            
        if (humiditySlider != null)
            humiditySlider.onValueChanged.AddListener(OnHumidityChanged);
            
        if (temperatureSlider != null)
            temperatureSlider.onValueChanged.AddListener(OnTemperatureChanged);
    }
    
    void OnSunlightChanged(float value)
    {
        Date_EnvironmentalFactory.SetSunlight(value);
        if (sunlightText != null)
            sunlightText.text = $"阳光强度: {value:F0}";
    }
    
    void OnHumidityChanged(float value)
    {
        Date_EnvironmentalFactory.SetHumidity(value);
        if (humidityText != null)
            humidityText.text = $"湿度: {value:F0}%";
    }
    
    void OnWaterSourceChanged(float value)
    {
        Date_EnvironmentalFactory.SetWaterSource(value);
        if (waterSourceText != null)
        {
            // 检查场景中是否有名为"Water"的对象
            GameObject waterObject = GameObject.Find("Water");
            if (waterObject != null)
            {
                waterSourceText.text = "水源: 充足";
            }
            else
            {
                waterSourceText.text = $"水源: {value:F0}%";
            }
        }
    }
    
    void OnTemperatureChanged(float value)
    {
        Date_EnvironmentalFactory.SetTemperature(value);
        if (temperatureText != null)
            temperatureText.text = $"温度: {value:F0}°C";
    }
    
    void Update()
    {
        
    }
    
    void UpdateAllTexts()
    {
        if (sunlightText != null)
            sunlightText.text = $"阳光强度: {Date_EnvironmentalFactory.Sunlight:F0}";
            
        if (humidityText != null)
            humidityText.text = $"湿度: {Date_EnvironmentalFactory.Humidity:F0}%";
            
        if (waterSourceText != null)
        {
            // 检查场景中是否有名为"Water"的对象
            GameObject waterObject = GameObject.Find("Water");
            if (waterObject != null)
            {
                waterSourceText.text = "水源: 充足";
            }
            else
            {
                waterSourceText.text = $"水源: {Date_EnvironmentalFactory.WaterSource:F0}%";
            }
        }
        
        if (temperatureText != null)
            temperatureText.text = $"温度: {Date_EnvironmentalFactory.Temperature:F0}°C";
    }
}
