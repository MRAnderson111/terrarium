using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateLimitManager.SetCreateLimit("ant", 100);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CreateLimitManager.LogAllCreateLimit();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            CreateLimitManager.AddToCreateLimit("ant", 10);
        }
    }
}

