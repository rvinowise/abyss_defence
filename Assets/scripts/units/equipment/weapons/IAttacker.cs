﻿using rvinowise.unity.actions;
using UnityEngine;
using Action = System.Action;


namespace rvinowise.unity {
public interface IAttacker 
{
    //Transform transform { get; }

    bool can_reach(Transform target);
    float get_reaching_distance();
    void attack(Transform target, System.Action on_completed = null);
}


public interface IActor_attacker : IAttacker, IActor { }


public abstract class Attacker_child_of_group: 
    Child_of_group,
    IAttacker {

    public abstract bool can_reach(Transform target);
    public abstract float get_reaching_distance();

    public abstract void attack(Transform target, Action on_completed = null);
}

}