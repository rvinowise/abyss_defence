using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.actions;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using UnityEngine;

namespace rvinowise.unity {

public class Gate:
    MonoBehaviour 

{
    public Animator animator;

    public System.Action on_closed_listeners;
    public System.Action on_opened_listeners;
    
    [called_in_animation]
    public void on_closed() {
        on_closed_listeners?.Invoke();
        on_closed_listeners = null;
    }

    [called_in_animation]
    public void on_opened() {
        on_opened_listeners?.Invoke();
        on_opened_listeners = null;
    }

    public void start_closing() {
        animator.speed = 0.25f;
        animator.SetTrigger("close");
    }
    
    public void start_opening() {
        animator.speed = 1;
        animator.SetTrigger("open");
    }
}

}