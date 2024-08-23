using UnityEngine;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;


namespace rvinowise.unity {

[RequireComponent(typeof(Animator))]
public class Hand:Arm_segment
{

    
    public Side_type side = Side_type.LEFT;
    public Transform valuable_point;
    public Holding_place held_part;
    public Arm arm;
    
    public Animator animator;
    public Transform bottom_part;
    public Transform top_part;
    public Hand_gesture gesture;
    
    public void set_gesture(Hand_gesture in_gesture) {
        gesture = in_gesture;
        animator.SetInteger("gesture", (int)in_gesture);
    }
    
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



    protected void Start() {
        //base.Start();
        parent_segment = transform.parent?.GetComponent<Arm_segment>();

        if (held_part != null) {
            attach_holding_part(held_part);
        }
    }


    public void switch_held_tools(Holding_place new_held_part) {
        if (new_held_part.holding_hand != null) {
            new_held_part.drop_from_hand();
        }
        if (held_part != null) {
            detach_tool();
        }
        
        if (new_held_part != null) {
            attach_holding_part(new_held_part);
        }
        Contract.Ensures(this.held_tool != null);
        
    }
    
    public Tool detach_tool() {
        Tool dropped_tool = held_tool;
        held_part.drop_from_hand();
        held_part = null;
        gesture = Hand_gesture.Relaxed;
        return dropped_tool;
    }

    public void attach_holding_part(Holding_place held_part) {
        set_gesture(held_part.grip_gesture);
        this.held_part = held_part;
        held_part.set_parenting_for_holding(this);
        held_part.tool.hold_by(this);
    }

    

}
}