using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAnt : MonoBehaviour, IGetObjectClass
{
    public string BigClass => "Animal";

    public string SmallClass => "BigAnt";

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
