using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour, IGetObjectClass
{
    public string BigClass => "Plant";

    public string SmallClass => "Grass";

    // Start is called before the first frame update
    void Start()
    {
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

    
    
}
