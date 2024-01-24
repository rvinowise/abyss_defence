using System.Collections.Generic;
using rvinowise.unity.extensions.pooling;
using UnityEngine;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;

namespace rvinowise.unity.extensions {
public static partial class Unity_extension
{

    /*public static IEnumerable<T> get_components_which_are<T>(this UnityEngine.GameObject game_object) {
        List<T> result;

        return result;
    }*/

    public static TComponent add_component<TComponent>(this GameObject game_object)
    where TComponent: UnityEngine.Component
    {
        game_object.AddComponent<TComponent>();
        return game_object.GetComponent<TComponent>();
    }

    public static IList<Transform> direct_children(this GameObject game_object) {
        IList < Transform > children = new List<Transform>();
        foreach (Transform child in game_object.transform) {
            children.Add(child);
        }
        return children;
    }
    
    public static void set_sorting_layer(
        this GameObject game_object, 
        string layer,
        int bottom_level
    ) {
        set_sorting_layer_recursive(game_object, layer, bottom_level);
    }
         
    private static void set_sorting_layer_recursive(
        this GameObject game_object, 
        string layer,
        int bottom_level    
    ) {
        SpriteRenderer sprite = game_object.gameObject.GetComponent<SpriteRenderer>();
        if (sprite)
        {
            sprite.sortingLayerName = layer;
            float local_order = ((float)sprite.sortingOrder % 1);
            sprite.sortingOrder = bottom_level;// + local_order;
        }
             
        foreach (Transform child in game_object.transform) {
            set_sorting_layer_recursive(child.gameObject, layer, bottom_level);
        }
    }

    [called_by_prefab]
    public static TComponent get_from_pool<TComponent>(
        this GameObject prefab   
    ) 
        where TComponent: MonoBehaviour
    {
        Pooled_object pooled_object = prefab.GetComponent<Pooled_object>();
        Contract.Requires(pooled_object != null, "pooled prefabs must have the Pooled_object component");
        TComponent returned_component = pooled_object.pool.get().GetComponent<TComponent>();
        return returned_component;
    }

    public static void destroy(
        this GameObject game_object
    ) {
        Pooled_object pooled_object = game_object.GetComponent<Pooled_object>();
        if (pooled_object != null) {
            pooled_object.destroy();
        } else {
            UnityEngine.Object.Destroy(game_object);
        }
    }

    public static void copy_physics_from(
        this GameObject in_object,
        GameObject src_object
    ) {
        in_object.transform.position = src_object.transform.position;
        in_object.transform.rotation = src_object.transform.rotation;
        in_object.transform.localScale = src_object.transform.localScale;
        
    }

    public static bool is_prefab(
        this GameObject in_object
    ) {
        return in_object.scene.name == null;
    }
    
}
}