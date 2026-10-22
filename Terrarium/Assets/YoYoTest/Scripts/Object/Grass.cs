using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour, IGetObjectClass, IGetQuantityLimits
{
    public string bigClass = "Plant";
    public string BigClass => bigClass;
    public string smallClass = "Grass";
    public string SmallClass => smallClass;

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
