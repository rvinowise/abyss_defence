﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;


namespace rvinowise.units.parts.weapons.guns {


public class Guns_controller:
    Equipment_controller
    ,IWeaponry 
{
    private IList<Ak47> rifles;


    public override IEnumerable<Child> tools { get; }

    public Guns_controller(User_of_equipment dst_host) : base(dst_host) {
        
    }
    
    public override IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
        return new Guns_controller(dst_host);
    }

    

    public override void update() {
    }

    public override void add_tool(Child child) {
        /*if (child is Ak47 rifle) {
            rifle.parent = transform;
            rifles.Add(rifle);
        }*/
        
    }


    protected override void execute_commands() {
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