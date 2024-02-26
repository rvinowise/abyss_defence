using System;
using System.Collections.Generic;
using UnityEngine;


namespace rvinowise.unity {


public class Guns_controller:
    Children_group
    ,IAttacker 
{
    private IList<Ak47> rifles;


    public override IEnumerable<IChild_of_group> children { get; }
    


    public void update() {
    }


    public override void add_child(IChild_of_group compound_object) {
        /*if (child is Ak47 rifle) {
            rifle.parent = transform;
            rifles.Add(rifle);
        }*/
        
    }


    /* IWeaponry interface */

    public bool can_reach(Transform target) {
        throw new NotImplementedException();
    }

    public void attack(Transform target, Action on_completed) {
        throw new NotImplementedException();
    }
}
}