using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.transport;


namespace rvinowise.units.parts {

public interface IChildren_groups_host {

    ITransporter transporter { get; set; }
    
    IWeaponry weaponry { get; set; }
    
    GameObject game_object { get; }
    
    IList<Children_group> children_groups { get; }

    T add_equipment_controller<T>() where T :
        Children_group, new();

    void add_equipment_controllers_after(IChildren_groups_host src_user);

}
}