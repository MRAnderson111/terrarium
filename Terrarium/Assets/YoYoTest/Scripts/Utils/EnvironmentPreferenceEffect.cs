using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 根据偏好值与当前生态缸环境的相对偏离，计算状态倍率、需水量和致死判定。
/// 偏离 = |当前值 - 偏好值| / 偏好值。
/// </summary>
public static class EnvironmentPreferenceUtility
{
    public const float HalfPenaltyDeviation = 0.5f;
    public const float LethalDeviation = 0.75f;
    public const float LethalDamagePerSecond = 10f;
    public const float MinWaterNeed = 60f;
    public const float NeutralWaterNeed = 100f;
    public const float MaxWaterNeed = 140f;

    public static EnvironmentalData GetCurrentEnvironment()
    {
        if (EnvironmentalParaManager.Instance == null)
        {
            return default;
        }

        return EnvironmentalParaManager.Instance.EnvironmentalData;
    }

    public static float GetRelativeDeviation(float current, float preferred)
    {
        float safePreferred = Mathf.Max(preferred, 0.0001f);
        return Mathf.Abs(current - preferred) / safePreferred;
    }

    public static float GetMaxRelativeDeviation(float preferredTemperature, float preferredHumidity, float preferredSunlight)
    {
        EnvironmentalData data = GetCurrentEnvironment();
        return Mathf.Max(
            GetRelativeDeviation(data.temperature, preferredTemperature),
            GetRelativeDeviation(data.humidity, preferredHumidity),
            GetRelativeDeviation(data.sunshine, preferredSunlight));
    }

    public static float GetStatMultiplier(float deviation)
    {
        return deviation >= HalfPenaltyDeviation ? 0.5f : 1f;
    }

    public static bool IsLethal(float deviation)
    {
        return deviation >= LethalDeviation;
    }

    /// <summary>
    /// 光照和温度越高需水量越大：中间值对应 100，最低 60，最高 140。
    /// </summary>
    public static float GetWaterNeed(float sunshine, float temperature)
    {
        float factor = Mathf.Clamp01((Mathf.Clamp01(sunshine) + Mathf.Clamp01(temperature)) * 0.5f);
        return Mathf.Lerp(MinWaterNeed, MaxWaterNeed, factor);
    }
}

/// <summary>
/// 把偏好偏离应用到生命值、生长速度、移动速度，并在极端环境下扣血闪红直至死亡。
/// </summary>
public class EnvironmentPreferenceDriver
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly Color FlashColor = Color.red;
    private const float FlashDuration = 0.35f;

    private readonly MonoBehaviour owner;
    private readonly bool affectMoveSpeed;
    private readonly bool affectGrowthSpeed;
    private readonly bool affectHealth;

    private IBeHurt beHurt;
    private SimpleBeHurt simpleBeHurt;
    private NewAnimalBeHurt animalBeHurt;
    private IGrowth growth;
    private NewAntTest ant;
    private NavMeshAgent agent;
    private readonly List<FlashRendererState> flashStates = new List<FlashRendererState>();

    private bool capturedHealth;
    private bool capturedGrowth;
    private bool capturedMoveSpeed;
    private bool capturedRecovery;
    private bool flashCached;
    private bool wasLethal;
    private float baseHealthLimit;
    private float baseGrowthSpeed;
    private float baseMoveSpeed;
    private float baseRecoverySpeed;
    private float lastStatMultiplier = 1f;
    private float lethalTimer = 1f;
    private Coroutine flashRoutine;

    public float StatMultiplier { get; private set; } = 1f;
    public float CurrentDeviation { get; private set; }

    private struct FlashRendererState
    {
        public Renderer renderer;
        public Material[] materials;
        public Color[] originalBaseColors;
        public Color[] originalColors;
        public Color[] originalEmissionColors;
        public bool[] hadEmission;
    }

    public EnvironmentPreferenceDriver(MonoBehaviour owner, bool affectHealth, bool affectGrowthSpeed, bool affectMoveSpeed)
    {
        this.owner = owner;
        this.affectHealth = affectHealth;
        this.affectGrowthSpeed = affectGrowthSpeed;
        this.affectMoveSpeed = affectMoveSpeed;
    }

    public void Tick(float preferredTemperature, float preferredHumidity, float preferredSunlight)
    {
        if (owner == null)
        {
            return;
        }

        CurrentDeviation = EnvironmentPreferenceUtility.GetMaxRelativeDeviation(
            preferredTemperature,
            preferredHumidity,
            preferredSunlight);
        StatMultiplier = EnvironmentPreferenceUtility.GetStatMultiplier(CurrentDeviation);

        ResolveReferences();
        CaptureBases();
        ApplyStatMultiplier(StatMultiplier);

        bool isLethal = EnvironmentPreferenceUtility.IsLethal(CurrentDeviation);
        bool canTakeLethalDamage = beHurt != null && beHurt.CurrentHealth > 0f;
        SetRecoveryPaused(isLethal && canTakeLethalDamage);

        if (isLethal && canTakeLethalDamage)
        {
            if (!wasLethal)
            {
                lethalTimer = 1f;
            }

            UpdateLethalDamage();
            wasLethal = true;
        }
        else if (!isLethal)
        {
            wasLethal = false;
        }
    }

    public void Dispose()
    {
        if (flashRoutine != null && owner != null)
        {
            owner.StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        ApplyFlash(false);
        SetRecoveryPaused(false);
    }

    private void ResolveReferences()
    {
        if (beHurt == null)
        {
            beHurt = owner.GetComponent<IBeHurt>();
            simpleBeHurt = owner.GetComponent<SimpleBeHurt>();
            animalBeHurt = owner.GetComponent<NewAnimalBeHurt>();
        }

        if (growth == null)
        {
            growth = owner.GetComponent<IGrowth>();
        }

        if (ant == null)
        {
            ant = owner.GetComponent<NewAntTest>();
        }

        if (agent == null)
        {
            agent = owner.GetComponent<NavMeshAgent>();
        }
    }

    private void CaptureBases()
    {
        if (affectHealth && !capturedHealth)
        {
            float limit = GetHealthLimit();
            if (limit > 0f)
            {
                baseHealthLimit = limit;
                capturedHealth = true;
            }
        }

        if (affectGrowthSpeed && !capturedGrowth && growth != null && growth.IsSpeedSet)
        {
            baseGrowthSpeed = growth.GrowthSpeed;
            capturedGrowth = true;
        }

        if (affectMoveSpeed && !capturedMoveSpeed)
        {
            if (ant != null)
            {
                baseMoveSpeed = ant.moveSpeed;
                capturedMoveSpeed = true;
            }
            else if (agent != null)
            {
                baseMoveSpeed = agent.speed;
                capturedMoveSpeed = true;
            }
        }

        if (!capturedRecovery)
        {
            baseRecoverySpeed = GetRecoverySpeed();
            capturedRecovery = true;
        }
    }

    private void ApplyStatMultiplier(float multiplier)
    {
        if (affectHealth && capturedHealth)
        {
            float newLimit = baseHealthLimit * multiplier;
            SetHealthLimit(newLimit);
            if (beHurt != null && beHurt.CurrentHealth > newLimit)
            {
                beHurt.CurrentHealth = newLimit;
            }
        }

        if (affectGrowthSpeed && capturedGrowth && growth != null)
        {
            if (Mathf.Approximately(multiplier, 1f))
            {
                if (!Mathf.Approximately(lastStatMultiplier, 1f))
                {
                    growth.GrowthSpeed = baseGrowthSpeed;
                }
                else
                {
                    baseGrowthSpeed = growth.GrowthSpeed;
                }
            }
            else
            {
                growth.GrowthSpeed = baseGrowthSpeed * multiplier;
            }
        }

        if (affectMoveSpeed && capturedMoveSpeed)
        {
            float newSpeed = baseMoveSpeed * multiplier;
            if (ant != null)
            {
                ant.moveSpeed = newSpeed;
            }

            if (agent != null)
            {
                agent.speed = newSpeed;
            }
        }

        lastStatMultiplier = multiplier;
    }

    private void UpdateLethalDamage()
    {
        lethalTimer -= Time.deltaTime;
        if (lethalTimer > 0f)
        {
            return;
        }

        lethalTimer = 1f;
        if (beHurt == null || beHurt.CurrentHealth <= 0f)
        {
            return;
        }

        ApplyFlash(true);
        beHurt.BeHurt(EnvironmentPreferenceUtility.LethalDamagePerSecond);

        if (owner == null || !owner.isActiveAndEnabled)
        {
            return;
        }

        if (flashRoutine != null)
        {
            owner.StopCoroutine(flashRoutine);
        }

        flashRoutine = owner.StartCoroutine(ClearFlashAfterDelay());
    }

    private IEnumerator ClearFlashAfterDelay()
    {
        yield return new WaitForSeconds(FlashDuration);
        ApplyFlash(false);
        flashRoutine = null;
    }

    private void CacheFlashMaterials()
    {
        if (flashCached || owner == null)
        {
            return;
        }

        flashStates.Clear();
        Renderer[] renderers = owner.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || renderer is ParticleSystemRenderer)
            {
                continue;
            }

            Material[] materials = renderer.materials;
            FlashRendererState state = new FlashRendererState
            {
                renderer = renderer,
                materials = materials,
                originalBaseColors = new Color[materials.Length],
                originalColors = new Color[materials.Length],
                originalEmissionColors = new Color[materials.Length],
                hadEmission = new bool[materials.Length]
            };

            for (int m = 0; m < materials.Length; m++)
            {
                Material material = materials[m];
                if (material == null)
                {
                    continue;
                }

                state.originalBaseColors[m] = material.HasProperty(BaseColorId)
                    ? material.GetColor(BaseColorId)
                    : Color.white;
                state.originalColors[m] = material.HasProperty(ColorId)
                    ? material.GetColor(ColorId)
                    : material.color;
                state.originalEmissionColors[m] = material.HasProperty(EmissionColorId)
                    ? material.GetColor(EmissionColorId)
                    : Color.black;
                state.hadEmission[m] = material.IsKeywordEnabled("_EMISSION");
            }

            flashStates.Add(state);
        }

        flashCached = true;
    }

    private void ApplyFlash(bool flashing)
    {
        CacheFlashMaterials();

        for (int i = 0; i < flashStates.Count; i++)
        {
            FlashRendererState state = flashStates[i];
            if (state.materials == null)
            {
                continue;
            }

            for (int m = 0; m < state.materials.Length; m++)
            {
                Material material = state.materials[m];
                if (material == null)
                {
                    continue;
                }

                if (flashing)
                {
                    if (material.HasProperty(BaseColorId))
                    {
                        material.SetColor(BaseColorId, FlashColor);
                    }

                    if (material.HasProperty(ColorId))
                    {
                        material.SetColor(ColorId, FlashColor);
                    }

                    material.color = FlashColor;

                    if (material.HasProperty(EmissionColorId))
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor(EmissionColorId, FlashColor);
                    }
                }
                else
                {
                    if (material.HasProperty(BaseColorId))
                    {
                        material.SetColor(BaseColorId, state.originalBaseColors[m]);
                    }

                    if (material.HasProperty(ColorId))
                    {
                        material.SetColor(ColorId, state.originalColors[m]);
                    }

                    material.color = state.originalColors[m];

                    if (material.HasProperty(EmissionColorId))
                    {
                        material.SetColor(EmissionColorId, state.originalEmissionColors[m]);
                        if (!state.hadEmission[m])
                        {
                            material.DisableKeyword("_EMISSION");
                        }
                    }
                }
            }
        }
    }

    private void SetRecoveryPaused(bool paused)
    {
        if (!capturedRecovery)
        {
            return;
        }

        float recovery = paused ? 0f : baseRecoverySpeed;
        if (simpleBeHurt != null)
        {
            simpleBeHurt.recoverySpeed = recovery;
        }

        if (animalBeHurt != null)
        {
            animalBeHurt.recoverySpeed = recovery;
        }
    }

    private float GetRecoverySpeed()
    {
        if (simpleBeHurt != null)
        {
            return Mathf.Max(0f, simpleBeHurt.recoverySpeed);
        }

        if (animalBeHurt != null)
        {
            return Mathf.Max(0f, animalBeHurt.recoverySpeed);
        }

        return 0f;
    }

    private float GetHealthLimit()
    {
        if (simpleBeHurt != null)
        {
            return simpleBeHurt.healthLimit;
        }

        if (animalBeHurt != null)
        {
            return animalBeHurt.healthLimit;
        }

        return 0f;
    }

    private void SetHealthLimit(float value)
    {
        if (simpleBeHurt != null)
        {
            simpleBeHurt.healthLimit = value;
        }

        if (animalBeHurt != null)
        {
            animalBeHurt.healthLimit = value;
        }
    }
}
