using System.Collections;
using UnityEngine;

/// <summary>
/// 动物移动系统 - 处理所有移动相关的逻辑
/// </summary>
public class AnimalMovementSystem : MonoBehaviour, IAnimalMovement
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float wanderSpeed = 5f;
    [SerializeField] private float wanderRadius = 15f;
    [SerializeField] private float wanderInterval = 3f;
    
    // 移动状态
    private bool isMoving = false;
    private bool isWandering = false;
    private Vector3 currentTarget;
    private Vector3 wanderTarget;
    private float wanderTimer = 0f;
    
    // 地面检测
    private bool hasGrounded = false;
    
    // 环境影响修正系数
    private float environmentalSpeedModifier = 1f;
    
    // 属性实现
    public bool IsMoving => isMoving;
    public float MoveSpeed 
    { 
        get => moveSpeed; 
        set => moveSpeed = value; 
    }
    
    public bool HasGrounded => hasGrounded;
    public bool IsWandering => isWandering;
    
    void Update()
    {
        if (isWandering)
        {
            HandleWandering();
        }
        else if (isMoving)
        {
            MoveTowardsTarget();
        }
    }
    
    public void MoveTo(Vector3 target)
    {
        if (!hasGrounded)
        {
            Debug.LogWarning("动物尚未落地，无法移动");
            return;
        }
        
        currentTarget = target;
        isMoving = true;
        isWandering = false;
        
        Debug.Log($"开始移动到目标位置: {target}");
    }
    
    public void StopMovement()
    {
        isMoving = false;
        isWandering = false;
        Debug.Log("停止移动");
    }
    
    public void StartWandering()
    {
        if (!hasGrounded)
        {
            Debug.LogWarning("动物尚未落地，无法开始游荡");
            return;
        }
        
        isWandering = true;
        isMoving = false;
        wanderTimer = 0f;
        ChooseNewWanderTarget();
        Debug.Log("开始游荡行为");
    }
    
    public void SetGrounded(bool grounded)
    {
        hasGrounded = grounded;
        if (grounded)
        {
            Debug.Log("动物已落地，可以开始移动");
        }
    }
    
    public void SetEnvironmentalSpeedModifier(float modifier)
    {
        environmentalSpeedModifier = Mathf.Clamp01(modifier);
    }
    
    private void HandleWandering()
    {
        wanderTimer += Time.deltaTime;
        
        // 每隔一定时间选择新的游荡目标
        if (wanderTimer >= wanderInterval)
        {
            ChooseNewWanderTarget();
            wanderTimer = 0f;
        }
        
        // 向游荡目标移动
        MoveTowardsWanderTarget();
    }
    
    private void ChooseNewWanderTarget()
    {
        // 在当前位置周围随机选择一个目标点
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        wanderTarget = new Vector3(
            transform.position.x + randomDirection.x,
            transform.position.y,
            transform.position.z + randomDirection.y
        );
        
        Debug.Log($"选择新的游荡目标: {wanderTarget}");
    }
    
    private void MoveTowardsWanderTarget()
    {
        // 计算移动方向（只在水平面移动，保持Y轴不变）
        Vector3 direction = (wanderTarget - transform.position).normalized;
        
        // 应用环境影响的游荡速度
        float adjustedWanderSpeed = wanderSpeed * environmentalSpeedModifier;
        Vector3 newPosition = transform.position + direction * adjustedWanderSpeed * Time.deltaTime;
        
        // 使用射线检测确保动物贴地移动
        newPosition = GetGroundedPosition(newPosition);
        transform.position = newPosition;
        
        // 检查是否接近目标点（距离小于2米时选择新目标）
        float distance = Vector3.Distance(transform.position, wanderTarget);
        if (distance < 2f)
        {
            ChooseNewWanderTarget();
        }
    }
    
    private void MoveTowardsTarget()
    {
        // 计算移动方向（只在水平面移动，保持Y轴不变）
        Vector3 targetPosition = new Vector3(currentTarget.x, transform.position.y, currentTarget.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // 应用环境影响的移动速度
        float adjustedMoveSpeed = moveSpeed * environmentalSpeedModifier;
        Vector3 newPosition = transform.position + direction * adjustedMoveSpeed * Time.deltaTime;
        
        // 使用射线检测确保动物贴地移动
        newPosition = GetGroundedPosition(newPosition);
        transform.position = newPosition;
        
        // 检查是否到达目标附近（距离小于3米）
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 3f)
        {
            isMoving = false;
            Debug.Log("到达目标位置");
            
            // 触发到达目标事件
            OnTargetReached?.Invoke();
        }
    }
    
    private Vector3 GetGroundedPosition(Vector3 targetPosition)
    {
        // 从目标位置上方向下发射射线，确保动物贴地移动
        float raycastHeight = 10f;
        float maxDistance = 20f;
        
        Vector3 rayStart = new Vector3(targetPosition.x, targetPosition.y + raycastHeight, targetPosition.z);
        
        // 发射射线向下检测地面
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, maxDistance))
        {
            // 检查碰撞的是否是地面对象
            if (IsGroundObject(hit.collider.gameObject))
            {
                // 将动物位置设置为地面上方一点点（避免穿透）
                return new Vector3(targetPosition.x, hit.point.y + 0.5f, targetPosition.z);
            }
        }
        
        // 如果没有检测到地面，保持当前Y坐标
        return new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
    }
    
    private bool IsGroundObject(GameObject obj)
    {
        return obj.CompareTag("Ground") ||
               obj.name.Contains("Ground") ||
               obj.name.Contains("Plane") ||
               obj.name.Contains("BarrenGround") ||
               obj.name.Contains("FertileGround") ||
               obj.name.Contains("Terrain");
    }
    
    // 事件
    public System.Action OnTargetReached;
    
    // 公共方法供外部调用
    public float GetDistanceToTarget()
    {
        if (isMoving)
        {
            return Vector3.Distance(transform.position, currentTarget);
        }
        return 0f;
    }
    
    public Vector3 GetCurrentTarget()
    {
        return isMoving ? currentTarget : transform.position;
    }
}
