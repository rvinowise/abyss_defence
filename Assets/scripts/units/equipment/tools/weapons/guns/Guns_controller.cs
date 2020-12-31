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


    public override IEnumerable<IChild_of_group> children { get; }
    

    public Command_batch command_batch { get; }

    public void update() {
    }


    public override void add_child(IChild_of_group compound_object) {
        /*if (child is Ak47 rifle) {
            rifle.parent = transform;
            rifles.Add(rifle);
        }*/
        
    }

   protected override void init_child_list() {

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