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
            GlobalCooldownManager.Score = 100;
            Debug.Log("Score: " + GlobalCooldownManager.Score);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            
            Debug.Log("Score: " + GlobalCooldownManager.Score);
        }
    }
}

