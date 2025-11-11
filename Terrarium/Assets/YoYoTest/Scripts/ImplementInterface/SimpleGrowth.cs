using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

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
    public Transform fxSpot; // 特效播放位置点

    //保存原始scale
    private Vector3 originalScale;

    // 基础生长速度
    public float defaultGrowthSpeed = 1f;

    // 速度设置完成
    public bool isSpeedSet = false;
    public bool IsSpeedSet { get => isSpeedSet; set => isSpeedSet = value; }

    //生成研究点数概率
    public float researchPointProbability = 0.5f;

    [Header("特效相关")]
    // FX_Grown特效预制体
    public GameObject fxGrownPrefab;
    // 防止重复播放特效的标志
    private bool hasPlayedEffect = false;

    // FX_Planted特效预制体
    public GameObject fxPlantedPrefab;
    // 防止重复播放种植特效的标志
    private bool hasPlayedPlantedEffect = false;

    [Header("特效随机化设置")]
    // 最小缩放值
    public float minScale = 0.4f;
    // 最大缩放值
    public float maxScale = 0.75f;

    [Header("音效设置")]
    public AudioClip plantedSoundClip; // 种植时播放的声效
    private AudioSource audioSource; // 音频源组件

    // Start is called before the first frame update
    void Awake()
    {
        // 获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 保存原始scale
        originalScale = meshObject.transform.localScale;
        Debug.Log("在 Awake 中捕获到的原始 scale: " + originalScale.ToString("F4"));

        // 播放种植特效
        PlayPlantedEffect();

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

        // 播放带timeline的particle system特效
        PlayTimelineWithParticleSystem();

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

    /// <summary>
    /// 播放FX_Grown特效
    /// </summary>
    public void PlayTimelineWithParticleSystem()
    {
        if (hasPlayedEffect || fxGrownPrefab == null) return;

        Vector3 pos = meshObject2 != null ? meshObject2.transform.position : transform.position;
        PlayEffect(fxGrownPrefab, pos);
        hasPlayedEffect = true;
    }

    /// <summary>
    /// 播放FX_Planted种植特效
    /// </summary>
    public void PlayPlantedEffect()
    {
        if (hasPlayedPlantedEffect || fxPlantedPrefab == null) return;

        Vector3 pos = fxSpot != null ? fxSpot.position : transform.position;
        PlayEffect(fxPlantedPrefab, pos);
        hasPlayedPlantedEffect = true;
        // 播放种植声效
        PlayPlantedSound();
    }

    /// <summary>
    /// 统一的特效播放方法
    /// </summary>
    private void PlayEffect(GameObject effectPrefab, Vector3 position)
    {
        // 随机旋转和缩放
        float randomY = Random.Range(0f, 360f);
        float randomScale = Random.Range(minScale, maxScale);

        // 实例化特效
        GameObject fx = Instantiate(effectPrefab, position, Quaternion.Euler(0, randomY, 0));

        // 应用Transform缩放
        fx.transform.localScale = Vector3.one * randomScale;

        // 直接修改ParticleSystem的大小属性
        foreach (var ps in fx.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            if (main.startSize.mode == ParticleSystemCurveMode.Constant)
            {
                main.startSize = main.startSize.constant * randomScale;
            }
            else if (main.startSize.mode == ParticleSystemCurveMode.TwoConstants)
            {
                main.startSize = new ParticleSystem.MinMaxCurve(
                    main.startSize.constantMin * randomScale,
                    main.startSize.constantMax * randomScale
                );
            }
        }

        Debug.Log($"特效缩放应用: {randomScale:F2} (范围: {minScale:F2} - {maxScale:F2})，预制体: {effectPrefab.name}");

        // 重置并播放所有组件
        StartCoroutine(ResetAndPlayEffect(fx));
    }

    /// <summary>
    /// 重置并播放特效的所有组件
    /// </summary>
    private System.Collections.IEnumerator ResetAndPlayEffect(GameObject fx)
    {
        // 停止所有组件
        foreach (var ps in fx.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Clear();
        }

        foreach (var director in fx.GetComponentsInChildren<PlayableDirector>())
        {
            director.Stop();
            director.time = 0;
        }

        // 等待一帧确保停止生效
        yield return null;

        // 重新播放所有组件
        foreach (var ps in fx.GetComponentsInChildren<ParticleSystem>())
            ps.Play();

        foreach (var director in fx.GetComponentsInChildren<PlayableDirector>())
            director.Play();

        foreach (var animator in fx.GetComponentsInChildren<Animator>())
            if (animator.runtimeAnimatorController != null)
                animator.Play(0, 0, 0f);
    }

    /// <summary>
    /// 停止所有FX_Grown特效播放
    /// </summary>
    public void StopAllFXGrownEffects()
    {
        // 查找场景中所有名为"FX_Grown"的GameObject实例
        GameObject[] fxInstances = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in fxInstances)
        {
            if (obj.name.Contains("FX_Grown"))
            {
                StopEffectsInGameObject(obj);
            }
        }
        Debug.Log("所有FX_Grown特效已停止");
    }

    /// <summary>
    /// 停止指定GameObject中的所有特效
    /// </summary>
    private void StopEffectsInGameObject(GameObject targetObject)
    {
        // 停止所有ParticleSystem
        ParticleSystem[] particleSystems = targetObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }

        // 停止所有PlayableDirector
        PlayableDirector[] playableDirectors = targetObject.GetComponentsInChildren<PlayableDirector>();
        foreach (PlayableDirector director in playableDirectors)
        {
            director.Stop();
        }

        // 停止所有Animator
        Animator[] animators = targetObject.GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            animator.enabled = false;
        }
    }

    /// <summary>
    /// 播放种植声效
    /// </summary>
    private void PlayPlantedSound()
    {
        if (plantedSoundClip != null && audioSource != null)
        {
            // 播放声效
            audioSource.PlayOneShot(plantedSoundClip);
            Debug.Log($"播放种植声效: {plantedSoundClip.name}");
        }
        else
        {
            Debug.LogWarning("种植声效未设置或AudioSource组件缺失！");
        }
    }

}
