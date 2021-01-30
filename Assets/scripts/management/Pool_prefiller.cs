using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.extensions.pooling;
using UnityEngine;


namespace rvinowise.unity.management {

[Serializable]
public struct Prefilled_prefab{
    public Pooled_object prefab;
    public int qty;
}
public class Pool_prefiller : MonoBehaviour
{
    public GameObject test_prefab;
    public List<Prefilled_prefab> prefabs;
    void Start()
    {
        foreach(var pooled in prefabs) {
            pooled.prefab.prefill_pool(pooled.qty);
        }
    }

    void Update()
    {
        
    }
}

}