using System;
using geometry2d;
using UnityEngine;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.tools;
using UnityEngine.UIElements;
using Action = rvinowise.units.parts.actions.Action;

namespace rvinowise.units.parts.limbs.arms  {

public partial class Arm: 
    Limb3, IPerform_actions 

{

    /* constant characteristics */

    public Arm_controller controller;
    
    public Segment shoulder;

    public Segment upper_arm {
        get { return segment1 as arms.Segment;}
        set { segment1 = value; }
    }
    public Segment forearm {
        get { return segment2 as arms.Segment;}
        set { segment2 = value; }
    }

    public override limbs.Segment segment3 {
        get{
            return _hand;
        }
    }

    public Hand hand {
        get { return _hand;}
        set { _hand = value; }
    }
    private Hand _hand;

    public float length {
        get { return upper_arm.length + forearm.length + hand.length; }
    }
    
    /* IPerform_actions interface */

    public Action current_action {
        get { return _current_action;}
        set {
            _current_action = value;
            
        }
    }
    private Action _current_action;

    public void set_root_action(Action in_action) {
        current_action = in_action;
        current_action.start_as_root();
    }
    
    /* Arm itself */

    /* parameters assigned by creators */
    public Baggage baggage; // where to take children from
    public Transform idle_target; // pay attention to it, when start_idle_action

    public Tool held_tool {
        get { return held_part?.tool; }
    }
    public Holding_place held_part;

    public Arm(
        Arm_controller in_controller, 
        Segment in_shoulder,
        Segment in_upper_arm,
        Segment in_forearm,
        Hand in_hand
    ) 
    {
        controller = in_controller;
        
        shoulder = in_shoulder;
        upper_arm = in_upper_arm;
        forearm = in_forearm;
        hand = in_hand;


        debug = new Arm.Debug(this);
    }
    
   
    public void start_idle_action() {
        set_root_action(
            Idle_vigilant_only_arm.create(
                this,
                idle_target,
                controller.transporter
            )
        );
    }
    public void update() {
        if (current_action != null) {
            bool test = true;
        }
        
        current_action?.update();

        base.preserve_possible_rotations();


        TEST_draw_debug_lines();
    }
    public virtual void rotate_to_orientation(Orientation needed_orientation) {
        set_desired_directions_by_position(needed_orientation.position);
        hand.target_quaternion = needed_orientation.rotation;
        rotate_to_desired_directions();
    }
    public override void rotate_to_desired_directions() {
        shoulder.rotate_to_desired_direction();
        base.rotate_to_desired_directions();
    }

    public void take_tool_from_baggage(Tool tool) {
        if (folding_direction == Side.RIGHT) {
            bool test = true;
        }
        Contract.Requires(held_tool == null, "must be free in order to grab a tool");

        set_root_action(
            Action_sequential_parent.create(
                actions.Take_tool_from_bag.create(
                    this, baggage, tool
                ),
                actions.Idle_vigilant_only_arm.create(
                    this,
                    idle_target,
                    controller.transporter
                )
            )
        );

    }

    public void support_held_tool(Tool tool) {
        Contract.Requires(held_tool == null, "must be free in order to grab a tool");

        current_action = Action_sequential_parent.create(
            actions.Arm_reach_holding_part_of_tool.create(
                tool.second_holding
            ),
            actions.Attach_to_holding_part_of_tool.create(
                tool.second_holding
            )
        );
        
        

        move_main_arm_closer(tool);
    }

    private void move_main_arm_closer(Tool tool) {
        Arm main_arm = tool.main_holding.holding_arm;
        main_arm.current_action = actions.idle_vigilant.main_arm.Gun_without_stock.create(
            idle_target,
            controller.transporter
        );
    }

    public void stash_tool_to_bag() {
        /*Contract.Requires(held_tool != null, "must hold a tool in order to stash it");
        current_action.current_child_action_setter = actions.Put_hand_before_bag.create(current_action, this, baggage);
        current_action.new_next_child = actions.Move_hand_into_bag.create(current_action, this, baggage);
    */
    }


    public void attach_tool_to_hand_for_holding(Holding_place new_held_part) {
        if (held_part != null) {
            deattach_tool_from_hand(held_part);
        }
        this.held_part = new_held_part;
        if (new_held_part != null) {
            attach_tool_to_hand(new_held_part);
        }
        Contract.Assert(this.held_tool != null);
    }

    void deattach_tool_from_hand(Holding_place held_part) {
        held_part.hold_by(null);
        this.hand.gesture = Hand_gesture.Relaxed;
    }
    void attach_tool_to_hand(Holding_place held_part) {
        hand.gesture = held_part.grip_gesture;

        Tool tool = held_part.tool;
        set_Z_for_holding(tool);
        held_part.hold_by(this);
    }

    private void set_Z_for_holding(Tool tool) {
        tool.gameObject.set_sorting_layer("arms", hand.bottom_sorting_order+1 );
    }


    
}
}