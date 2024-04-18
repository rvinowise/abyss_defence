using rvinowise.unity.extensions.pooling;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity {


public class Droplet: 
    MonoBehaviour
{


    public new Rigidbody2D rigidbody;
    public Trajectory_flyer trajectory_flyer;

    public Puddle puddle_prefab;
    
    public float size {
        get {return transform.localScale.x;}
        set {
            transform.localScale = new Vector3(value,value,1);
        }
    }
    public void stain_the_ground() {
        Puddle puddle = puddle_prefab.instantiate<Puddle>();
        puddle.copy_physics_from(this);
        Destroy(gameObject);
    }

    

    
}
}

