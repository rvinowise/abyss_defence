using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.extensions.pooling;
using rvinowise.contracts;
using UnityEngine;
using rvinowise.unity.extensions.attributes;

namespace rvinowise.unity.extensions {


public static partial class Unity_extension {


    [called_by_prefab]
    public static TComponent get_from_pool<TComponent>(
        this Component prefab_component
    )
        where TComponent : Component 
    {
        Pooled_object pooled_object = prefab_component.GetComponent<Pooled_object>();
        Contract.Requires(pooled_object != null, "pooled prefabs must have the Pooled_object component");
        TComponent returned_component = pooled_object.get_from_pool<TComponent>();
        return returned_component;
    }

    [called_by_prefab]
    public static TComponent get_from_pool<TComponent>(
        this Component prefab_component,
        Vector2 in_position,
        Quaternion in_rotation
    )
        where TComponent : Component 
    {
        Pooled_object pooled_object = prefab_component.GetComponent<Pooled_object>();
        Contract.Requires(pooled_object != null, "pooled prefabs must have the Pooled_object component");
        TComponent returned_component = pooled_object.get_from_pool<TComponent>(
            in_position, in_rotation
        );
        return returned_component;
    }

    [called_by_prefab]
    public static TComponent instantiate<TComponent>(
        this Component prefab_component
    )
        where TComponent : Component 
    {
        if (prefab_component.GetComponent<Pooled_object>() is Pooled_object pooled_object) {
            return pooled_object.get_from_pool<TComponent>();
        }
        return GameObject.Instantiate(prefab_component).GetComponent<TComponent>();
    }

    public static void copy_physics_from(
        this Component in_component,
        Component src_component
    ) {
        Transform dst_transform = in_component.gameObject.transform;
        Transform src_transform = src_component.gameObject.transform;
        dst_transform.position = src_transform.position;
        dst_transform.rotation = src_transform.rotation;
        dst_transform.localScale = src_transform.localScale;

    }

    private static ISet<Type> ignored_fields = new HashSet<Type>() {
        typeof(UnityEngine.Events.UnityEvent)
    };

    public static void copy_fields_from(
        this Component dst_component,
        Component src_component
    ) {
        var type = dst_component.GetType();
        Contract.Requires(type == src_component.GetType());
        foreach (var field in type.GetFields())
        {
            if (ignored_fields.Any(field_type => field_type == field.FieldType)) {
                continue;
            }
            field.SetValue(dst_component, field.GetValue(src_component));
        } 
    }
    public static void copy_enabledness(
        this Component dst_component,
        Component src_component
    ) {
        if (
            (dst_component is MonoBehaviour dst_behaviour)
            &&
            (src_component is MonoBehaviour src_behaviour)
        ) {
            if (dst_behaviour.enabled != src_behaviour.enabled) {
                dst_behaviour.enabled = src_behaviour.enabled;
            }
        }
    }
    

    

}
}