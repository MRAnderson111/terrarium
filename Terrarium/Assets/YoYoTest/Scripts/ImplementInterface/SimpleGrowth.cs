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
    public GameObject meshObject2;

    //保存原始scale
    private Vector3 originalScale;

    // 基础生长速度
    public float defaultGrowthSpeed = 1f;

    // 速度设置完成
    public bool isSpeedSet = false;
    public bool IsSpeedSet { get => isSpeedSet; set => isSpeedSet = value; }

    //生成研究点数概率
    public float researchPointProbability = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        // 保存原始scale
        originalScale = meshObject.transform.localScale;
        Debug.Log("在 Awake 中捕获到的原始 scale: " + originalScale.ToString("F4"));

        // 初始设置为原始scale的0.1倍
        meshObject.transform.localScale = originalScale * 0.1f;
        
        // 如果有meshObject2，初始设置为不可见
        if (meshObject2 != null)
        {
            meshObject2.SetActive(false);
        }

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

        isSpeedSet = true;
        
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

        // 检查是否有meshObject2，如果有则切换mesh
        if (meshObject2 != null)
        {
            // 关闭当前mesh
            meshObject.SetActive(false);
            // 显示meshObject2
            meshObject2.SetActive(true);
            Debug.Log("植物已完全生长，切换到meshObject2");
        }

        // 释放研究点
        if (TryGetComponent<IReleaseResearchPoint>(out var releaseComponent))
        {
            // 根据概率决定是否生成研究点数
            if (Random.value <= researchPointProbability)
            {
                releaseComponent.ReleaseResearchPoint();
                Debug.Log($"研究点数生成成功，概率: {researchPointProbability}");
            }
            else
            {
                Debug.Log($"研究点数生成失败，概率: {researchPointProbability}");
            }
        }

        // 检查是否可以繁殖
        if (TryGetComponent<IReproductionCheck>(out var reproductionCheckComponent))
        {
            reproductionCheckComponent.ReproductionCheck();
        }
    }


}
