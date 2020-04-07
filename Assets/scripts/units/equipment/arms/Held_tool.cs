using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.weapons;
using rvinowise.units.parts.weapons.guns;


namespace rvinowise.units.parts.limbs.arms {

public class Held_tool: IWeapon {
    
    public Gun gun;


    public Orientation desired_orientaiton { get; set; }

    private Transform baggage;
    //private IList<Arm> arms = new List<Arm>();
    public Arm trigger_arm;
    public Arm stock_arm;

    public float time_to_shooting(Transform target) {
        float time_for_rotation = time_to_aim_at(target);
        float need_to_wait = time_to_readiness();
        return Math.Max(need_to_wait, time_for_rotation);
    }

    public void pull_trigger() {
        gun.pull_trigger();
    }

    public void return_tool_into_bagage() {
        Contract.Requires(gun != null, "must hold a tool in order to return it");
        /*if (tool_is_in_baggage()) {
            return
        } else if (tool_touches_baggage()) {
            desired_orientaiton.position = baggage.position;
            //desired_direction = Quaternion.Inverse(baggage.rotation);
        }
        else {
            set_desired_directions(get_orientation_touching_baggage());
        }*/
    }

    public void take_tool_from_baggage() {
        Contract.Requires(gun == null, "must be free in order to grab a tool");
        set_desired_directions(get_orientation_touching_baggage());
    }

    private bool tool_touches_baggage() {
        return gun.transform.orientation() == get_orientation_touching_baggage();
    }

    private void set_desired_directions(Orientation tool_orientation) {
        trigger_arm.set_desired_directions_by_position(tool_orientation.position);
        trigger_arm.hand.desired_direction = tool_orientation.rotation;
    }

    private Orientation get_orientation_touching_baggage() {
        /*return new Orientation(
            baggage.position + baggage.rotation * gun.tip,
            Quaternion.Inverse(baggage.rotation),
            null
        );*/
        return new Orientation(
            gun.tip,
            Quaternion.Inverse(Quaternion.identity),
            baggage
        );
    }

    public void update() {
        trigger_arm.update();
        stock_arm?.update();
    }
    

    public void shoot(Transform target) {
        
    }
    

    public float time_to_readiness() {
        return gun.time_to_readiness();
    }

    public float time_to_aim_at(Transform target) {
        return 0;
    }
    

}
}