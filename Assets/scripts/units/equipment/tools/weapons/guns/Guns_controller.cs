using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;


namespace rvinowise.units.parts.weapons.guns {


public class Guns_controller:
    Children_group
    ,IWeaponry 
{
    private IList<Ak47> rifles;


    public override IEnumerable<Child> children { get; }

    public Guns_controller(IChildren_groups_host dst_host) : base(dst_host) {
        
    }
    
    public override IChildren_group copy_empty_into(IChildren_groups_host dst_host) {
        return new Guns_controller(dst_host);
    }


    public Command_batch command_batch { get; }

    public void update() {
    }


    public override void add_child(Child child) {
        /*if (child is Ak47 rifle) {
            rifle.parent = transform;
            rifles.Add(rifle);
        }*/
        
    }


    /* IWeaponry interface */
    public void fire() {
        
    }

    public void shoot(Transform target) {
        throw new NotImplementedException();
    }

    public void shoot(GameObject target) {
    }
}
}