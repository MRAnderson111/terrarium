using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象统计管理器：负责追踪游戏中各类对象的生成和销毁事件，并维护其数量统计。
/// </summary>
public class ObjectStatisticsManager : MonoBehaviour
{
#region 单例模式
    public static ObjectStatisticsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
#endregion

#region 字段和属性
    /// <summary>
    /// 存储大类对象的数量统计，键为BigClass，值为该大类对象的总数量。
    /// </summary>
    public Dictionary<string, int> bigClassCount = new();

    /// <summary>
    /// 存储小类对象的数量统计，键为SmallClass，值为该小类对象的数量。
    /// </summary>
    public Dictionary<string, int> smallClassCount = new();

    /// <summary>
    /// 存储全局冷却时间，键为SmallClass，值为冷却时间。
    /// </summary>
    public Dictionary<string, float> globalCoolDown = new();

    /// <summary>
    /// 存储所有植物的总血量
    /// </summary>
    public float totalPlantHealth = 0f;

    /// <summary>
    /// 植物对象的缓存列表，避免每帧都重新查找
    /// </summary>
    private List<GameObject> plantObjectsCache = new List<GameObject>();

    /// <summary>
    /// 植物IBeHurt接口的缓存列表，避免每帧都重新获取组件
    /// </summary>
    private List<IBeHurt> plantBeHurtCache = new List<IBeHurt>();

    /// <summary>
    /// 标记缓存是否需要更新
    /// </summary>
    private bool plantCacheDirty = true;
#endregion

#region Unity 生命周期方法
    void Start()
    {
        // 注册对象生成和销毁事件的监听器
        Events.OnCreateObject.AddListener(OnCreateObject);
        Events.OnDestroyObject.AddListener(OnDestroyObject);
    }

    /// <summary>
    /// 每帧增加冷却时间
    /// </summary>
    void Update()
    {
        // 检查全局冷却时间字典是否为空，不为空才遍历增加时间
        if (globalCoolDown.Count > 0)
        {
            // 先获取所有键，避免在遍历过程中修改字典
            var keys = new List<string>(globalCoolDown.Keys);
            
            // 遍历键列表，增加冷却时间
            foreach (var key in keys)
            {
                // 增加冷却时间
                globalCoolDown[key] += Time.deltaTime;
            }
        }

        
        // 计算所有植物的总血量
        CalculateTotalPlantHealth();
        

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     // 检查字典中是否包含 "Grass" 键
        //     if (globalCoolDown.ContainsKey("Grass"))
        //     {
        //         Debug.LogError("全局冷却时间：" + globalCoolDown["Grass"]);
        //     }
        //     else
        //     {
        //         Debug.LogError("Grass 不在全局冷却时间字典中");
        //     }
        // }
    }
#endregion

#region 事件处理方法
    /// <summary>
    /// 处理对象生成事件。当有新对象生成时，更新BigClass和SmallClass的统计数量。
    /// </summary>
    /// <param name="arg0">实现了 IGetObjectClass 接口的新生成对象。</param>
    private void OnCreateObject(IGetObjectClass arg0)
    {
        Debug.Log("生成物种：" + arg0.BigClass + " " + arg0.SmallClass);

        // // 检查这个小类是否已经在全局冷却时间中
        // if (globalCoolDown.ContainsKey(arg0.SmallClass))
        // {
        //     Debug.Log("小类 " + arg0.SmallClass + " 已在全局冷却时间中，跳过生成");
        //     return;
        // }

        // 更新BigClass数量统计
        if (bigClassCount.ContainsKey(arg0.BigClass))
        {
            bigClassCount[arg0.BigClass]++;
        }
        else
        {
            bigClassCount[arg0.BigClass] = 1;
        }

        // 更新SmallClass数量统计
        if (smallClassCount.ContainsKey(arg0.SmallClass))
        {
            smallClassCount[arg0.SmallClass]++;
        }
        else
        {
            smallClassCount[arg0.SmallClass] = 1;
        }

        // 将这个小类添加到全局冷却时间中
        globalCoolDown[arg0.SmallClass] = 0f;
        Debug.Log("小类 " + arg0.SmallClass + " 已添加到全局冷却时间中");

        // 如果是植物类对象，更新缓存
        if (arg0.BigClass == "Plant")
        {
            UpdatePlantCache();
        }

        UpdateStatistics(); // 更新并打印统计信息
    }

    /// <summary>
    /// 处理对象销毁事件。当对象被销毁时，更新BigClass和SmallClass的统计数量。
    /// </summary>
    /// <param name="arg0">实现了 IGetObjectClass 接口的被销毁对象。</param>
    private void OnDestroyObject(IGetObjectClass arg0)
    {
        Debug.Log("销毁物种：" + arg0.BigClass + " " + arg0.SmallClass);

        // 更新BigClass数量统计
        if (bigClassCount.ContainsKey(arg0.BigClass))
        {
            bigClassCount[arg0.BigClass]--;
            if (bigClassCount[arg0.BigClass] <= 0)
            {
                bigClassCount.Remove(arg0.BigClass);
            }
        }

        // 更新SmallClass数量统计
        if (smallClassCount.ContainsKey(arg0.SmallClass))
        {
            smallClassCount[arg0.SmallClass]--;
            if (smallClassCount[arg0.SmallClass] <= 0)
            {
                smallClassCount.Remove(arg0.SmallClass);
                
                // 如果小类数量小于等于0，重置全局冷却时间
                if (globalCoolDown.ContainsKey(arg0.SmallClass))
                {
                    globalCoolDown[arg0.SmallClass] = 0f;
                    Debug.Log("小类 " + arg0.SmallClass + " 数量已为0，重置全局冷却时间");
                }
            }
        }
        
        // 如果是植物类对象，更新缓存
        if (arg0.BigClass == "Plant")
        {
            UpdatePlantCache();
        }
        
        UpdateStatistics(); // 更新并打印统计信息
    }
#endregion

#region 冷却时间管理
    /// <summary>
    /// 检查指定小类的全局冷却时间是否准备就绪。
    /// </summary>
    /// <param name="smallClass">要检查的小类名称。</param>
    /// <param name="requiredCoolDown">所需的冷却时间。</param>
    /// <returns>如果冷却时间已到或不存在记录，则返回true；否则返回false。</returns>
    public bool IsGlobalCoolDownReady(string smallClass, float requiredCoolDown)
    {
        if (globalCoolDown.TryGetValue(smallClass, out float currentCoolDown))
        {
            return currentCoolDown >= requiredCoolDown;
        }
        return true; // 如果没有记录，说明可以立即执行
    }

    /// <summary>
    /// 重置指定小类的全局冷却时间。
    /// </summary>
    /// <param name="smallClass">要重置的小类名称。</param>
    public void ResetGlobalCoolDown(string smallClass)
    {
        if (globalCoolDown.ContainsKey(smallClass))
        {
            globalCoolDown[smallClass] = 0f;
        }
    }
#endregion

#region 植物缓存管理
    /// <summary>
    /// 更新植物缓存列表
    /// </summary>
    private void UpdatePlantCache()
    {
        plantCacheDirty = true;
    }

    /// <summary>
    /// 获取或刷新植物缓存
    /// </summary>
    private void RefreshPlantCache()
    {
        if (!plantCacheDirty) return;

        plantObjectsCache.Clear();
        plantBeHurtCache.Clear();

        // 查找所有游戏对象
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allGameObjects)
        {
            // 检查是否实现了 IGetObjectClass 接口
            IGetObjectClass objectClass = obj.GetComponent<IGetObjectClass>();
            if (objectClass != null && objectClass.BigClass == "Plant")
            {
                plantObjectsCache.Add(obj);
                
                // 获取IBeHurt接口并添加到缓存
                IBeHurt beHurt = obj.GetComponent<IBeHurt>();
                if (beHurt != null)
                {
                    plantBeHurtCache.Add(beHurt);
                }
            }
        }
        
        plantCacheDirty = false;
    }
#endregion

#region 对象获取方法
    /// <summary>
    /// 获取所有植物类的游戏对象
    /// </summary>
    /// <returns>植物类游戏对象的列表</returns>
    public List<GameObject> GetAllPlantObjects()
    {
        List<GameObject> plantObjects = new List<GameObject>();
        
        // 查找所有游戏对象
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allGameObjects)
        {
            // 检查是否实现了 IGetObjectClass 接口
            IGetObjectClass objectClass = obj.GetComponent<IGetObjectClass>();
            if (objectClass != null && objectClass.BigClass == "Plant")
            {
                plantObjects.Add(obj);
            }
        }
        
        return plantObjects;
    }

    /// <summary>
    /// 随机获取一个植物类的游戏对象
    /// </summary>
    /// <returns>随机植物类游戏对象，如果没有植物则返回null</returns>
    public GameObject GetRandomPlantObject()
    {
        List<GameObject> plantObjects = GetAllPlantObjects();
        
        if (plantObjects.Count == 0)
        {
            Debug.LogWarning("没有找到任何植物类对象");
            return null;
        }
        
        // 随机选择一个植物对象
        int randomIndex = UnityEngine.Random.Range(0, plantObjects.Count);
        return plantObjects[randomIndex];
    }

    /// <summary>
    /// 获取所有动物类的游戏对象
    /// </summary>
    /// <returns>动物类游戏对象的列表</returns>
    public List<GameObject> GetAllAnimalObjects()
    {
        List<GameObject> animalObjects = new List<GameObject>();
        
        // 查找所有游戏对象
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allGameObjects)
        {
            // 检查是否实现了 IGetObjectClass 接口
            IGetObjectClass objectClass = obj.GetComponent<IGetObjectClass>();
            if (objectClass != null && objectClass.BigClass == "Animal")
            {
                animalObjects.Add(obj);
            }
        }
        
        return animalObjects;
    }

    /// <summary>
    /// 随机获取一个动物类的游戏对象
    /// </summary>
    /// <returns>随机动物类游戏对象，如果没有动物则返回null</returns>
    public GameObject GetRandomAnimalObject()
    {
        List<GameObject> animalObjects = GetAllAnimalObjects();
        
        if (animalObjects.Count == 0)
        {
            Debug.LogWarning("没有找到任何动物类对象");
            return null;
        }
        
        // 随机选择一个动物对象
        int randomIndex = UnityEngine.Random.Range(0, animalObjects.Count);
        return animalObjects[randomIndex];
    }
#endregion

#region 统计信息计算
    /// <summary>
    /// 更新并打印当前所有物种的数量统计信息。
    /// </summary>
    private void UpdateStatistics()
    {
        Debug.Log("=== 大类统计 ===");
        foreach (var item in bigClassCount)
        {
            Debug.Log("大类：" + item.Key + " 总数量：" + item.Value);
        }

        Debug.Log("=== 小类统计 ===");
        foreach (var item in smallClassCount)
        {
            Debug.Log("小类：" + item.Key + " 数量：" + item.Value);
        }
    }

    /// <summary>
    /// 计算所有植物的总血量
    /// </summary>
    private void CalculateTotalPlantHealth()
    {
        totalPlantHealth = 0f;
        
        // 刷新缓存（如果需要）
        RefreshPlantCache();
        
        // 使用缓存的IBeHurt接口列表直接计算总血量
        foreach (IBeHurt beHurt in plantBeHurtCache)
        {
            // 累加当前血量
            totalPlantHealth += beHurt.CurrentHealth;
        }
    }
#endregion
}
