using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour, IGetObjectClass
{
    public string BigClass => "Animal";

    public string SmallClass => "Spider";

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

    
    
}
