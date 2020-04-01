using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise;
using rvinowise.units;
using rvinowise.units.parts;
using rvinowise.rvi.contracts;
using rvinowise.units.debug;
using rvinowise.units.parts.transport;

namespace rvinowise.units.parts
{
public partial class Children_groups_host
{


    public ITransporter transporter { get; set; }
    public IWeaponry weaponry { get; set; }

    

    /*private void Update() {
        foreach (var equipment_controller in children_groups) {
            equipment_controller.update();
        }
    }


    public void remove_empty_controllers() {
        List<Children_group> new_tool_controllers = new List<Children_group>(children_groups.Count);
        for (int i_tool_controller = 0; i_tool_controller < children_groups.Count; i_tool_controller++) {
            Children_group children_controller = children_groups[i_tool_controller];
            if (children_controller.children.Any()) {
                new_tool_controllers.Add(children_controller);
            }
            /*else {
                destroy_tool_controller(children_controller);
            }#1#
        }
        children_groups = new_tool_controllers;

    }*/



    
}

}

