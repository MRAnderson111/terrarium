using UnityEngine;
using UnityEngine.EventSystems;

public class MouseCameraController : MonoBehaviour
{
    [Header("鼠标灵敏度")]
    public float mouseSensitivity = 100f;
    
    [Header("垂直旋转限制")]
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;
    
    [Header("平滑移动")]
    public float smoothTime = 0.1f;
    
    [Header("WASD移动设置")]
    public float moveSpeed = 10f;
    public float maxDistanceFromCenter = 20f;
    public Transform planeCenter;
    // 删除了lookAtSmoothness参数

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;
    private Vector3 planeCenterPosition;

    void Start()
    {
        // 不要自动锁定鼠标，让玩家手动点击来锁定
        // Cursor.lockState = CursorLockMode.Locked; // 删除这行
        
        // 查找Plane中心点
        if (planeCenter == null)
        {
            GameObject plane = GameObject.Find("Plane");
            if (plane != null)
            {
                planeCenter = plane.transform;
            }
        }
        
        if (planeCenter != null)
        {
            planeCenterPosition = planeCenter.position;
        } 
        else
        {
            planeCenterPosition = Vector3.zero;
        }
        
        // 获取当前相机的旋转角度
        Vector3 currentRotation = transform.eulerAngles;
        yRotation = currentRotation.y;
        xRotation = currentRotation.x;
        
        // 处理角度范围
        if (xRotation > 180f)
            xRotation -= 360f;
    }

    void Update()
    {
        // 按ESC键解锁鼠标
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
        // 点击屏幕重新锁定鼠标（但排除UI点击）
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            // 检查是否存在EventSystem且是否点击在UI上
            if (!IsPointerOverUI())
            {
                Cursor.lockState = CursorLockMode.Confined; // 改为Confined模式
            }
        }
        
        // 右键点击也可以激活相机控制
        if (Input.GetMouseButtonDown(1) && Cursor.lockState == CursorLockMode.None)
        {
            if (!IsPointerOverUI())
            {
                Cursor.lockState = CursorLockMode.Confined; // 改为Confined模式
            }
        }
        
        // 按住任意WASD键也可以激活相机控制
        if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                Cursor.lockState = CursorLockMode.Confined; // 改为Confined模式
            }
        }
        
        // 在Confined模式下控制相机
        if (Cursor.lockState == CursorLockMode.Confined)
        {
            HandleMouseLook();
            HandleMovement();
        }
    }
    
    // 安全地检查指针是否在UI上
    private bool IsPointerOverUI()
    {
        // 确保EventSystem存在
        if (EventSystem.current != null)
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
        return false; // 如果没有EventSystem，假定不是在UI上
    }

    void HandleMouseLook()
    {
        // 获取鼠标输入
        Vector2 targetMouseDelta = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );
        
        // 平滑鼠标移动
        currentMouseDelta = Vector2.SmoothDamp(
            currentMouseDelta, 
            targetMouseDelta, 
            ref currentMouseDeltaVelocity, 
            smoothTime
        );
        
        // 计算旋转
        yRotation += currentMouseDelta.x * mouseSensitivity * Time.deltaTime;
        xRotation -= currentMouseDelta.y * mouseSensitivity * Time.deltaTime;
        
        // 限制垂直旋转角度
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        
        // 应用旋转
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void HandleMovement()
    {
        // 获取WASD输入
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S
        
        if (horizontal != 0 || vertical != 0)
        {
            // 基于世界坐标系计算移动方向，而不是相机朝向
            Vector3 worldForward = Vector3.forward;
            Vector3 worldRight = Vector3.right;
            
            // 计算移动向量（基于世界坐标）
            Vector3 moveDirection = (worldForward * vertical + worldRight * horizontal).normalized;
            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            
            // 保持原有高度
            newPosition.y = transform.position.y;
            
            // 限制移动范围（只在水平面上计算距离）
            Vector3 horizontalPosition = new Vector3(newPosition.x, planeCenterPosition.y, newPosition.z);
            Vector3 horizontalCenter = new Vector3(planeCenterPosition.x, planeCenterPosition.y, planeCenterPosition.z);
            Vector3 directionFromCenter = horizontalPosition - horizontalCenter;
            
            if (directionFromCenter.magnitude > maxDistanceFromCenter)
            {
                // 将位置限制在最大距离内（只影响水平位置）
                directionFromCenter = directionFromCenter.normalized * maxDistanceFromCenter;
                newPosition.x = horizontalCenter.x + directionFromCenter.x;
                newPosition.z = horizontalCenter.z + directionFromCenter.z;
                newPosition.y = transform.position.y;
            }
            
            // 应用新位置
            transform.position = newPosition;
            
            // 移除了LookAtPlaneCenter()调用 - 不再自动旋转镜头
        }
    }

    void LookAtPlaneCenter()
    {
        // 计算看向Plane中心的方向
        Vector3 directionToCenter = planeCenterPosition - transform.position;
        
        // 计算目标旋转角度
        float targetYRotation = Mathf.Atan2(directionToCenter.x, directionToCenter.z) * Mathf.Rad2Deg;
        float targetXRotation = -Mathf.Atan2(directionToCenter.y, 
            Mathf.Sqrt(directionToCenter.x * directionToCenter.x + directionToCenter.z * directionToCenter.z)) * Mathf.Rad2Deg;
        
        // 使用更温和的平滑过渡，减少旋转干扰
        yRotation = Mathf.LerpAngle(yRotation, targetYRotation, Time.deltaTime);
        xRotation = Mathf.LerpAngle(xRotation, targetXRotation, Time.deltaTime);
        
        // 限制垂直旋转角度
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        
        // 应用旋转
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}










