using rvinowise.unity.extensions.pooling;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity {


public class Pooled_droplet: 
    MonoBehaviour
{

    // exists only in the Prefab, provides pooling for all the instances of it
    public Object_pool pool { get; set; } 

    public new Rigidbody2D rigidbody;
    public Pooled_object pooled_object;
    public Trajectory_flyer trajectory_flyer;


    public Puddle puddle_prefab;
    
    public float size {
        get {return transform.localScale.x;}
        set {
            transform.localScale = new Vector3(value,value,1);
        }
    }

    void OnEnable() {
        trajectory_flyer.enabled = true;
    }
    public void stain_the_ground() {
        Puddle puddle = puddle_prefab.get_from_pool<Puddle>();
        puddle.copy_physics_from(this);
        pooled_object.destroy();
    }

    

    
}
}

