using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.effects.trails.mesh_impl;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEngine.Serialization;


namespace rvinowise.unity {

[RequireComponent(typeof(Rigidbody2D))]
public class Plasma_ball : MonoBehaviour {

    
    public Rigidbody2D rigid_body;
    public IDestructible destructible;

    public float minimum_velocity = 1f;
    
    void Awake() {
        if (!rigid_body) {
            rigid_body = GetComponent<Rigidbody2D>();
        }
        destructible = GetComponent<IDestructible>();
    }

    private void Update() {
        if (rigid_body.velocity.magnitude <= minimum_velocity) {
            destructible.die();
        }
    }


}


}