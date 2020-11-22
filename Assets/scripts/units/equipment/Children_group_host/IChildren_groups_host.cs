using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.transport;


namespace rvinowise.unity.units.parts {

public interface IChildren_groups_host {

    
    GameObject game_object { get; }
    
    IList<Children_group> children_groups { get; }

}
}