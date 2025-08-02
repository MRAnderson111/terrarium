using System.Collections;
using UnityEngine;

public class SimpleReproductionCheck : MonoBehaviour, IReproductionCheck
{
    [Header("检测参数")]
    public float checkDistance = 1f; // 检测距离
    public float sphereRadius = 0.5f; // 检测球体半径
    public bool drawDebugSphere = false; // 是否绘制调试球体

    public void ReproductionCheck()
    {
        // 调用静态工具类进行球形检测
        SphereDetectionUtility.PerformDirectionalSphereDetection(
            transform.position,
            checkDistance,
            sphereRadius,
            drawDebugSphere
        );
    }
}
