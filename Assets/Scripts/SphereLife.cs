using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereLife : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 15f);
    }
    
    

//    private void OnCollisionEnter(Collision other)
//    {
//        //gameObject.GetComponent<Rigidbody>().useGravity = false;
//        gameObject.GetComponent<Rigidbody>().drag = 10;
//    }
}
