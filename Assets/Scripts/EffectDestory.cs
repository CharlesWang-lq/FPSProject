using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class EffectDestroy : MonoBehaviour
{
    public float destroyTime; 

    void Start()
    {
        Destroy(gameObject, destroyTime); 
    }

    void Update()
    {
        // No updates needed for this script
    } 
}