using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR;

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
    public SelectButton selectButton; // 当前选中的SelectButton脚本引用
    public GameObject hitPrefab; // 用于显示鼠标射线指向位置的GameObject
    public GameObject selectPosition; // 用于显示鼠标射线指向位置的GameObject

    public Transform rayOrigin; // 旧的 VR 射线起点，没有鼠标时才使用

    [Header("鼠标放置")]
    public bool useMousePlacement = true;
    public float placeHeightOffset = 0.5f;
    
    [Header("射线可视化")]
    public AdvancedRayVisualizer rayVisualizer; // 高级射线可视化器
    public bool useAdvancedRayVisualizer = false;

    [Header("预制体")]
    public bool isHit; // 射线是否击中Ground物体的标志位
    public int goldCount = 1000; // 金币数量

    public void AddGold(int amount)
    {
        goldCount += amount;
        Debug.Log($"金币数量增加: {amount}, 当前总数: {goldCount}");
    }
 
    // 初始化方法，在游戏开始时调用
    void Awake()
    {
        // 确保只有一个实例
        if (instance == null)
        {
            instance = this;
            
            // 如果需要使用高级射线可视化器且未指定，则自动添加
            if (useAdvancedRayVisualizer && rayVisualizer == null)
            {
                rayVisualizer = gameObject.AddComponent<AdvancedRayVisualizer>();
            }

            Events.OnSelectPrefab.AddListener(OnSelectPrefab);

            if (hitPrefab != null)
            {
                selectPosition = Instantiate(hitPrefab);
                selectPosition.SetActive(false);
            }

            EnsureDesktopCamera();
        }
        else
        {
            // 如果已存在实例，销毁此游戏对象
            Destroy(gameObject);
        }
    }



    void Update()
    {
        UpdateSelectPosition();
        HandlePlacementInput();
    }

    /// <summary>
    /// 没有头显时给主相机挂上桌面视角：右键转向，WASD 移动，左键留给放置。
    /// </summary>
    private void EnsureDesktopCamera()
    {
        if (XRSettings.isDeviceActive)
            return;

        Camera camera = Camera.main;
        if (camera != null && camera.GetComponent<MouseCameraController>() == null)
            camera.gameObject.AddComponent<MouseCameraController>();
    }

    private bool TryGetPlacementRay(out Ray ray)
    {
        if (useMousePlacement)
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                Vector2 screenPosition = Mouse.current != null
                    ? Mouse.current.position.ReadValue()
                    : (Vector2)Input.mousePosition;
                ray = camera.ScreenPointToRay(screenPosition);
                return true;
            }
        }

        if (rayOrigin != null)
        {
            ray = new Ray(rayOrigin.position, rayOrigin.forward);
            return true;
        }

        ray = default;
        return false;
    }

    private bool WasPlacePressedThisFrame()
    {
        if (useMousePlacement && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        if (useMousePlacement && Mouse.current == null && Input.GetMouseButtonDown(0))
            return true;

        try
        {
            return InputActionsManager.Actions.XRIRightInteraction.Activate.WasPressedThisFrame();
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;
        if (EventSystem.current.IsPointerOverGameObject())
            return true;
        return Mouse.current != null && EventSystem.current.IsPointerOverGameObject(Mouse.current.deviceId);
    }

    /// <summary>
    /// 更新选择位置。鼠标模式下从主相机穿过鼠标位置打射线。
    /// </summary>
    private void UpdateSelectPosition()
    {
        if (selectPosition == null || !TryGetPlacementRay(out Ray ray)) return;

        RaycastHit hit;

        // 检测射线是否击中物体
        if (Physics.Raycast(ray, out hit, 200f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            // 使用高级射线可视化器绘制射线到击中点（绿色表示击中）
            if (useAdvancedRayVisualizer && rayVisualizer != null)
            {
                rayVisualizer.DrawRay(ray.origin, hit.point, true);
            }
            else
            {
                // 保留原有的Debug.DrawLine作为备用
                Debug.DrawLine(ray.origin, hit.point, Color.green);
            }
            
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
                Vector3 targetPosition = ray.origin + ray.direction * 10f; // 距离射线起点10单位
                selectPosition.transform.position = targetPosition;
                isHit = false;
                // 当射线没有击中Ground物体时，隐藏selectPosition
                selectPosition.SetActive(false);
            }
        }
        else
        {
            // 如果没有击中任何物体，绘制固定长度的射线（红色表示未击中）
            Vector3 targetPosition = ray.origin + ray.direction * 10f; // 距离射线起点10单位
            
            // 使用高级射线可视化器绘制射线
            if (useAdvancedRayVisualizer && rayVisualizer != null)
            {
                rayVisualizer.DrawRay(ray.origin, targetPosition, false);
            }
            else
            {
                // 保留原有的Debug.DrawLine作为备用
                Debug.DrawLine(ray.origin, targetPosition, Color.red);
            }
            
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



        // 获取当前对象数量统计管理器
        ObjectStatisticsManager statsManager = FindObjectOfType<ObjectStatisticsManager>();
        if (statsManager != null)
        {
            // 获取预制体的名称作为键值
            string prefabName = prefab.name;
            int currentCount = 0;

            // 根据预制体名称获取当前数量
            if (statsManager.smallClassCount.ContainsKey(prefabName))
            {
                currentCount = statsManager.smallClassCount[prefabName];
            }

            // 从静态管理器获取数量限制值
            int currentQuantityLimit = StaticCreateLimitManager.GetCreateLimit(prefabName, 10); // 默认值为10
            
            // 检查是否超过数量限制
            if (currentCount >= currentQuantityLimit)
            {
                Debug.LogWarning($"无法生成 {prefab.name}，当前数量 {currentCount} 已达到限制 {currentQuantityLimit}");
                return null;
            }

            Debug.Log($"检查数量限制：{prefab.name}，当前数量 {currentCount}，限制数量 {currentQuantityLimit}");
        }
        else
        {
            Debug.LogWarning("未找到 ObjectStatisticsManager 实例，无法进行数量限制检查");
        }

        // 生成随机的Y轴旋转角度（0-360度）
        float randomYRotation = UnityEngine.Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(0, randomYRotation, 0);
        
        // 在指定位置生成预制体，并应用随机Y轴旋转
        GameObject newObject = Instantiate(prefab, position, randomRotation);
        Debug.Log("在位置 " + position + " 生成了预制体：" + prefab.name + "，Y轴旋转：" + randomYRotation + "度");

        // 不在这里触发生成事件，让对象自己的 Start() 方法来触发

        return newObject;
    }

    /// <summary>
    /// 左键或右手柄 Trigger 在击中的地面上生成当前选中的预制体。
    /// </summary>
    private void HandlePlacementInput()
    {
        if (!WasPlacePressedThisFrame() || IsPointerOverUI())
            return;

        if (isHit && selectPrefab != null)
        {
            if (selectButton != null && selectButton.quantity > 0)
            {
                StaticCreateLimitManager.AddToCreateLimit(selectPrefab.name);
                CreatePrefab(selectPrefab, selectPosition.transform.position + Vector3.up * placeHeightOffset);
                selectButton.DecrementQuantity();
            }
            else if (selectButton == null)
            {
                CreatePrefab(selectPrefab, selectPosition.transform.position + Vector3.up * placeHeightOffset);
            }
        }
        else if (!isHit)
        {
            Debug.Log("射线未击中地面，无法生成预制体");
        }
        else if (selectPrefab == null)
        {
            Debug.Log("未选择预制体，无法生成");
        }
    }

    public void OnSelectPrefab(GameObject prefab, SelectButton button)
    {
        selectPrefab = prefab;
        selectButton = button;
        Debug.Log("选择预制体：" + selectPrefab.name + "，SelectButton：" + button.name);
    }

    private void OnDestroy()
    {
        Events.OnSelectPrefab.RemoveListener(OnSelectPrefab);
    }

    /// <summary>
    /// 获取场景中已存在的同类对象的最新的数量限制值
    /// </summary>
    /// <param name="smallClass">小类名称</param>
    /// <returns>最新的数量限制值</returns>
    public int GetCurrentQuantityLimit(string smallClass)
    {
        // 查找场景中所有实现了IGetQuantityLimits接口的对象
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        // 默认使用预制体的默认值
        int currentQuantityLimit = 10;
        
        foreach (var gameObject in allObjects)
        {
            // 检查对象是否实现了IGetQuantityLimits接口
            IGetQuantityLimits quantityLimitInterface = gameObject.GetComponent<IGetQuantityLimits>();
            if (quantityLimitInterface != null)
            {
                // 获取对象的IGetObjectClass组件
                IGetObjectClass objectClassInterface = gameObject.GetComponent<IGetObjectClass>();
                if (objectClassInterface != null)
                {
                    // 检查是否是同类对象
                    if (objectClassInterface.SmallClass == smallClass)
                    {
                        // 找到同类对象，使用其最新的数量限制值
                        currentQuantityLimit = quantityLimitInterface.QuantityLimits;
                        break;
                    }
                }
            }
        }
        
        return currentQuantityLimit;
    }
}
