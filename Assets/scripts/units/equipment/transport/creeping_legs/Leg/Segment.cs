using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.creeping_legs {

public class Segment: limbs.Segment {

    /* the most comfortable direction_quaternion if the body isn't moving.
     determined during construction*/
    public Quaternion desired_relative_direction_standing { get; set; } //relative to local_position
    
    
    /* limbs.Segment interface */
    public static Segment create(string in_name) {
        GameObject game_object = new GameObject(in_name);
        game_object.AddComponent<SpriteRenderer>();
        var new_component = game_object.add_component<Segment>();
        return new_component;
    }
}
}