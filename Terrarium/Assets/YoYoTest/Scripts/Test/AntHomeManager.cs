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
        // 首先在场景中搜索是否有名为"home"的对象
        GameObject existingHome = GameObject.Find("home");
        
        if (existingHome != null)
        {
            Debug.Log("场景中已存在名为'home'的居住地，使用现有居住地");
            homeObject = existingHome;
            
            // 获取居住地脚本组件
            homeScript = homeObject.GetComponent<AntHomeTest>();
            if (homeScript == null)
            {
                Debug.LogError("现有居住地对象上没有AntHomeTest组件");
                return;
            }
            
            isHaveHome = true;
            OnHomeFound?.Invoke(homeObject);
            return;
        }
        
        // 如果场景中没有名为"home"的对象，则创建新的居住地
        Debug.Log("场景中没有名为'home'的居住地，创建新的居住地");
        
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
}