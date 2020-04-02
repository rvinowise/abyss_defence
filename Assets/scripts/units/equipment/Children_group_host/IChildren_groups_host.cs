using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.transport;


namespace rvinowise.units.parts {

public interface IChildren_groups_host {

    ITransporter transporter { get; }
    
    GameObject game_object { get; }
    
    IList<Children_group> children_groups { get; }


}
}