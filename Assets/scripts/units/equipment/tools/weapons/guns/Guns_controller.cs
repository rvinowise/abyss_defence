using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;


namespace rvinowise.unity.units.parts.weapons.guns {


public class Guns_controller:
    Children_group
    ,IWeaponry 
{
    private IList<Ak47> rifles;


    public override IEnumerable<ICompound_object> children { get; }
    

    public Command_batch command_batch { get; }

    public void update() {
    }


    public override void add_child(ICompound_object compound_object) {
        /*if (child is Ak47 rifle) {
            rifle.parent = transform;
            rifles.Add(rifle);
        }*/
        
    }

    public override void hide_children_from_copying() {
        //children_stashed_from_copying = children
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