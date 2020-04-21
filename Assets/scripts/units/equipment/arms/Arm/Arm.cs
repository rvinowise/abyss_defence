using System;
using geometry2d;
using UnityEngine;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.actions;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.limbs.arms.actions;
using rvinowise.units.parts.tools;
using units.equipment.arms.Arm.actions;
using UnityEngine.UIElements;

namespace rvinowise.units.parts.limbs.arms  {

public partial class Arm: Limb3/*, IDo_actions*/ {

    /* constant characteristics */

    private Arm_controller controller;
    
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
    /*public Hand hand {
        get { return segment3 as arms.Hand;}
        set { segment3 = value; }
    }*/
    public Hand hand {
        get { return _hand;}
        set { _hand = value; }
    }
    private Hand _hand;

    public float length {
        get { return upper_arm.length + forearm.length + hand.length; }
    }
    
    /* IDo_actions interface */

    public Action_parent action = new Action_parent();
    
    /* Arm itself */

    /* parameters assigned by creators */
    public Baggage baggage; // where to take children from
    public Transform idle_target; // pay attention to it, when start_idle_action

    public Tool held_tool {
        get { return held_part?.tool; }
    }
    public Holding_place held_part;

    public Arm(Arm_controller in_controller, Transform in_idle_target) {
        controller = in_controller;
        
        upper_arm = new Segment("upper_arm");
        upper_arm.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        upper_arm.parent = in_controller.transform;
        
        forearm = new Segment("forearm");
        forearm.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        forearm.game_object.GetComponent<SpriteRenderer>().sortingOrder = -1;
        forearm.parent = upper_arm.transform;

        hand = new Hand(
            "hand", 
            Resources.Load<GameObject>("objects/units/human/hand")
        );
        hand.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        hand.game_object.GetComponent<SpriteRenderer>().sortingOrder = -10;
        hand.parent = forearm.transform;

        idle_target = in_idle_target;
        start_idle_action();

        

        debug = new Arm.Debug(this);
    }

    public void start_idle_action() {
        action.current_child_action = Idle_vigilant_only_arm.create(
            action,
            this,
            idle_target,
            controller.transporter
        );
    }
    public void update() {
        if (folding_direction == Side.RIGHT) {
            bool test = true;
        }
        action?.update();

        base.preserve_possible_rotations();


        TEST_draw_debug_lines();
    }
    

    public void take_tool_from_baggage(Tool tool) {
        if (folding_direction == Side.RIGHT) {
            bool test = true;
        }
        Contract.Requires(held_tool == null, "must be free in order to grab a tool");

        action.current_child_action = actions.Take_tool_from_bag.create(action, this, baggage, tool);
        
        action.new_next_child = actions.Idle_vigilant_only_arm.create(
            action,
            this,
            idle_target, 
            controller.transporter
        );

    }

    public void support_held_tool(Tool tool) {
        Contract.Requires(held_tool == null, "must be free in order to grab a tool");
        
        action.current_child_action = actions.Arm_reach_holding_part_of_tool.create(
            action, 
            tool.second_holding
        );
        action.new_next_child = actions.Attach_to_holding_part_of_tool.create(
            action,
            tool.second_holding
        );

        move_main_arm_closer(tool);
    }

    private void move_main_arm_closer(Tool tool) {
        Arm main_arm = tool.main_holding.holding_arm;
        main_arm.action.current_child_action = actions.idle_vigilant.main_arm.Gun_without_stock.create(
            main_arm.action, 
            idle_target,
            controller.transporter
        );
    }

    public void stash_tool_to_bag() {
        Contract.Requires(held_tool != null, "must hold a tool in order to stash it");
        action.current_child_action = actions.Put_hand_before_bag.create(action, this, baggage);
        action.new_next_child = actions.Move_hand_into_bag.create(action, this, baggage);
    }


    public void attach_tool_to_hand_for_holding(Holding_place new_held_part) {
        if (held_part != null) {
            deattach_tool_from_hand(held_part);
        }
        this.held_part = new_held_part;
        if (new_held_part != null) {
            attach_tool_to_hand(new_held_part);
        }
        
        void deattach_tool_from_hand(Holding_place held_part) {
            
            held_part.holding_arm = null;
            
            this.hand.gesture = Hand_gesture.Relaxed;
        }
        void attach_tool_to_hand(Holding_place held_part) {
            held_part.holding_arm = this;
            Transform tool_transform = held_part.tool.gameObject.transform;
            tool_transform.localPosition =
                hand.tip -
                held_part.place_on_tool;
            tool_transform.localRotation = 
                held_part.grip_direction.adjust_to_side(this.folding_direction).to_quaternion();
            
            this.hand.gesture = held_part.grip_gesture;
        }
    }

    

}
}