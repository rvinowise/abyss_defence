using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using UnityEngine.Events;


namespace rvinowise.effects.physics {

public class Trajectory_flyer: MonoBehaviour {

    public float weight = 0.1f;
    [HideInInspector]
    public float size = 1f;

    public float height;
    [HideInInspector]
    public float vertical_velocity = 0f;


    public UnityEngine.Events.UnityEvent on_fell_on_the_ground;
    
    private void Update() {

        height += vertical_velocity * Time.deltaTime;
        vertical_velocity -= weight * Time.deltaTime;
        
        float local_scale = size * (height + 1);
        transform.localScale = new Vector2(local_scale, local_scale);
        
        if (height <= 0f) {
            if (on_fell_on_the_ground != null) {
                on_fell_on_the_ground.Invoke();
            }
        }

        
    }

}
}