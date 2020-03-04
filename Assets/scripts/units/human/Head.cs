using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.equipment.limbs;


namespace rvinowise.units.human {

public class Head: Body_part {


    /* Head itself */

    public Head() {
        
    }
    
    public Transform transform {
        get { return game_object.transform; }
    }
    
    public void look_in_direction(Vector2 direction) {
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, 
                desired_direction,
                rotation_speed * Time.deltaTime);
    }
}
}