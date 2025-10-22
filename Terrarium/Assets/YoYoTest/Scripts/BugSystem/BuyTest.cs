using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyTest : MonoBehaviour
{
    // 存档键名常量
    private const string COINS_KEY = "PlayerCoins";
    private const string ITEMS_PREFIX = "Item_";
    
    // 默认金币数量
    private const int DEFAULT_COINS = 500;
    
    // 物品列表（可以在这里添加更多物品）
    public List<string> itemNames = new List<string>
    {
        "苹果", "香蕉", "橙子", "葡萄", "西瓜",
        "草莓", "蓝莓", "芒果", "菠萝", "樱桃"
    };
    
    // 当前金币数量
    private int currentCoins;
    
    // 物品字典，存储物品名称和数量
    private Dictionary<string, int> itemsDictionary;
    
    void Start()
    {
        // 初始化物品字典
        itemsDictionary = new Dictionary<string, int>();
        
        // 加载存档数据
        LoadGameData();
        
        // 如果是第一次运行，初始化所有物品数量为0
        if (!PlayerPrefs.HasKey(COINS_KEY))
        {
            InitializeDefaultData();
        }
        
        // 打印当前存档信息（用于测试）
        Debug.Log("当前金币数量: " + currentCoins);
        Debug.Log("物品库存:");
        foreach (var item in itemsDictionary)
        {
            Debug.Log(item.Key + ": " + item.Value);
        }
    }
    
    void Update()
    {
        // 测试按键（可以删除这些，仅用于演示）
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddCoins(10);
            Debug.Log("增加了10个金币，当前金币: " + currentCoins);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddItem("苹果", 1);
            Debug.Log("增加了1个苹果，当前苹果数量: " + GetItemCount("苹果"));
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RemoveItem("苹果", 1);
            Debug.Log("减少了1个苹果，当前苹果数量: " + GetItemCount("苹果"));
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGameData();
            Debug.Log("游戏数据已重置");
        }
    }
    
    /// <summary>
    /// 初始化默认游戏数据
    /// </summary>
    private void InitializeDefaultData()
    {
        // 设置默认金币数量
        currentCoins = DEFAULT_COINS;
        PlayerPrefs.SetInt(COINS_KEY, currentCoins);
        
        // 初始化所有物品数量为0
        foreach (string itemName in itemNames)
        {
            itemsDictionary[itemName] = 0;
            PlayerPrefs.SetInt(ITEMS_PREFIX + itemName, 0);
        }
        
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 加载游戏数据
    /// </summary>
    private void LoadGameData()
    {
        // 加载金币数量
        currentCoins = PlayerPrefs.GetInt(COINS_KEY, DEFAULT_COINS);
        
        // 加载物品数量
        foreach (string itemName in itemNames)
        {
            int itemCount = PlayerPrefs.GetInt(ITEMS_PREFIX + itemName, 0);
            itemsDictionary[itemName] = itemCount;
        }
    }
    
    /// <summary>
    /// 保存游戏数据
    /// </summary>
    private void SaveGameData()
    {
        // 保存金币数量
        PlayerPrefs.SetInt(COINS_KEY, currentCoins);
        
        // 保存物品数量
        foreach (var item in itemsDictionary)
        {
            PlayerPrefs.SetInt(ITEMS_PREFIX + item.Key, item.Value);
        }
        
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 增加金币
    /// </summary>
    /// <param name="amount">增加的数量</param>
    public void AddCoins(int amount)
    {
        if (amount > 0)
        {
            currentCoins += amount;
            SaveGameData();
        }
    }
    
    /// <summary>
    /// 减少金币
    /// </summary>
    /// <param name="amount">减少的数量</param>
    /// <returns>是否成功减少（金币是否足够）</returns>
    public bool SpendCoins(int amount)
    {
        if (amount > 0 && currentCoins >= amount)
        {
            currentCoins -= amount;
            SaveGameData();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 获取当前金币数量
    /// </summary>
    /// <returns>当前金币数量</returns>
    public int GetCoins()
    {
        return currentCoins;
    }
    
    /// <summary>
    /// 添加物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <param name="amount">添加数量</param>
    public void AddItem(string itemName, int amount)
    {
        if (amount > 0 && itemsDictionary.ContainsKey(itemName))
        {
            itemsDictionary[itemName] += amount;
            SaveGameData();
        }
        else if (!itemsDictionary.ContainsKey(itemName))
        {
            Debug.LogWarning("物品 '" + itemName + "' 不存在于物品列表中");
        }
    }
    
    /// <summary>
    /// 移除物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <param name="amount">移除数量</param>
    /// <returns>是否成功移除（物品数量是否足够）</returns>
    public bool RemoveItem(string itemName, int amount)
    {
        if (amount > 0 && itemsDictionary.ContainsKey(itemName))
        {
            if (itemsDictionary[itemName] >= amount)
            {
                itemsDictionary[itemName] -= amount;
                SaveGameData();
                return true;
            }
        }
        else if (!itemsDictionary.ContainsKey(itemName))
        {
            Debug.LogWarning("物品 '" + itemName + "' 不存在于物品列表中");
        }
        return false;
    }
    
    /// <summary>
    /// 获取物品数量
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>物品数量</returns>
    public int GetItemCount(string itemName)
    {
        if (itemsDictionary.ContainsKey(itemName))
        {
            return itemsDictionary[itemName];
        }
        Debug.LogWarning("物品 '" + itemName + "' 不存在于物品列表中");
        return 0;
    }
    
    /// <summary>
    /// 添加新物品类型
    /// </summary>
    /// <param name="itemName">新物品名称</param>
    public void AddNewItem(string itemName)
    {
        if (!itemsDictionary.ContainsKey(itemName))
        {
            itemNames.Add(itemName);
            itemsDictionary[itemName] = 0;
            PlayerPrefs.SetInt(ITEMS_PREFIX + itemName, 0);
            PlayerPrefs.Save();
            Debug.Log("已添加新物品: " + itemName);
        }
        else
        {
            Debug.LogWarning("物品 '" + itemName + "' 已存在");
        }
    }
    
    /// <summary>
    /// 获取所有物品的列表
    /// </summary>
    /// <returns>物品字典</returns>
    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(itemsDictionary);
    }
    
    /// <summary>
    /// 重置所有游戏数据
    /// </summary>
    public void ResetGameData()
    {
        PlayerPrefs.DeleteKey(COINS_KEY);
        
        foreach (string itemName in itemNames)
        {
            PlayerPrefs.DeleteKey(ITEMS_PREFIX + itemName);
        }
        
        PlayerPrefs.Save();
        
        // 重新初始化
        InitializeDefaultData();
    }
}
