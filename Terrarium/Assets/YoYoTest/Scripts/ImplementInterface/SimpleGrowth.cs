using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class SimpleGrowth : MonoBehaviour, IGrowth
{
    [Header("环境偏好（0-1）")]
    [SerializeField, Range(0f, 1f)]
    private float preferredTemperature = 0.5f;
    [SerializeField, Range(0f, 1f)]
    private float preferredHumidity = 0.5f;
    [SerializeField, Range(0f, 1f)]
    private float preferredSunlight = 0.5f;

    public float PreferredTemperature => preferredTemperature;
    public float PreferredHumidity => preferredHumidity;
    public float PreferredSunlight => preferredSunlight;

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

    private EnvironmentPreferenceDriver preferenceDriver;

    [Header("特效随机化设置")]
    // 最小缩放值
    public float minScale = 0.4f;
    // 最大缩放值
    public float maxScale = 0.75f;

    void Awake()
    {
        preferenceDriver = new EnvironmentPreferenceDriver(this, true, true, false);
        ResolveGrowthTargets();
        if (meshObject == null)
        {
            Debug.LogWarning($"{name} 没有可生长的模型，生长已跳过", this);
            return;
        }

        originalScale = meshObject.transform.localScale;
        PlayPlantedEffect();
        meshObject.transform.localScale = originalScale * 0.1f;

        if (meshObject2 != null && meshObject2 != meshObject)
            meshObject2.SetActive(false);

        SetGrowthSpeedFromGround();
    }

    void Update()
    {
        preferenceDriver?.Tick(preferredTemperature, preferredHumidity, preferredSunlight);
    }

    void OnDestroy()
    {
        preferenceDriver?.Dispose();
    }

    /// <summary>
    /// 场景里有的植物把 meshObject 指到了模型资源而不是实例，或者根本没挂生长模型。
    /// 这两种情况都改成使用自己子层级里真正的渲染物体。
    /// </summary>
    private void ResolveGrowthTargets()
    {
        if (!IsUsableTarget(meshObject))
            meshObject = null;
        if (!IsUsableTarget(meshObject2))
            meshObject2 = null;
        if (meshObject != null)
            return;

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        float bestSize = -1f;
        foreach (Renderer renderer in renderers)
        {
            if (renderer is ParticleSystemRenderer)
                continue;

            string objectName = renderer.gameObject.name;
            if (objectName.Contains("Sphere") || objectName.Contains("FX") || objectName.Contains("Coverage"))
                continue;

            float size = renderer.bounds.extents.sqrMagnitude;
            if (size > bestSize)
            {
                bestSize = size;
                meshObject = renderer.gameObject;
            }
        }
    }

    private bool IsUsableTarget(GameObject target)
    {
        return target != null && target.transform.IsChildOf(transform);
    }

    /// <summary>
    /// 检测地面并设置生长速度，然后开始生长。
    /// 肥沃度是 0 到 100 的配置值，按比例换成温和的速度倍率，避免肥沃地面瞬间长完、贫瘠地面完全不动。
    /// </summary>
    private void SetGrowthSpeedFromGround()
    {
        int fertility = GroundDetectionUtils.GetGroundFertility(transform.position, 20f, transform);
        if (fertility > 0)
        {
            float multiplier = Mathf.Clamp(fertility / 20f, 0.35f, 2.5f);
            GrowthSpeed = defaultGrowthSpeed * multiplier;
        }
        else
        {
            GrowthSpeed = defaultGrowthSpeed;
        }

        isSpeedSet = true;
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
            

            // 每帧更新一次
            yield return null;
        }



        // 播放带timeline的particle system特效
        PlayTimelineWithParticleSystem();

        // 检查是否有meshObject2，如果有则切换mesh
        if (meshObject2 != null)
        {
            // 关闭当前mesh
            meshObject.SetActive(false);
            // 显示meshObject2
            meshObject2.SetActive(true);
        }

        // 释放研究点
        if (TryGetComponent<IReleaseResearchPoint>(out var releaseComponent))
        {
            // 根据概率决定是否生成研究点数
            if (Random.value <= researchPointProbability)
            {
                releaseComponent.ReleaseResearchPoint();
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


}
