using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour, IGetObjectClass, IGetQuantityLimits
{
    public string BigClass => "Plant";

    public string SmallClass => "Grass";

    public int quantityLimits = 10;
    public int QuantityLimits => quantityLimits;

    private float growthSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        Events.OnCreateObject.Invoke(this);

        // 检测地面并设置生长速度
        GroundDetectionUtils.SetGrowthSpeedFromGroundSimple(transform.position, (fertility) => growthSpeed = growthSpeed * fertility);

        // 设置生长速度
        GetComponent<IGrowth>().GrowthSpeed = growthSpeed;
        GetComponent<IGrowth>().Growth();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        // Events.OnSelectPrefab.AddListener(OnSelectPrefab);
    }


    private void OnDestroy()
    {
        Events.OnDestroyObject.Invoke(this);
    }

    public void Death()
    {
        Destroy(gameObject);
    }



}
