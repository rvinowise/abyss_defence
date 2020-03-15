using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.sensors;


namespace rvinowise.units.parts.head {

public class Head: Turning_element, ISensory_organ {


    /* Head itself */

    public Head() {
        
    }
    
    
    public override void update() {
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, 
                desired_direction,
                rotation_speed * Time.deltaTime);

        base.update();
    }

    public void pay_attention_to(Vector2 point) {
        desired_direction = (point - position).to_quaternion();
    }
}


}