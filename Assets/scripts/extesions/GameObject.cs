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
    
}
