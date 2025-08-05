using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigAnt : MonoBehaviour, IGetObjectClass
{
    public string BigClass => "Animal";

    public string SmallClass => "BigAnt";

    private Rigidbody thisRb;

    private bool hasTarget = false;
    private GameObject target;


    


    // Start is called before the first frame update
    void Start()
    {
        Events.OnCreateObject.Invoke(this);
        thisRb = GetComponent<Rigidbody>();
    } 

    // Update is called once per frame
    void Update()
    {
        if(hasTarget)
        {
            //thisRb.velocity = (target.transform.position - transform.position).normalized * 10;
            MoveToTarget();
        }
        else
        {

            // thisRb.velocity = Vector3.zero;
            FindATarget();
        }
    }

    private void FindATarget()
    {
        // GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
    }

    private void MoveToTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        thisRb.velocity = direction * 10;
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
        throw new System.NotImplementedException();
    }



    // test func

}
