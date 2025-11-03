using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBeHurt : MonoBehaviour, IBeHurt
{
    public float healthLimit;
    public float currentHealth;
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float recoverySpeed;


    public float scoreMultiplier= 1f;


    public void BeHurt(float hurtValue)
    {
        currentHealth -= hurtValue;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GetComponent<IGetObjectClass>().Death();
        }
    }

    public float GetHealthScore { get; set; }

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

        GetHealthScore = currentHealth / healthLimit * scoreMultiplier;

        //     BeHurt(10);
        // }
    }
}
