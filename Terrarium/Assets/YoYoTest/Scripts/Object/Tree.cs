using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IGetObjectClass, IGetQuantityLimits
{
    public string BigClass => "Plant";

    public string SmallClass => "Tree";

    public int quantityLimits = 10;
    public int QuantityLimits
    {
        get => quantityLimits;
        set => quantityLimits = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        Events.OnCreateObject.Invoke(this);

        // 生长速度和生长过程现在都由SimpleGrowth组件自己负责
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
