using System;
using System.Collections.Generic;
using System.Linq;
using Animancer;
using rvinowise.unity.effects.trails.mesh_impl;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEditor;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace rvinowise.unity {



public class Item_carrier: 
    MonoBehaviour 
    ,IDestructible
{

    public List<Carriable_item> carried_items = new List<Carriable_item>();
    
    void Awake() {
        if (!carried_items.Any()) {
            carried_items = GetComponentsInChildren<Carriable_item>().ToList();
        }
        foreach (var carried_item in carried_items) {
            if (!carried_item) continue;
            carried_item.pick_up();
        }
    }
    



    public void drop_all_items() {
        foreach (var dropped_item in carried_items) {
            drop_item(dropped_item);
        }
    }

    public void drop_item(Carriable_item dropped_item) {
        if (dropped_item == null) return;
        dropped_item.drop();
        push_item_aside(dropped_item.rigidbody2d);
    }

    private void push_item_aside(Rigidbody2D item) {
        var vector_from_owner = item.position - (Vector2)transform.position;
        item.AddForce(vector_from_owner*6000);
        item.AddTorque(-20+Random.Range(0, 40));
    }
    
    public void die() {
        drop_all_items();
    }


}


}