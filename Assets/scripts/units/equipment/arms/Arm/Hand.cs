using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.tools;
using rvinowise.contracts;

namespace rvinowise.unity.units.parts.limbs.arms {

[RequireComponent(typeof(Animator))]
public class Hand:Segment,
IHave_velocity 
{

    
    public Side side = Side.LEFT;
    public Transform valuable_point;
    public Holding_place held_part;
    
    public Hand_gesture gesture {
        get { return _gesture;}
        set {
            _gesture = value;
            animator.SetInteger("gesture", value.Value);
            localTip = value.valuable_point;
        } 
    }
    
    public Vector2 velocity { get; set; }
    public float linear_drag { get; set; } = 0.02f;

    public float held_object_local_z {
        get {
            return 
                (bottom_part.transform.localPosition.z +
                top_part.transform.localPosition.z)
                /2f;
        }
    }

    public Tool held_tool {
        get { return held_part?.tool; }
    }

    private Animator animator;
    private Hand_gesture _gesture;
    private Transform bottom_part;
    private Transform top_part;

    protected override void Awake() {
        base.Awake();
        init_parts();
    }

    protected override void Start() {
        base.Start();
        if (held_part != null) {
            attach_tool_to_hand_for_holding(held_part);
        }
    }

    private void init_parts() {
        animator = GetComponent<Animator>();
        bottom_part = transform.Find("bottom")?.transform;
        top_part = transform.Find("top")?.transform;
        valuable_point = transform.Find("valuable_point")?.transform;
    }
    
    public override void mirror_from(limbs.Segment src) {
        base.mirror_from(src);
        if (src is arms.Hand src_hand) {
            side = src_hand.side.mirror();
        }
    }


    public void attach_tool_to_hand_for_holding(Holding_place new_held_part) {
        if (held_part != null) {
            deattach_tool_from_hand(held_part);
        }
        this.held_part = new_held_part;
        if (new_held_part != null) {
            attach_tool_to_hand(new_held_part);
        }
        Contract.Ensures(this.held_tool != null);

        void deattach_tool_from_hand(Holding_place held_part) {
            held_part.hold_by(null);
            gesture = Hand_gesture.Relaxed;
        }
        void attach_tool_to_hand(Holding_place held_part) {
            gesture = held_part.grip_gesture;

            Tool tool = held_part.tool;
            tool.transform.set_z(held_object_local_z);
            held_part.hold_by(this);
        }
    }

    

}
}