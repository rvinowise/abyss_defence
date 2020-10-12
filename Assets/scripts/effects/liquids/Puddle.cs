using System;
using System.Collections;
using System.Collections.Generic;
using effects.persistent_residue;
using extensions.pooling;
using UnityEngine;
using rvinowise;
using geometry2d;


namespace rvinowise.effects.liquids {

public class Puddle: MonoBehaviour {

    public float size;

    //public Pooled_object pooled_object;

    public void Awake() {
        transform.scale(size);
    }

    /* invoked from the animation clip */
    public void on_animation_ends() {
        turn_into_static_stain();
    }

    private void turn_into_static_stain() {
        GetComponent<Leaving_persistent_residue>().leave_persistent_image();
        
        GetComponent<Pooled_object>().destroy();
    }

}
}