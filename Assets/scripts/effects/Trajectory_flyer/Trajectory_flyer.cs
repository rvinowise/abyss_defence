using System;
using UnityEngine;


namespace rvinowise.unity {

public class Trajectory_flyer: MonoBehaviour {

    public float weight = 0.1f;
    public float size = 1f;

    public float height;
    [HideInInspector]
    public float vertical_velocity = 0f;


    public UnityEngine.Events.UnityEvent on_fell_on_the_ground;
    
    public bool is_on_the_ground() {
        return height <= 0f;
    }

    private void Update() {

        height += vertical_velocity * Time.deltaTime;
        vertical_velocity -= weight * Time.deltaTime;
        
        if (is_on_the_ground()) {
            this.enabled = false;
            transform.localScale = new Vector2(size, size);
            stop_movement();
            if (on_fell_on_the_ground != null) {
                on_fell_on_the_ground.Invoke();
            }
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