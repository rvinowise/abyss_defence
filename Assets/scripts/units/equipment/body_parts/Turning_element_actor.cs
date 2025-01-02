using System;
using rvinowise.unity.actions;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using static rvinowise.unity.geometry2d.Directions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Turning_element_actor: 
    Turning_element
    ,IActing_role 
{

    #region IActor

    public Actor actor { get; set; }

    public void on_lacking_action() {
        
    }

    #endregion
}
}