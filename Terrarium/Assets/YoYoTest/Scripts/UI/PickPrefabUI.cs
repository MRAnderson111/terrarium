using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PickPrefabUI : MonoBehaviour
{
    // 分类统计数据，参考test.json格式
    private Dictionary<string, Dictionary<string, int>> classData = new Dictionary<string, Dictionary<string, int>>();
    
    // 单例模式，方便其他脚本访问
    public static PickPrefabUI Instance { get; private set; }
    
    private void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // 初始化分类数据
        InitializeClassData();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(GetClassDataJson());
        }
    }
    
    // 初始化分类数据结构
    private void InitializeClassData()
    {
        // 从test.json加载初始数据，或者使用默认数据
        classData = new Dictionary<string, Dictionary<string, int>>
        {
            { "Plant", new Dictionary<string, int> { { "Grass", 0 }, { "Flower", 0 }, { "Tree", 0 } } },
            { "Animal", new Dictionary<string, int> { { "BigAnt", 0 } } },
            { "Killer", new Dictionary<string, int> { { "AntKiller", 0 } } }
        };
    }
    
    // 添加预制体到分类统计中
    public void AddPrefabToClass(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("传入的prefab为空");
            return;
        }
        
        IGetObjectClass objectClass = prefab.GetComponent<IGetObjectClass>();
        if (objectClass == null)
        {
            Debug.LogWarning($"预制体 {prefab.name} 没有实现IGetObjectClass接口");
            return;
        }
        
        string bigClass = objectClass.BigClass;
        string smallClass = objectClass.SmallClass;
        
        // 更新统计数据
        if (!classData.ContainsKey(bigClass))
        {
            // 如果大类不存在，创建新的大类字典
            classData[bigClass] = new Dictionary<string, int>();
        }
        
        if (!classData[bigClass].ContainsKey(smallClass))
        {
            // 如果小类不存在，初始化为0
            classData[bigClass][smallClass] = 0;
        }
        
        // 增加计数
        classData[bigClass][smallClass]++;
        
        Debug.Log($"添加预制体: {prefab.name} -> 大类: {bigClass}, 小类: {smallClass}, 当前数量: {classData[bigClass][smallClass]}");
        
        // 数据更新后，同步到YoYoGameManager
        SyncDataToGameManager();
    }
    
    // 减少预制体计数（当预制体被销毁时调用）
    public void RemovePrefabFromClass(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("传入的prefab为空");
            return;
        }
        
        IGetObjectClass objectClass = prefab.GetComponent<IGetObjectClass>();
        if (objectClass == null)
        {
            Debug.LogWarning($"预制体 {prefab.name} 没有实现IGetObjectClass接口");
            return;
        }
        
        string bigClass = objectClass.BigClass;
        string smallClass = objectClass.SmallClass;
        
        // 减少计数
        if (classData.ContainsKey(bigClass) && classData[bigClass].ContainsKey(smallClass))
        {
            classData[bigClass][smallClass]--;
            if (classData[bigClass][smallClass] < 0)
            {
                classData[bigClass][smallClass] = 0; // 确保计数不为负数
            }
            
            Debug.Log($"移除预制体: {prefab.name} -> 大类: {bigClass}, 小类: {smallClass}, 当前数量: {classData[bigClass][smallClass]}");
            
            // 数据更新后，同步到YoYoGameManager
            SyncDataToGameManager();
        }
    }
    
    // 获取分类数据的JSON字符串
    public string GetClassDataJson()
    {
        // 将Dictionary转换为JSON格式
        System.Text.StringBuilder jsonBuilder = new System.Text.StringBuilder();
        jsonBuilder.Append("{\n");
        
        bool firstBigClass = true;
        foreach (var bigClassPair in classData)
        {
            if (!firstBigClass)
            {
                jsonBuilder.Append(",\n");
            }
            
            jsonBuilder.Append($"    \"{bigClassPair.Key}\": {{\n");
            
            bool firstSmallClass = true;
            foreach (var smallClassPair in bigClassPair.Value)
            {
                if (!firstSmallClass)
                {
                    jsonBuilder.Append(",\n");
                }
                
                jsonBuilder.Append($"        \"{smallClassPair.Key}\": {smallClassPair.Value}");
                firstSmallClass = false;
            }
            
            jsonBuilder.Append("\n    }");
            firstBigClass = false;
        }
        
        jsonBuilder.Append("\n}");
        
        return jsonBuilder.ToString();
    }
    
    // 获取特定大类的所有小类计数
    public Dictionary<string, int> GetSmallClassesByBigClass(string bigClass)
    {
        if (classData.ContainsKey(bigClass))
        {
            return new Dictionary<string, int>(classData[bigClass]);
        }
        return new Dictionary<string, int>();
    }
    
    // 获取特定大类的特定小类计数
    public int GetSmallClassCount(string bigClass, string smallClass)
    {
        if (classData.ContainsKey(bigClass) && classData[bigClass].ContainsKey(smallClass))
        {
            return classData[bigClass][smallClass];
        }
        return 0;
    }
    
    // 重置所有计数
    public void ResetAllCounts()
    {
        foreach (var bigClassPair in classData)
        {
            foreach (var smallClassPair in bigClassPair.Value.Keys.ToList())
            {
                classData[bigClassPair.Key][smallClassPair] = 0;
            }
        }
        Debug.Log("所有分类计数已重置");
        
        // 数据重置后，同步到YoYoGameManager
        SyncDataToGameManager();
    }
    
    // 同步数据到YoYoGameManager
    private void SyncDataToGameManager()
    {
        try
        {
            // 获取当前分类数据的JSON字符串
            string jsonData = GetClassDataJson();
            
            // 同步到YoYoGameManager
            if (YoYoGameManager.Instance != null)
            {
                YoYoGameManager.Instance.UpdateJsonData(jsonData);
            }
            else
            {
                Debug.LogWarning("YoYoGameManager实例不存在，无法同步数据");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"同步数据到YoYoGameManager时发生错误: {e.Message}");
        }
    }
    
    // 手动同步数据到YoYoGameManager（公共方法）
    public void ForceSyncToGameManager()
    {
        SyncDataToGameManager();
        Debug.Log("手动同步分类数据到YoYoGameManager完成");
    }
}
