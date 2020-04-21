using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.limbs.arms;


namespace rvinowise.units.parts.tools {

public class Holding_place {
    public Vector2 place_on_tool = Vector2.zero;
    public Hand_gesture grip_gesture = Hand_gesture.Relaxed;
    public Degree grip_direction = new Degree(0);
    public Quaternion grip_direction_quaternion {
        get { return grip_direction.to_quaternion(); }
    }
    public Tool tool;
    public bool is_main;
    public Arm holding_arm {
        get { return _holding_arm; }
        set {
            _holding_arm = value;
            if (is_main) {
                tool.gameObject.transform.parent = _holding_arm.hand.transform;
            }
        }
    }
    public Arm _holding_arm;

    public IHave_velocity holder {
        get { return holding_arm.hand; }
    }

    public Holding_place(Tool in_tool) {
        tool = in_tool;
    }

    public static Holding_place main(Tool in_tool) {
        Holding_place holding_place = new Holding_place(in_tool);
        holding_place.is_main = true;
        return holding_place;
    }
    public static Holding_place secondary(Tool in_tool) {
        Holding_place holding_place = new Holding_place(in_tool);
        holding_place.is_main = false;
        return holding_place;
    }
    
    public Vector2 position {
        get{
            return tool.transform.TransformPoint(place_on_tool);
        }
    }
    public Quaternion rotation {
        get { return tool.transform.rotation * grip_direction_quaternion; }
    }
    
}
}