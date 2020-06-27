using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;


namespace rvinowise.units.parts.actions {

public abstract class Action_parent: 
    Action {

    public abstract void on_child_reached_goal(Action in_sender_child);

}
}