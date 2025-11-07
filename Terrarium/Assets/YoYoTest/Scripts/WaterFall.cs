using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFall : MonoBehaviour
{
    [Header("音频设置")]
    public AudioClip waterFallFX; // 瀑布音效

    [Range(0f, 3f)]
    public float volume = 1f; // 音量

    [Range(1f, 100f)]
    public float minDistance = 1f; // 最小距离（全音量范围）

    [Range(5f, 500f)]
    public float maxDistance = 50f; // 最大听到距离

    public bool playOnStart = true; // 是否在开始时播放
    public bool loop = true; // 是否循环播放

    [Header("3D音频设置")]
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic; // 衰减模式

    [Range(0f, 5f)]
    public float dopplerLevel = 1f; // 多普勒效果

    [Range(0f, 360f)]
    public float spread = 0f; // 声音扩散角度

    [Header("位置设置")]
    public Transform waterfall_SFX; // 指定的音频播放位置

    private GameObject audioObject; // 用于播放音频的临时对象

    void Start()
    {
        if (playOnStart && waterFallFX != null)
        {
            // 延迟一帧确保所有组件都已初始化
            StartCoroutine(DelayedPlay());
        }
        else if (waterFallFX == null)
        {
            Debug.LogWarning("WaterFall音效未设置！请在Inspector中设置waterFallFX");
        }
    }

    private IEnumerator DelayedPlay()
    {
        yield return null; // 等待一帧
        PlayWaterFallSound();
    }

    /// <summary>
    /// 在指定位置播放瀑布音效
    /// </summary>
    public void PlayWaterFallSound()
    {
        // 优先使用waterfall_SFX位置，如果没有则使用当前对象位置
        Vector3 playPosition = waterfall_SFX != null ? waterfall_SFX.position : transform.position;
        PlayWaterFallSoundAtPosition(playPosition);
    }

    /// <summary>
    /// 在指定位置播放瀑布音效
    /// </summary>
    /// <param name="position">播放位置</param>
    public void PlayWaterFallSoundAtPosition(Vector3 position)
    {
        if (waterFallFX == null)
        {
            Debug.LogWarning("WaterFall音效未设置！");
            return;
        }

        // 如果需要循环播放，创建持久的AudioSource
        if (loop)
        {
            CreatePersistentAudioSource(position);
        }
        else
        {
            // 使用PlayClipAtPoint播放一次性音效
            AudioSource.PlayClipAtPoint(waterFallFX, position, volume);
        }
    }

    /// <summary>
    /// 创建持久的AudioSource用于循环播放
    /// </summary>
    private void CreatePersistentAudioSource(Vector3 position)
    {
        // 如果已经有音频对象，先销毁
        if (audioObject != null)
        {
            Destroy(audioObject);
        }

        // 创建新的音频对象
        audioObject = new GameObject("WaterFall_AudioSource");
        audioObject.transform.position = position;

        // 添加AudioSource组件
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = waterFallFX;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;

        // 3D音频设置
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = rolloffMode;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.dopplerLevel = dopplerLevel;
        audioSource.spread = spread;
        audioSource.reverbZoneMix = 1f;
        audioSource.bypassEffects = false;
        audioSource.bypassListenerEffects = false;
        audioSource.bypassReverbZones = false;
        audioSource.priority = 64;

        // 播放音频
        audioSource.Play();
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    public void StopWaterFallSound()
    {
        if (audioObject != null)
        {
            Destroy(audioObject);
            audioObject = null;
        }
    }

    /// <summary>
    /// 更新音频位置（如果瀑布对象移动）
    /// </summary>
    public void UpdateAudioPosition()
    {
        if (audioObject != null)
        {
            Vector3 newPosition = waterfall_SFX != null ? waterfall_SFX.position : transform.position;
            audioObject.transform.position = newPosition;
        }
    }

    void Update()
    {
        // 实时更新音频位置（如果waterfall_SFX移动了）
        if (audioObject != null && waterfall_SFX != null)
        {
            if (Vector3.Distance(audioObject.transform.position, waterfall_SFX.position) > 0.1f)
            {
                UpdateAudioPosition();
            }
        }
    }

    void OnDestroy()
    {
        // 清理音频对象
        if (audioObject != null)
        {
            Destroy(audioObject);
        }
    }


}
