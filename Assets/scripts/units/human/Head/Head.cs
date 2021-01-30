using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts.sensors;


namespace rvinowise.unity.units.parts.head {

public class Head: Turning_element, ISensory_organ {

    public Transform attention_target;

    protected void Start() {
        attention_target = rvinowise.unity.ui.input.Input.instance.cursor.transform;
    }
    public static Head create() {
        GameObject game_object = new GameObject();
        game_object.AddComponent<SpriteRenderer>();
        var new_component = game_object.add_component<Head>();
        return new_component;
    }
    
    
    protected override void update_rotation() {
        base.update_rotation();
        preserve_possible_rotations();
    }

    public void pay_attention_to(Vector3 point) {
        target_rotation = (point - position).to_quaternion();
    }

    protected void Update() {
        pay_attention_to(attention_target.position);
    } 

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        if (Application.isPlaying) {
            rvinowise.unity.debug.Debug.DrawLine_simple(
                transform.position, 
                transform.position + target_rotation * Vector2.right * 0.3f,
                Color.blue,
                3
            );
        }
    }
}


}