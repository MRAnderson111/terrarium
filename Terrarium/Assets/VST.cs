using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;

public class VST : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake(){
        PXR_Manager.EnableVideoSeeThrough = true;
    }

    void OnApplicationPause(bool pause){
        if(!pause){
            PXR_Manager.EnableVideoSeeThrough = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
