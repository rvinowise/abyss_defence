using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.extensions.pooling;

using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using rvinowise;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity.effects.liquids {

public class Puddle: MonoBehaviour {

    public float size;

    //public Pooled_object pooled_object;

    public void Awake() {
        transform.scale(size);
    }

    [called_in_animation]
    public void on_animation_ends() {
        GetComponent<Pooled_object>().destroy();
    }

    [called_in_animation]
    public void create_static_stain() {
        GetComponent<Leaving_persistent_mesh_residue>().leave_persistent_residue();
        
    }

}
}