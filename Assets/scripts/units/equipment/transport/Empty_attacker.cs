using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {
    
public class Empty_attacker:
    IAttacker 
{

    public bool can_reach(Transform target) {
        return false;
    }

    public void attack(Transform target, System.Action on_completed = null) {
        on_completed?.Invoke();
    }

}

}