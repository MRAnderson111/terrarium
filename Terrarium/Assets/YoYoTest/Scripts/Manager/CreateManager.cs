using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateManager : MonoBehaviour
{
    // 单例实例
    private static CreateManager instance;
    
    // 公共访问点
    public static CreateManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CreateManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("CreateManager");
                    instance = go.AddComponent<CreateManager>();
                    Debug.Log("创建了新的 CreateManager 实例");
                }
            }
            return instance;
        }
    }
    
    public GameObject selectPrefab;// button选择的植物预制体
    public GameObject hitPrefab; // 用于显示鼠标射线指向位置的GameObject
    public GameObject selectPosition; // 用于显示鼠标射线指向位置的GameObject


    [Header("预制体")]
    public bool isHit; // 射线是否击中Ground物体的标志位

    // 初始化方法，在游戏开始时调用
    void Awake()
    {
        // 确保只有一个实例
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 监听预制体选择事件
            Events.OnSelectPrefab.AddListener(OnSelectPrefab);

            // 实例化位置指示器并初始隐藏（用来显示鼠标射线指向位置）
            selectPosition = Instantiate(hitPrefab);
            selectPosition.SetActive(false);
        }
        else
        {
            // 如果已存在实例，销毁此游戏对象
            Destroy(gameObject);
        }
    }



    // Update is called once per frame
    void Update()
    {
        // 实时检测鼠标射线并更新selectPosition位置
        UpdateSelectPosition();

        // 检测鼠标点击
        HandleMouseClick();
    }

    /// <summary>
    /// 更新选择位置，根据鼠标射线检测结果
    /// </summary>
    private void UpdateSelectPosition()
    {
        if (selectPosition == null) return;

        // 从摄像机发射射线到鼠标位置
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 检测射线是否击中物体
        if (Physics.Raycast(ray, out hit))
        {
            // 检查击中的物体是否有"Ground"标签
            if (hit.collider.CompareTag("Ground"))
            {
                // 将selectPosition移动到射线击中的位置
                selectPosition.transform.position = hit.point;
                isHit = true;
                // 当射线击中Ground物体时，激活selectPosition
                selectPosition.SetActive(true);
            }
            else
            {
                // 击中了物体但不是Ground标签，视为未击中
                Vector3 targetPosition = ray.origin + ray.direction * 10f; // 距离摄像机10单位
                selectPosition.transform.position = targetPosition;
                isHit = false;
                // 当射线没有击中Ground物体时，隐藏selectPosition
                selectPosition.SetActive(false);
            }
        }
        else
        {
            // 如果没有击中任何物体，可以将位置设置到射线上的某个固定距离
            Vector3 targetPosition = ray.origin + ray.direction * 10f; // 距离摄像机10单位
            selectPosition.transform.position = targetPosition;
            isHit = false;
            // 当射线没有击中物体时，也激活selectPosition
            selectPosition.SetActive(false);
        }
    }

    /// <summary>
    /// 生成预制体到指定位置
    /// </summary>
    /// <param name="prefab">要生成的预制体</param>
    /// <param name="position">生成位置</param>
    /// <returns>生成的GameObject实例</returns>
    public GameObject CreatePrefab(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogWarning("预制体为空，无法生成");
            return null;
        }

        // 检查对象是否实现了数量限制接口
        IGetQuantityLimits quantityLimitInterface = prefab.GetComponent<IGetQuantityLimits>();
        if (quantityLimitInterface != null)
        {
            // 获取当前对象数量统计管理器
            ObjectStatisticsManager statsManager = FindObjectOfType<ObjectStatisticsManager>();
            if (statsManager != null)
            {
                // 获取预制体的小类名称（假设预制体的名称就是小类名称）
                string smallClass = prefab.name;
                int currentCount = 0;
                
                // 根据小类获取当前数量
                if (statsManager.smallClassCount.ContainsKey(smallClass))
                {
                    currentCount = statsManager.smallClassCount[smallClass];
                }
                
                // 检查是否超过数量限制
                if (currentCount >= quantityLimitInterface.QuantityLimits)
                {
                    Debug.LogWarning($"无法生成 {prefab.name}，当前数量 {currentCount} 已达到限制 {quantityLimitInterface.QuantityLimits}");
                    return null;
                }
                
                Debug.Log($"检查数量限制：{prefab.name}，当前数量 {currentCount}，限制数量 {quantityLimitInterface.QuantityLimits}");
            }
            else
            {
                Debug.LogWarning("未找到 ObjectStatisticsManager 实例，无法进行数量限制检查");
            }
        }

        // 在指定位置生成预制体
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        Debug.Log("在位置 " + position + " 生成了预制体：" + prefab.name);
        
        // 触发生成事件
        IGetObjectClass objectClass = newObject.GetComponent<IGetObjectClass>();
        if (objectClass != null)
        {
            Events.OnCreateObject.Invoke(objectClass);
        }
        
        return newObject;
    }

    /// <summary>
    /// 处理鼠标点击，在射线击中位置生成预制体
    /// </summary>
    private void HandleMouseClick()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 只有当射线击中物体且有选择的预制体时才生成
            if (isHit && selectPrefab != null)
            {
                CreatePrefab(selectPrefab, selectPosition.transform.position + Vector3.up * 0.5f);
            }
            else if (!isHit)
            {
                Debug.Log("射线未击中任何物体，无法生成预制体");
            }
            else if (selectPrefab == null)
            {
                Debug.Log("未选择预制体，无法生成");
            }
        }
    }

    public void OnSelectPrefab(GameObject prefab)
    {
        selectPrefab = prefab;
        Debug.Log("选择预制体：" + selectPrefab.name);
    }

    private void OnDestroy()
    {
        Events.OnSelectPrefab.RemoveListener(OnSelectPrefab);
    }
}
