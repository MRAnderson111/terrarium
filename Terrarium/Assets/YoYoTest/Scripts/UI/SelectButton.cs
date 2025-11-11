using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    public GameObject selectPrefab;
    private Button button;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI quantityText;
    public int quantity;
    public int price;
    public Button addButton;

    [Header("特效设置")]
    public GameObject fxGoldCoinPrefab; // FX_GoldCoin特效预制体

    [Header("音效设置")]
    public AudioClip selectSoundClip; // 选择按钮时播放的声效
    private AudioSource audioSource; // 音频源组件

    
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        // 获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        addButton.onClick.AddListener(OnAddButtonClick);
        quantityText.text = quantity.ToString();

        nameText.text = selectPrefab.name;
    }

    void OnAddButtonClick()
    {
        if (CreateManager.Instance != null)
        {
            if (CreateManager.Instance.goldCount >= price)
            {
                CreateManager.Instance.AddGold(-price);
                quantity++;
                quantityText.text = quantity.ToString();

                // 在按钮位置播放FX_GoldCoin特效
                PlayGoldCoinEffect();
            }
            else
            {
                Debug.Log("金币不足，无法购买！");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        Debug.Log("选择预制体：" + selectPrefab.name);
        Events.OnSelectPrefab.Invoke(selectPrefab, this);

        // 播放选择声效
        PlaySelectSound();

        // 触发数量减一的action
        // onQuantityDecrease?.Invoke();
    }
    
    // 独立的让数量减一的方法
    public void DecrementQuantity()
    {
        quantity--;
        quantityText.text = quantity.ToString();
        // Debug.Log("当前数量：" + quantity);
    }

    /// <summary>
    /// 播放选择声效
    /// </summary>
    private void PlaySelectSound()
    {
        if (selectSoundClip != null && audioSource != null)
        {
            // 播放声效
            audioSource.PlayOneShot(selectSoundClip);
            Debug.Log($"播放选择声效: {selectSoundClip.name}");
        }
        else
        {
            Debug.LogWarning("选择声效未设置或AudioSource组件缺失！");
        }
    }

    /// <summary>
    /// 播放FX_GoldCoin特效
    /// </summary>
    private void PlayGoldCoinEffect()
    {
        if (fxGoldCoinPrefab == null)
        {
            Debug.LogWarning("FX_GoldCoin特效预制体未设置！");
            return;
        }

        // 在按钮位置播放特效
        Vector3 effectPosition = transform.position;
        PlayEffect(fxGoldCoinPrefab, effectPosition);
    }

    /// <summary>
    /// 统一的特效播放方法
    /// </summary>
    private void PlayEffect(GameObject effectPrefab, Vector3 position)
    {
        // 实例化特效
        GameObject fx = Instantiate(effectPrefab, position, Quaternion.identity);

        Debug.Log($"在按钮位置播放特效: {effectPrefab.name}，位置: {position}");

        // 重置并播放所有组件
        StartCoroutine(ResetAndPlayEffect(fx));
    }

    /// <summary>
    /// 重置并播放特效的协程
    /// </summary>
    private IEnumerator ResetAndPlayEffect(GameObject fx)
    {
        // 等待一帧确保对象完全实例化
        yield return null;

        // 重置所有粒子系统
        ParticleSystem[] particleSystems = fx.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            ps.Stop();
            ps.Clear();
            ps.Play();
        }

        // 重置所有动画器
        Animator[] animators = fx.GetComponentsInChildren<Animator>();
        foreach (var animator in animators)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        // 等待特效播放完成后销毁（假设特效持续3秒）
        yield return new WaitForSeconds(3f);

        if (fx != null)
        {
            Destroy(fx);
        }
    }
}
