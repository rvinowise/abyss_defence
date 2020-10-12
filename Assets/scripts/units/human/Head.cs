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
    
    
    protected override void update_rotation() {
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, 
                target_quaternion,
                rotation_speed * Time.deltaTime);

        base.update_rotation();
        collide_with_rotation_borders();
    }

    public void pay_attention_to(Vector2 point) {
        target_quaternion = (point - position).to_quaternion();
    }
}


}