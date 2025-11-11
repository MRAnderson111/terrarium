using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPoint : MonoBehaviour
{
    public GameObject fxPrefab; // 特效预制体
    public CreateManager createManager; // CreateManager的引用

    [Header("音效设置")]
    public AudioClip collectSoundClip; // 收集时播放的声效
    private AudioSource audioSource; // 音频源组件

    private Vector3 originalScale; // 原始大小
    private Vector3 targetScale; // 目标大小
    private bool isHitByRay = false; // 是否被射线射中
    private float scaleSpeed = 5f; // 缩放速度
    private const float SCALE_MULTIPLIER = 1.3f; // 放大倍数（30%）
    
    // Start is called before the first frame update
    void Start()
    {
        // 保存原始大小
        originalScale = transform.localScale;
        targetScale = originalScale;

        // 获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 如果没有设置CreateManager引用，尝试获取
        if (createManager == null)
        {
            createManager = CreateManager.Instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 检查是否被射线击中
        bool currentlyHit = IsHitByRay();
        
        // 如果射中状态发生变化
        if (currentlyHit != isHitByRay)
        {
            isHitByRay = currentlyHit;
            
            if (isHitByRay)
            {
                // 被射线射中，设置目标大小为原始大小的130%
                targetScale = originalScale * SCALE_MULTIPLIER;
            }
            else
            {
                // 不再被射中，恢复原始大小
                targetScale = originalScale;
            }
        }
        
        // 平滑缩放到目标大小
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        
        // 使用Unity输入系统的按钮按下检测方法
        if (InputActionsManager.Actions.XRIRightInteraction.Activate.WasPressedThisFrame())
        {
            // 检查是否被射线击中
            if (isHitByRay)
            {
                // 生成特效
                if (fxPrefab != null)
                {
                    Instantiate(fxPrefab, transform.position, transform.rotation);
                }

                // 播放收集声效
                PlayCollectSound();

                // 增加金币
                if (createManager != null)
                {
                    createManager.AddGold(100);
                }
                
                // 销毁自身
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 播放收集声效
    /// </summary>
    private void PlayCollectSound()
    {
        if (collectSoundClip != null && audioSource != null)
        {
            // 播放声效
            audioSource.PlayOneShot(collectSoundClip);
            Debug.Log($"播放收集声效: {collectSoundClip.name}");
        }
        else
        {
            Debug.LogWarning("收集声效未设置或AudioSource组件缺失！");
        }
    }

    /// <summary>
    /// 检查当前对象是否被CreateManager的射线击中
    /// </summary>
    /// <returns>如果被击中返回true，否则返回false</returns>
    private bool IsHitByRay()
    {
        if (createManager == null || createManager.rayOrigin == null)
            return false;
            
        // 使用与CreateManager相同的射线
        Ray ray = new Ray(createManager.rayOrigin.position, createManager.rayOrigin.forward);
        RaycastHit hit;
        
        // 检测射线是否击中物体
        if (Physics.Raycast(ray, out hit))
        {
            // 检查击中的物体是否是当前对象
            return hit.collider.gameObject == gameObject;
        }
        
        return false;
    }
}
