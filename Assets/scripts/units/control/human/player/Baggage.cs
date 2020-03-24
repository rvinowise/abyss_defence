using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.tools;
using rvinowise.units.parts.weapons.guns;

namespace rvinowise.units.parts.humanoid {

/* keeps track of unit's posessions */
public class Baggage: units.parts.Baggage {

    
    public int left_item;
    //public int left_switch_direction = 1;
    public int right_item;
    //public int right_switch_direction = 1;

    public Tool switch_left_hand_item(int index_change) {
        left_item += index_change;
        left_item = ensure_borders(left_item);
        return items[left_item];
    }

    

    /*public Gun get_next_right_hand_item() {
        right_item += right_switch_direction;
    }*/

}
}