using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    public float destroyTime; // Time to destroy the effect

    void Start()
    {
        Destroy(gameObject, destroyTime); // Destroy the game object after a specified time
    }

    void Update()
    {

    } 
}