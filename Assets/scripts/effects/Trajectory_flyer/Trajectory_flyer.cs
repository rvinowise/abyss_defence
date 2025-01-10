#define RVI_DEBUG

using System;
using rvinowise.unity.extensions.pooling;
using UnityEngine;


namespace rvinowise.unity {

public class Trajectory_flyer: MonoBehaviour {

    public float weight = 0.1f;
    public float size = 1f;

    public float height;
    [HideInInspector]
    public float vertical_velocity = 0f;


    public UnityEngine.Events.UnityEvent on_fell_on_the_ground;

    private Pooled_object pooled_object;
    
    #if RVI_DEBUG
    public static int counter = 0;
    public int number = 0;
    #endif
    
    public bool is_on_the_ground() {
        return height <= 0f;
    }

    private void Awake() {
        pooled_object = GetComponent<Pooled_object>();
        // if (GetComponent<Rigidbody2D>() is { } rigidbody2d) {
        //     weight = rigidbody2d.mass;
        // }
        
#if RVI_DEBUG
        number = counter++;
#endif
    }

    public void on_restore_from_pool() {
        if (pooled_object != null) {
        }
    }
    
    private void Update() {

        height += vertical_velocity * Time.deltaTime;
        vertical_velocity -= weight * Time.deltaTime;
        
        if (is_on_the_ground()) {
            #if RVI_DEBUG
            Debug.Log($"Trajectory_flyer::is_on_the_ground for {name} #{number}");
            #endif
            
            enabled = false;
            transform.localScale = new Vector2(size, size);
            stop_movement();
            on_fell_on_the_ground?.Invoke();
        } else {
            float local_scale = size * (height + 1);
            transform.localScale = new Vector2(local_scale, local_scale);
            transform.position = new Vector3(transform.position.x, transform.position.y, -height);
        }
    }
    

    public float get_vertical_impulse_for_landing_at_distance(
        float landing_distance,
        float launching_speed
    ) {
        var time_reaching_target = landing_distance / launching_speed;

        var starting_velocity = 
            - (float)(height - 0.5 * (weight) * Math.Pow(time_reaching_target, 2)) / time_reaching_target;

        return starting_velocity;
    }


    public void stop_movement() {
        var rigid_body = GetComponent<Rigidbody2D>();
        if (rigid_body != null) {
            rigid_body.velocity = Vector2.zero;
            rigid_body.angularVelocity = 0;
        }
    }

}
}