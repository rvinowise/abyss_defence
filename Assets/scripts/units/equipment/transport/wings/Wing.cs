using System;
using System.Collections.Generic;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Wing:
    Child_of_group {

    public float provided_impulse = 1;
    public Animator animator;

    private void Awake() {
        if (animator == null) {
            animator = GetComponent<Animator>();
        }
    }

    public void fold() {
        animator.speed = 0;
    }
}

}