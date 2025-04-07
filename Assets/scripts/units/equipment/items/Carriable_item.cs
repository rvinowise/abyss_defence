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



public class Carriable_item: 
    MonoBehaviour {

    public Rigidbody2D rigidbody2d;
    public Opening_box opening_box;
    void Awake() {
        rigidbody2d = GetComponent<Rigidbody2D>();
        opening_box = GetComponent<Opening_box>();
        
        
    }

    public bool is_started_flying = false;
    private void FixedUpdate() {
        if (rigidbody2d.simulated) {
            is_started_flying = true;
        }
    }

    public void pick_up() {
        is_started_flying = false;
        rigidbody2d.simulated = false;
    }

    public void drop() {
        transform.SetParent(null,true);
        
        transform.set_z(Map.instance.ground_z-0.1f);
        rigidbody2d.simulated = true;
    }
    
    public bool is_dropped() {
        return is_started_flying;
    }
    private void Update() {
        if (
            (is_dropped())&&
            (rigidbody2d.velocity.magnitude < 0.01)&&
            (rigidbody2d.angularVelocity < 0.01)
        ){ 
            if ((opening_box)&&(!opening_box.is_opening)) {
                opening_box.open();
            }
        }
}


}


}