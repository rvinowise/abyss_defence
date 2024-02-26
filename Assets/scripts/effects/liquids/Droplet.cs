using rvinowise.unity.extensions.pooling;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity {

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Trajectory_flyer))]

public class Droplet: 
    MonoBehaviour
{

    // exists only in the Prefab, provides pooling for all the instances of it
    public Object_pool pool { get; set; } 

    [HideInInspector]
    public new Rigidbody2D rigidbody;
    [HideInInspector]
    public Pooled_object pooled_object;
    [HideInInspector]
    public Trajectory_flyer trajectory_flyer;
    [HideInInspector] SpriteRenderer sprite_renderer;

    [HideInInspector]
    public float vertical_velocity = 0f;

    public Puddle puddle_prefab;
    
    public float size {
        get {return transform.localScale.x;}
        set {
            transform.localScale = new Vector3(value,value,1);
        }
    }

    public void Awake() {
        
        rigidbody = GetComponent<Rigidbody2D>();
        pooled_object = GetComponent<Pooled_object>();
        trajectory_flyer = GetComponent<Trajectory_flyer>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        /* if (puddle_prefab != null) {
            trajectory_flyer.on_fell_on_the_ground.AddListener(stain_the_ground);
        } */
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

