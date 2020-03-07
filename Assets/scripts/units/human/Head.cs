using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs;


namespace rvinowise.units.parts.head {

public class Head: Turning_element {


    /* Head itself */

    //public Vector2 desired_direction_vector;
    public Quaternion desired_direction;
    
    public Head() {
        
    }
    
    public Transform transform {
        get { return game_object.transform; }
    }
    
    public void update() {
        base.update();
        
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, 
                desired_direction,
                rotation_speed * Time.deltaTime);
    }
}
}