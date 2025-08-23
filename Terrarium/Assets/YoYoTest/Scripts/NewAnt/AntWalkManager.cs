using UnityEngine;

public class AntWalkManager : MonoBehaviour
{
    [Header("Walk Settings")]
    [SerializeField] private float walkRadius = 1f; // 散步半径
    [SerializeField] private float arrivalDistance = 0.1f; // 到达判定距离
    
    // 散步相关变量
    private Vector3 walkTargetPosition; // 散步目标位置
    private bool isWalking = false; // 是否正在散步
    
    // 引用蚂蚁实例
    private NewAntTest ant;
    
    /// <summary>
    /// 开始散步
    /// </summary>
    /// <param name="antInstance">蚂蚁实例</param>
    public void StartWalking(NewAntTest antInstance)
    {
        ant = antInstance;
        isWalking = true;
        
        // 生成随机目标位置
        walkTargetPosition = GetRandomWalkPosition();
        
        // Debug.Log($"蚂蚁开始散步，目标位置: {walkTargetPosition}");
    }
    
    /// <summary>
    /// 停止散步
    /// </summary>
    public void StopWalking()
    {
        isWalking = false;
        Debug.Log("蚂蚁停止散步");
    }
    
    /// <summary>
    /// 获取随机散步位置
    /// </summary>
    /// <returns>随机位置</returns>
    private Vector3 GetRandomWalkPosition()
    {
        // 在当前位置周围随机半径内生成目标位置
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection.y = 0; // 保持Y轴为0，确保在平面上移动
        
        return ant.transform.position + randomDirection;
    }
    
    /// <summary>
    /// 更新散步状态
    /// </summary>
    public void UpdateWalking()
    {
        if (!isWalking || ant == null)
            return;
            
        // 检查是否到达目标位置
        if (Vector3.Distance(ant.transform.position, walkTargetPosition) < arrivalDistance)
        {
            // 到达目标位置，生成新的目标位置
            walkTargetPosition = GetRandomWalkPosition();
            Debug.Log($"蚂蚁到达目标位置，设置新目标: {walkTargetPosition}");
        }
        else
        {
            // 向目标位置移动
            Vector3 direction = (walkTargetPosition - ant.transform.position).normalized;
            ant.transform.Translate(direction * ant.moveSpeed * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// 是否正在散步
    /// </summary>
    public bool IsWalking => isWalking;
}