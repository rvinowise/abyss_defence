using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;


namespace rvinowise.units.equipment.guns {


public class Guns_controller:
    Equipment_controller
    ,IWeaponry 
{
    private IList<Rifle> rifles;


    public override IEnumerable<Tool> tools { get; }

    public Guns_controller(User_of_equipment dst_host) : base(dst_host) {
        
    }
    
    public override IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
        return new Guns_controller(dst_host);
    }

    

    public override void update() {
    }

    public override void add_tool(Tool tool) {
        if (tool is Rifle rifle) {
            rifle.host = transform;
            rifles.Add(rifle);
        }
        
    }
    
}
}