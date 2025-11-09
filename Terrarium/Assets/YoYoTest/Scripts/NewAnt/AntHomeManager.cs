using UnityEngine;

public class AntHomeManager : MonoBehaviour, INewAntHome
{
    [Header("Home Settings")]
    [SerializeField] private GameObject homePrefab; // 居住地预制体
    [SerializeField] private float homeSearchRange = 50f; // 搜索居住地的范围
    [SerializeField] public float homeYOffset = 0.1f; // Home在Y轴上的偏移高度，避免穿模
    
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
    /// 根据物种分类寻找或创建居住地
    /// </summary>
    /// <param name="bigClass">物种大类</param>
    /// <param name="smallClass">物种小类</param>
    /// <param name="antPosition">蚂蚁当前位置（用于在蚂蚁位置创建居住地）</param>
    public void FindOrCreateHome(string bigClass, string smallClass, Vector3 antPosition)
    {
        Debug.Log($"FindOrCreateHome被调用: bigClass={bigClass}, smallClass={smallClass}, antPosition={antPosition}");
        // 如果当前已经有关联的家，先检查这个家是否与请求的物种匹配
        if (isHaveHome && homeScript != null)
        {
            // 如果类别不匹配，则重置状态，强制为新品种寻找或创建新家
            if (homeScript.bigClass != bigClass || homeScript.smallClass != smallClass)
            {
                isHaveHome = false;
                homeObject = null;
                homeScript = null;
                Debug.Log($"物种不匹配，重置HomeManager以寻找 {bigClass}/{smallClass} 的新家。");
            }
            else
            {
                // 物种匹配，说明已经找到了正确的家，无需任何操作
                return;
            }
        }
        
        // 只有在没有家或者家不匹配的情况下，才执行寻找逻辑
        if (!isHaveHome)
        {
            // 在场景中搜索所有AntHomeTest组件的对象
            AntHomeTest[] homeScripts = FindObjectsOfType<AntHomeTest>();

            // 遍历查找匹配的居住地
            foreach (AntHomeTest script in homeScripts)
            {
                if (script.bigClass == bigClass && script.smallClass == smallClass)
                {
                    // 找到了匹配的居住地
                    homeScript = script;
                    homeObject = homeScript.gameObject;
                    isHaveHome = true; // 确保状态被设置
                    OnHomeFound?.Invoke(homeObject);
                    Debug.Log($"找到匹配的居住地: {homeObject.name} for class {bigClass}/{smallClass}");
                    return; // 找到后立即返回
                }
            }
        }
        
        // 如果场景中没有找到匹配的，则创建新的居住地
        // 这个判断是必需的，防止在上面已经找到家的情况下还去创建
        if (!isHaveHome)
        {
            Debug.Log($"场景中没有找到 {bigClass}/{smallClass} 的居住地，创建新的居住地");
        
        // 检查是否有预制体
        if (homePrefab == null)
        {
            Debug.LogError("Home预制体未设置，请在Inspector中设置Home预制体");
            return;
        }
        
        // 在蚂蚁当前位置生成居住地，直接提高Y轴位置避免穿模
        Vector3 homePosition = new Vector3(antPosition.x, antPosition.y + homeYOffset, antPosition.z);
        Debug.Log($"创建home位置: {homePosition} (Y轴偏移: {homeYOffset})");
        
        // 生成居住地预制体
        homeObject = Instantiate(homePrefab, homePosition, Quaternion.identity);
        
        // 获取居住地脚本组件
        homeScript = homeObject.GetComponent<AntHomeTest>();
        if (homeScript == null)
        {
            Debug.LogError("居住地预制体上没有AntHomeTest组件");
            Destroy(homeObject); // 清理创建失败的对象
            return;
        }

        // 为新家设置分类标签
        homeScript.bigClass = bigClass;
        homeScript.smallClass = smallClass;
        
        isHaveHome = true;
        OnHomeCreated?.Invoke(homeObject);
        
        Debug.Log($"为 {bigClass}/{smallClass} 创建了新居住地，位置: {homePosition}");
        }
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
    public void LeaveHome(INewAnt ant, System.Action<bool> onAntLeft)
    {
        if (isHaveHome && homeObject != null && homeScript != null)
        {
            homeScript.OnAntLeft(ant.GetGameObject().GetComponent<NewAntTest>(), onAntLeft);
        }
    }
    
    /// <summary>
    /// 强制创建新的居住地（接口实现）
    /// </summary>
    public void ForceCreateHome()
    {
        // 这个方法现在只重置状态，而不创建家
        // 真正的创建逻辑由蚂蚁的行为触发
        ResetHome();
    }
    
    /// <summary>
    /// 强制创建新的居住地（在指定位置）
    /// </summary>
    /// <param name="antPosition">蚂蚁当前位置（用于在蚂蚁位置创建居住地）</param>
    /// <param name="bigClass">物种大类</param>
    /// <param name="smallClass">物种小类</param>
    public void ForceCreateHome(Vector3 antPosition, string bigClass = "Default", string smallClass = "Default")
    {
        // 重置当前居住地状态
        ResetHome();
        
        // 直接在蚂蚁位置创建新的居住地
        // 检查是否有预制体
        if (homePrefab == null)
        {
            Debug.LogError("Home预制体未设置，请在Inspector中设置Home预制体");
            return;
        }
        
        // 在蚂蚁当前位置生成居住地，直接提高Y轴位置避免穿模
        Vector3 homePosition = new Vector3(antPosition.x, antPosition.y + homeYOffset, antPosition.z);
        Debug.Log($"ForceCreateHome - 创建home位置: {homePosition} (Y轴偏移: {homeYOffset})");
        
        // 生成居住地预制体
        homeObject = Instantiate(homePrefab, homePosition, Quaternion.identity);
        
        // 获取居住地脚本组件
        homeScript = homeObject.GetComponent<AntHomeTest>();
        if (homeScript == null)
        {
            Debug.LogError("居住地预制体上没有AntHomeTest组件");
            Destroy(homeObject); // 清理创建失败的对象
            return;
        }

        // 为新家设置分类标签
        homeScript.bigClass = bigClass;
        homeScript.smallClass = smallClass;
        
        isHaveHome = true;
        OnHomeCreated?.Invoke(homeObject);
        
        Debug.Log($"强制创建新居住地，位置: {homePosition}，分类: {bigClass}/{smallClass}");
    }
    
    /// <summary>
    /// 回家并停留（白天）
    /// </summary>
    /// <param name="ant">蚂蚁实例</param>
    /// <param name="moveSpeed">移动速度</param>
    public void GoHomeAndStay(INewAnt ant, float moveSpeed)
    {
        var antScript = ant.GetGameObject().GetComponent<NewAntTest>();
        if (antScript == null) return;

        // 确保我们有正确的家
        FindOrCreateHome(antScript.bigClass, antScript.smallClass, antScript.transform.position);

        // 获取导航移动组件
        AnimalNavMove navMove = ant.GetGameObject().GetComponent<AnimalNavMove>();
        
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
                HomeScript.OnAntEntered(ant.GetGameObject().GetComponent<NewAntTest>(), (isInHome) => {
                    ant.SetInHome(isInHome);
                    Debug.Log($"蚂蚁已进入居住地，isInHome状态: {isInHome}");
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
            navMove.SetTarget(GetHomePosition());
            Debug.Log("成虫白天吃饱了，有居住地，未完成繁殖，正在回家，距离: " + distance);
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
    public void GoHomeAndSleep(INewAnt ant, float moveSpeed)
    {
        var antScript = ant.GetGameObject().GetComponent<NewAntTest>();
        if (antScript == null) return;
        
        // 确保我们有正确的家
        FindOrCreateHome(antScript.bigClass, antScript.smallClass, antScript.transform.position);
        
        // 获取导航移动组件
        AnimalNavMove navMove = ant.GetGameObject().GetComponent<AnimalNavMove>();
        
        // 使用居住地管理器获取方向和距离
        Vector3 direction = GetHomeDirection();
        float distance = GetHomeDistance();
        
        // 检查是否到达居住地（1米以内）
        if (IsAtHome(1f))
        {
            Debug.Log("成虫夜晚，有居住地，已到达居住地");
            
            // 设置蚂蚁为睡觉状态
            ant.SetSleeping(true);
            Debug.Log("蚂蚁开始睡觉");
            
            // 调用居住地的接口方法，通知蚂蚁已到达
            if (HomeScript != null)
            {
                HomeScript.OnAntEntered(ant.GetGameObject().GetComponent<NewAntTest>(), (isInHome) => {
                    ant.SetInHome(isInHome);
                    Debug.Log($"蚂蚁已进入居住地，isInHome状态: {isInHome}");
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
            navMove.SetTarget(GetHomePosition());
            Debug.Log("成虫夜晚，有居住地，正在回家睡觉，距离: " + distance);
        }
        else
        {
            Debug.Log("成虫夜晚，有居住地，但居住地不存在");
        }
    }
}