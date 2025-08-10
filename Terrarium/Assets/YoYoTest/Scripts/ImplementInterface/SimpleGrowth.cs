using System.Collections;
using UnityEngine;

public class SimpleGrowth : MonoBehaviour, IGrowth
{
    //生长速度（接口）
    public float GrowthSpeed { get; set; }
    //生长进度（接口）
    [SerializeField]
    private float growthProgress = 0f;
    public float GrowthProgress { get => growthProgress; set => growthProgress = value; }

    //mesh object
    public GameObject meshObject;

    //保存原始scale
    private Vector3 originalScale;

    // 默认生长速度
    private float defaultGrowthSpeed = 3f;

    // Start is called before the first frame update
    void Awake()
    {
        // 保存原始scale
        originalScale = meshObject.transform.localScale;
        Debug.Log("在 Awake 中捕获到的原始 scale: " + originalScale.ToString("F4"));

        // 初始设置为原始scale的0.1倍
        meshObject.transform.localScale = originalScale * 0.1f;

        // 检测地面并设置生长速度
        SetGrowthSpeedFromGround();
        
    }

    /// <summary>
    /// 检测地面并设置生长速度，然后开始生长
    /// </summary>
    private void SetGrowthSpeedFromGround()
    {
        int fertility = GroundDetectionUtils.GetGroundFertility(transform.position);
        if (fertility >= 0)
        {
            // 根据地面肥沃度调整生长速度
            GrowthSpeed = defaultGrowthSpeed * fertility;
            Debug.Log($"根据地面肥沃度设置生长速度为: {GrowthSpeed}");
        }
        else
        {
            // 使用默认生长速度
            GrowthSpeed = defaultGrowthSpeed;
            Debug.LogWarning("未检测到有效地面，使用默认生长速度: " + GrowthSpeed);
        }
        
        // 设置好生长速度后自动开始生长
        Growth();
    }


    public void Growth()
    {
        StartCoroutine(GrowthCoroutine());
    }

    private IEnumerator GrowthCoroutine()
    {
        while (growthProgress < 100f)
        {
            // 使用deltaTime让生长速度与帧率无关
            growthProgress += GrowthSpeed * Time.deltaTime;

            // 确保生长进度不超过100
            if (growthProgress > 100f)
                growthProgress = 100f;

            // 根据生长进度计算scale，从0.1到1.0
            float targetScale = 0.1f + (growthProgress / 100f) * 0.9f;
            meshObject.transform.localScale = originalScale * targetScale;
            // Debug.Log("originalScale：" + originalScale + " targetScale：" + targetScale + " progress：" + growthProgress.ToString("F2"));

            // 每帧更新一次
            yield return null;
        }

        Debug.Log("生长完成");


        // 释放研究点
        if (TryGetComponent<IReleaseResearchPoint>(out var releaseComponent))
        {
            releaseComponent.ReleaseResearchPoint();
        }

        // 检查是否可以繁殖
        if (TryGetComponent<IReproductionCheck>(out var reproductionCheckComponent))
        {
            reproductionCheckComponent.ReproductionCheck();
        }
    }


}
