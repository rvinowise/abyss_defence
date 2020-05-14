using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        GameObject game_object, 
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
    
}
