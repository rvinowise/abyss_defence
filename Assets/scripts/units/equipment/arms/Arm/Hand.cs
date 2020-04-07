using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.units.parts.tools;


namespace rvinowise.units.parts.limbs.arms {

public class Hand:Segment,
IHave_velocity
{

    public Side side = Side.LEFT;
    
    public Hand_gesture gesture {
        get { return _gesture;}
        set {
            animator.SetInteger("Gesture", value.Value);
            tip = value.valuable_point;
            if (side == Side.RIGHT) {
                _tip.y = -_tip.y;
            }
        } 
    }
    private Hand_gesture _gesture;
    
    public Vector2 velocity { get; set; }

    public Hand(string name) : base(name) { }
    public Hand(string name, GameObject prefab) : base(name, prefab) { }

    public override void mirror_from(limbs.Segment src) {
        base.mirror_from(src);
        if (src is arms.Hand src_hand) {
            side = src_hand.side.mirror();
        }
    }
}
}