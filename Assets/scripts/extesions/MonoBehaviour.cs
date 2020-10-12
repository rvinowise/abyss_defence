using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using extensions.pooling;
using rvinowise.rvi.contracts;
using UnityEngine;

public static partial class Unity_extension
{
    
         
    
    
    public static TComponent get_from_pool<TComponent>(
        this MonoBehaviour prefab_component
    ) 
        where TComponent: MonoBehaviour
    {
        Pooled_object pooled_object = prefab_component.GetComponent<Pooled_object>();
        Contract.Requires(pooled_object != null, "pooled prefabs must have the Pooled_object component");
        TComponent returned_component = pooled_object.instantiate().GetComponent<TComponent>();
        return returned_component;
    }
    
    public static void copy_physics_from(
        this MonoBehaviour in_component,
        MonoBehaviour src_component
    ) {
        Transform dst_transform = in_component.gameObject.transform;
        Transform src_transform = src_component.gameObject.transform;
        dst_transform.position = src_transform.position;
        dst_transform.rotation = src_transform.rotation;
        dst_transform.localScale = src_transform.localScale;
        
    }
    
}
