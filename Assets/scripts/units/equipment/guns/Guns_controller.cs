using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.equipment.guns {


public class Guns_controller:
    Equipment_controller
    ,IWeaponry 
{
    public override IEnumerable<Tool> tools { get; }
    public override IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
        throw new NotImplementedException();
    }

    public override void init() {
        throw new NotImplementedException();
    }

    public override void update() {
        throw new NotImplementedException();
    }

    public override void add_tool(Tool tool) {
        throw new NotImplementedException();
    }
}
}