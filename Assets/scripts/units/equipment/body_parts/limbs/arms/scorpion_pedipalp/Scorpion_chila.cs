using System;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {


public enum Pincers_state {
    idle,
    opened,
    closed
}
public class Scorpion_chila:
    Segment 
{

    public Animator animator;
    public System.Action on_closed_listeners;
    public System.Action on_opened_listeners;

    public Pincers_state pincers_state = Pincers_state.idle;
    
    [called_in_animation]
    public void on_closed() {
        pincers_state = Pincers_state.closed;
        on_closed_listeners?.Invoke();
        on_closed_listeners = null;
    }
    
    [called_in_animation]
    public void on_opened() {
        pincers_state = Pincers_state.opened;
        on_opened_listeners?.Invoke();
        on_opened_listeners = null;
    }
}

}