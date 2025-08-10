using UnityEngine;
using System;

/// <summary>
/// UI JSON 按钮生成器工具类，提供静态方法用于从JSON数据生成UI按钮
/// </summary>
public static class UIJsonButtonGenerator
{
    /// <summary>
    /// 解析JSON并生成按钮
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <param name="buttonSections">要生成的按钮配置数组，每个配置包含sectionName和parentGroup</param>
    /// <param name="buttonPrefab">按钮预制体</param>
    /// <param name="buttonSetupAction">设置按钮属性的回调函数</param>
    public static void ParseJsonAndGenerateButtons(string json, ButtonSectionConfig[] buttonSections, GameObject buttonPrefab, Action<GameObject, string, int> buttonSetupAction)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("UIJsonButtonGenerator: JSON字符串不能为空");
            return;
        }
        
        if (buttonSections == null || buttonSections.Length == 0)
        {
            Debug.LogError("UIJsonButtonGenerator: 按钮配置数组不能为空");
            return;
        }
        
        if (buttonPrefab == null)
        {
            Debug.LogError("UIJsonButtonGenerator: 按钮预制体不能为空");
            return;
        }
        
        try
        {
            foreach (var section in buttonSections)
            {
                GenerateButtonsFromJsonSection(json, section.SectionName, section.ParentGroup, buttonPrefab, buttonSetupAction);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"UIJsonButtonGenerator: JSON解析错误: {e.Message}");
        }
    }
    
    /// <summary>
    /// 从JSON的特定部分生成按钮
    /// </summary>
    /// <param name="json">完整JSON字符串</param>
    /// <param name="sectionName">部分名称（如"Plant"、"Animal"、"Killer"）</param>
    /// <param name="parentGroup">父组对象</param>
    /// <param name="buttonPrefab">按钮预制体</param>
    /// <param name="buttonSetupAction">设置按钮属性的回调函数</param>
    public static void GenerateButtonsFromJsonSection(string json, string sectionName, GameObject parentGroup, GameObject buttonPrefab, Action<GameObject, string, int> buttonSetupAction)
    {
        if (string.IsNullOrEmpty(sectionName))
        {
            Debug.LogError("UIJsonButtonGenerator: 部分名称不能为空");
            return;
        }
        
        if (parentGroup == null)
        {
            Debug.LogWarning($"UIJsonButtonGenerator: 父组为空，跳过{sectionName}按钮生成");
            return;
        }
        
        if (buttonPrefab == null)
        {
            Debug.LogError("UIJsonButtonGenerator: 按钮预制体不能为空");
            return;
        }
        
        if (buttonSetupAction == null)
        {
            Debug.LogError("UIJsonButtonGenerator: 按钮设置回调函数不能为空");
            return;
        }
        
        // 查找部分开始和结束位置
        string sectionStart = $"\"{sectionName}\": {{";
        int startIndex = json.IndexOf(sectionStart);
        if (startIndex == -1)
        {
            Debug.LogWarning($"UIJsonButtonGenerator: 在JSON中未找到{sectionName}部分");
            return;
        }
        
        startIndex += sectionStart.Length - 1; // 调整到花括号位置
        
        // 查找部分结束位置（下一个}或整个JSON结束）
        int endIndex = json.IndexOf('}', startIndex);
        if (endIndex == -1)
        {
            Debug.LogWarning($"UIJsonButtonGenerator: 在JSON中未找到{sectionName}部分的结束");
            return;
        }
        
        // 提取部分内容
        string sectionContent = json.Substring(startIndex + 1, endIndex - startIndex - 1);
        
        // 解析键值对
        string[] pairs = sectionContent.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (keyValue.Length >= 2)
            {
                string objectName = keyValue[0].Trim().Trim('"', ' ');
                string quantityStr = keyValue[1].Trim();
                
                if (int.TryParse(quantityStr, out int quantity))
                {
                    // 实例化按钮预制体
                    GameObject buttonObj = GameObject.Instantiate(buttonPrefab, parentGroup.transform);
                    buttonObj.SetActive(true);
                    
                    // 调用回调函数设置按钮属性
                    buttonSetupAction(buttonObj, objectName, quantity);
                }
                else
                {
                    Debug.LogWarning($"UIJsonButtonGenerator: 无法解析数量: {quantityStr}");
                }
            }
        }
    }
    
    /// <summary>
    /// 按钮配置结构
    /// </summary>
    public struct ButtonSectionConfig
    {
        /// <summary>
        /// JSON中的部分名称
        /// </summary>
        public string SectionName;
        
        /// <summary>
        /// 按钮的父组对象
        /// </summary>
        public GameObject ParentGroup;
    }
}