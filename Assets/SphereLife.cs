using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereLife : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 10f);
    }

}
