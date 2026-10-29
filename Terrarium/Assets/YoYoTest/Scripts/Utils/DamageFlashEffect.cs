using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 受伤时把模型短暂变成红色，再恢复原来的材质颜色。
/// </summary>
public class DamageFlashEffect : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly Color FlashColor = Color.red;
    private const float FlashDuration = 0.35f;

    private readonly List<FlashRendererState> flashStates = new List<FlashRendererState>();
    private bool flashCached;
    private bool isFlashing;
    private Coroutine flashRoutine;

    private struct FlashRendererState
    {
        public Material[] materials;
        public Color[] originalBaseColors;
        public Color[] originalColors;
        public Color[] originalEmissionColors;
        public bool[] hadEmission;
    }

    public static void PlayOn(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        DamageFlashEffect effect = target.GetComponent<DamageFlashEffect>();
        if (effect == null)
        {
            effect = target.AddComponent<DamageFlashEffect>();
        }

        effect.Play();
    }

    public void Play()
    {
        if (!isActiveAndEnabled || isFlashing)
        {
            return;
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private void OnDestroy()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        ApplyFlash(false);
    }

    private IEnumerator FlashRoutine()
    {
        isFlashing = true;
        ApplyFlash(true);
        yield return new WaitForSeconds(FlashDuration);
        ApplyFlash(false);
        isFlashing = false;
        flashRoutine = null;
    }

    private void CacheFlashMaterials()
    {
        if (flashCached)
        {
            return;
        }

        flashStates.Clear();
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
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
}
