using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {
    
/* provides information about possible speed and rotation for a moving Unit */
public class Empty_transporter: 
    ITransporter
{

    public float get_possible_rotation(){ return 0;}

    public float get_possible_impulse() { return 0;}

    private Turning_element moved_body;
    public void set_moved_body(Turning_element in_body) {
        moved_body = in_body;
    }

    public Turning_element get_moved_body() {
        return moved_body;
    }

    public void move_towards_destination(Vector2 destination) {
        
    }

    public void face_rotation(Quaternion direction) {
        
    }


    public Actor actor { get; set; }

    public void on_lacking_action() {
    }

    public GameObject gameObject { get; }

}

}