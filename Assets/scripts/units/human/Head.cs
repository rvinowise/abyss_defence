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

    public static Head create() {
        GameObject game_object = new GameObject();
        game_object.AddComponent<SpriteRenderer>();
        var new_component = game_object.add_component<Head>();
        return new_component;
    }
    
    
    public override void update() {
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, 
                desired_direction,
                rotation_speed * Time.deltaTime);

        base.update();
        preserve_possible_rotation();
    }

    public void pay_attention_to(Vector2 point) {
        desired_direction = (point - position).to_quaternion();
    }
}


}