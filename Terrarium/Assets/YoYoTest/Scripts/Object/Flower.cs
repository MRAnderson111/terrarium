using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour, IGetObjectClass, IGetQuantityLimits
{
    public string BigClass => "Plant";

    public string SmallClass => "Flower";

    public int quantityLimits = 10;
    public int QuantityLimits
    {
        get => quantityLimits;
        set => quantityLimits = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<SimpleGrowth>() == null)
            gameObject.AddComponent<SimpleGrowth>();

        Events.OnCreateObject.Invoke(this);
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
