using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBeHurt : MonoBehaviour, IBeHurt
{
    public float healthLimit;
    public float currentHealth;
    public float recoverySpeed;

    public void BeHurt(float hurtValue)
    {
        currentHealth -= hurtValue;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GetComponent<IGetObjectClass>().Death();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth += recoverySpeed * Time.deltaTime;
        if (currentHealth > healthLimit)
        {
            currentHealth = healthLimit;
        }


        // //test
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     BeHurt(10);
        // }
    }
}
