using rvinowise.unity.extensions.pooling;

using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;


namespace rvinowise.unity {

public class Persistent_puddle: Puddle {


    //public Pooled_object pooled_object;

    protected override void Awake() {
        base.Awake();
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