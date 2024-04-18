using System;
using System.Collections.Generic;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Wheel: MonoBehaviour {
    
    public Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void set_rotation_speed(float speed) {
        animator.speed = speed;
    }
}

}