using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.effects.physics;
using extensions.pooling;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.rvi.contracts;
using UnityEngine.Serialization;

namespace rvinowise.effects.liquids {

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Trajectory_flyer))]

public class Droplet: 
    MonoBehaviour
{

    // exists only in the Prefab, provides pooling for all the instances of it
    public Object_pool pool { get; set; } 

    [HideInInspector]
    public Rigidbody2D rigidbody;
    [HideInInspector]
    public Pooled_object pooled_object;
    [HideInInspector]
    public Trajectory_flyer trajectory_flyer;

    [HideInInspector]
    public float size = 1f;
    //public float height = 1f;
    [HideInInspector]
    public float vertical_velocity = 0f;

    public Puddle puddle_prefab;
    
    public void Awake() {
        
        rigidbody = GetComponent<Rigidbody2D>();
        pooled_object = GetComponent<Pooled_object>();
        if (puddle_prefab != null) {
            trajectory_flyer = GetComponent<Trajectory_flyer>();
            trajectory_flyer.on_fell_on_the_ground.AddListener(stain_the_ground);
        }
    }

    public void stain_the_ground() {
        Puddle puddle = puddle_prefab.GetComponent<Pooled_object>().instantiate<Puddle>();
        puddle.copy_physics_from(this);
        pooled_object.destroy();
    }

    

    
}
}
