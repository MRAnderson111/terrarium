using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISampleMove : MonoBehaviour
{
    public Transform lookTarget;      // 面向的目标
    public Transform moveTarget;      // 移动跟随的目标
    public Vector3 offset = new Vector3(0, 1f, 0); // 相对于moveTarget的偏移位置
    public float moveSpeed = 5f;      // 移动速度
    public float rotationSpeed = 10f; // 旋转速度

    private Vector3 targetPosition;   // 目标位置

    // Start is called before the first frame update
    void Start()
    {
        // 初始化位置
        if (moveTarget != null)
        {
            targetPosition = moveTarget.position + offset;
            transform.position = targetPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveTarget == null || lookTarget == null)
            return;

        // 计算目标位置（moveTarget的上方偏移位置）
        targetPosition = moveTarget.position + offset;

        // 使用插值平滑移动到目标位置
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 面向lookTarget（反向）
        Vector3 lookDirection = lookTarget.position - transform.position;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            // 在Y轴旋转180度，实现反向面向
            targetRotation *= Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
