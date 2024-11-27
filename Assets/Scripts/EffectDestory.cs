using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EffectDestory : MonoBehaviour
{
    public float destoryTime;

    void Start()
    {
        Destroy(gameObject, destoryTime);
    }

    void Update()
    {

    } 
}
