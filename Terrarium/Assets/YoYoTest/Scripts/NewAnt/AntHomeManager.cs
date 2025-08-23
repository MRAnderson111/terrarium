using UnityEngine;

public class AntHomeManager : MonoBehaviour
{
    [Header("Home Settings")]
    [SerializeField] private GameObject homePrefab; // 居住地预制体
    [SerializeField] private float homeSearchRange = 50f; // 搜索居住地的范围
    
    private GameObject homeObject; // 居住地对象
    private AntHomeTest homeScript; // 居住地脚本组件
    private bool isHaveHome = false; // 是否有居住地
    
    // 居住地相关事件
    public delegate void HomeEventHandler(GameObject homeObject);
    public event HomeEventHandler OnHomeCreated;
    public event HomeEventHandler OnHomeFound;
    
    // 属性
    public bool IsHaveHome => isHaveHome;
    public GameObject HomeObject => homeObject;
    public AntHomeTest HomeScript => homeScript;
    
    private void Awake()
    {
        // 不在初始化时创建居住地
        // 居住地将在蚂蚁成为成虫后创建
    }
    
    /// <summary>
    /// 初始化居住地
    /// </summary>
    private void InitializeHome()
    {
        // 首先在场景中搜索是否有AntHomeTest组件的对象
        AntHomeTest[] homeScripts = FindObjectsOfType<AntHomeTest>();
        
        if (homeScripts != null && homeScripts.Length > 0)
        {
            // 使用找到的第一个居住地
            homeScript = homeScripts[0];
            homeObject = homeScript.gameObject;
            isHaveHome = true;
            OnHomeFound?.Invoke(homeObject);
            Debug.Log("场景中已存在AntHomeTest组件的居住地: " + homeObject.name + "，位置: " + homeObject.transform.position);
            return;
        }
        
        // 如果场景中没有找到AntHomeTest组件的对象，则创建新的居住地
        Debug.Log("场景中没有找到AntHomeTest组件的居住地，创建新的居住地");
        
        // 检查是否有预制体
        if (homePrefab == null)
        {
            Debug.LogError("Home预制体未设置，请在Inspector中设置Home预制体");
            return;
        }
        
        // 在随机位置生成居住地
        Vector3 randomPosition = transform.position + new Vector3(
            Random.Range(-10f, 10f),
            0f,
            Random.Range(-10f, 10f)
        );
        
        // 生成居住地预制体
        homeObject = Instantiate(homePrefab, randomPosition, Quaternion.identity);
        
        // 获取居住地脚本组件
        homeScript = homeObject.GetComponent<AntHomeTest>();
        if (homeScript == null)
        {
            Debug.LogError("居住地预制体上没有AntHomeTest组件");
            return;
        }
        
        isHaveHome = true;
        OnHomeCreated?.Invoke(homeObject);
        
        Debug.Log("居住地已创建在位置: " + randomPosition);
    }
    
    /// <summary>
    /// 获取居住地位置
    /// </summary>
    /// <returns>居住地位置，如果没有居住地则返回当前位置</returns>
    public Vector3 GetHomePosition()
    {
        if (isHaveHome && homeObject != null)
        {
            return homeObject.transform.position;
        }
        return transform.position;
    }
    
    /// <summary>
    /// 获取到居住地的方向
    /// </summary>
    /// <returns>到居住地的方向向量，如果没有居住地则返回零向量</returns>
    public Vector3 GetHomeDirection()
    {
        if (isHaveHome && homeObject != null)
        {
            return (homeObject.transform.position - transform.position).normalized;
        }
        return Vector3.zero;
    }
    
    /// <summary>
    /// 获取到居住地的距离
    /// </summary>
    /// <returns>到居住地的距离，如果没有居住地则返回无穷大</returns>
    public float GetHomeDistance()
    {
        if (isHaveHome && homeObject != null)
        {
            return Vector3.Distance(transform.position, homeObject.transform.position);
        }
        return Mathf.Infinity;
    }
    
    /// <summary>
    /// 检查是否到达居住地
    /// </summary>
    /// <param name="arrivalDistance">到达判定距离</param>
    /// <returns>是否到达居住地</returns>
    public bool IsAtHome(float arrivalDistance = 1f)
    {
        if (!isHaveHome || homeObject == null)
        {
            return false;
        }
        
        float distance = Vector3.Distance(transform.position, homeObject.transform.position);
        return distance <= arrivalDistance;
    }
    
    /// <summary>
    /// 重置居住地
    /// </summary>
    public void ResetHome()
    {
        if (isHaveHome && homeObject != null)
        {
            if (homeScript != null)
            {
                // 创建一个空的回调函数，因为我们只是想移除蚂蚁
                homeScript.OnAntLeft(null, null);
            }
            
            isHaveHome = false;
            homeObject = null;
            homeScript = null;
            
            Debug.Log("居住地已重置");
        }
    }
    
    /// <summary>
    /// 离开居住地
    /// </summary>
    /// <param name="ant">离开的蚂蚁实例</param>
    /// <param name="onAntLeft">回调函数</param>
    public void LeaveHome(NewAntTest ant, System.Action<bool> onAntLeft)
    {
        if (isHaveHome && homeObject != null && homeScript != null)
        {
            homeScript.OnAntLeft(ant, onAntLeft);
        }
    }
    
    /// <summary>
    /// 强制创建新的居住地
    /// </summary>
    public void ForceCreateHome()
    {
        ResetHome();
        InitializeHome();
    }
    
    /// <summary>
    /// 回家并停留（白天）
    /// </summary>
    /// <param name="ant">蚂蚁实例</param>
    /// <param name="moveSpeed">移动速度</param>
    public void GoHomeAndStay(NewAntTest ant, float moveSpeed)
    {
        // 获取导航移动组件
        AnimalNavMove navMove = ant.GetComponent<AnimalNavMove>();
        
        // 使用居住地管理器获取方向和距离
        Vector3 direction = GetHomeDirection();
        float distance = GetHomeDistance();
        
        // 检查是否到达居住地（1米以内）
        if (IsAtHome(1f))
        {
            Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖，已到达居住地");
            
            // 调用居住地的接口方法，通知蚂蚁已到达
            if (HomeScript != null)
            {
                HomeScript.OnAntEntered(ant, (isInHome) => {
                    ant.isInHome = isInHome;
                    Debug.Log($"蚂蚁已进入居住地，isInHome状态: {ant.isInHome}");
                });
            }
            else
            {
                Debug.LogError("居住地脚本组件未找到");
            }
        }
        else if (direction != Vector3.zero)
        {
            // 使用导航移动组件移动到居住地
            if (navMove != null)
            {
                navMove.SetTarget(GetHomePosition());
                Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖，正在回家（使用导航），距离: " + distance);
            }
            else
            {
                // 如果没有导航组件，使用原来的translate方式作为备用
                direction.Normalize();
                ant.transform.Translate(direction * moveSpeed * Time.deltaTime);
                Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖，正在回家（使用translate备用），距离: " + distance);
            }
        }
        else
        {
            Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖，但居住地不存在");
        }
    }
    
    /// <summary>
    /// 回家睡觉（夜晚）
    /// </summary>
    /// <param name="ant">蚂蚁实例</param>
    /// <param name="moveSpeed">移动速度</param>
    public void GoHomeAndSleep(NewAntTest ant, float moveSpeed)
    {
        // 获取导航移动组件
        AnimalNavMove navMove = ant.GetComponent<AnimalNavMove>();
        
        // 使用居住地管理器获取方向和距离
        Vector3 direction = GetHomeDirection();
        float distance = GetHomeDistance();
        
        // 检查是否到达居住地（1米以内）
        if (IsAtHome(1f))
        {
            Debug.Log("成虫夜晚，有居住地，已到达居住地");
            
            // 设置蚂蚁为睡觉状态
            ant.isSleeping = true;
            Debug.Log("蚂蚁开始睡觉");
            
            // 调用居住地的接口方法，通知蚂蚁已到达
            if (HomeScript != null)
            {
                HomeScript.OnAntEntered(ant, (isInHome) => {
                    ant.isInHome = isInHome;
                    Debug.Log($"蚂蚁已进入居住地，isInHome状态: {ant.isInHome}");
                });
            }
            else
            {
                Debug.LogError("居住地脚本组件未找到");
            }
        }
        else if (direction != Vector3.zero)
        {
            // 使用导航移动组件移动到居住地
            if (navMove != null)
            {
                navMove.SetTarget(GetHomePosition());
                Debug.Log("成虫夜晚，有居住地，正在回家睡觉（使用导航），距离: " + distance);
            }
            else
            {
                // 如果没有导航组件，使用原来的translate方式作为备用
                direction.Normalize();
                ant.transform.Translate(direction * moveSpeed * Time.deltaTime);
                Debug.Log("成虫夜晚，有居住地，正在回家睡觉（使用translate备用），距离: " + distance);
            }
        }
        else
        {
            Debug.Log("成虫夜晚，有居住地，但居住地不存在");
        }
    }
}