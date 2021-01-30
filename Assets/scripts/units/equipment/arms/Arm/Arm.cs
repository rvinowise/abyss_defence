using rvinowise.unity.geometry2d;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.debug;
using Action = rvinowise.unity.units.parts.actions.Action;
using System;

namespace rvinowise.unity.units.parts.limbs.arms  {

public partial class Arm: 
    Limb3, IPerform_actions, IChild_of_group

{

    [HideInInspector]
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
    [SerializeField]
    private Hand _hand;

    public Tool held_tool {
        get {return hand.held_tool;}
    }

    public float length {
        get { return upper_arm.length + forearm.length + hand.length; }
    }
    
    #region IPerform_actions

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
    #endregion

    
    

    public Baggage baggage; 
    public Transform attention_target;

    

    protected override void Awake() 
    {
        base.Awake();
        attention_target = ui.input.Input.instance.cursor.transform;
    }

    

    private void mirror_from() {

    }
    
   
    public void start_idle_action() {
        set_root_action(
            Idle_vigilant_only_arm.create(
                this,
                attention_target,
                controller.transporter
            )
        );
    }
    public void FixedUpdate() {
        current_action?.update();

        base.preserve_possible_rotations();
    }
    public override void rotate_to_orientation(Orientation needed_orientation) {
        set_desired_directions_by_position(needed_orientation.position);
        hand.target_rotation = needed_orientation.rotation;
        rotate_to_desired_directions();
    }
    public override void rotate_to_desired_directions() {
        //shoulder.rotate_to_desired_direction();
        base.rotate_to_desired_directions();
    }

    public void take_tool_from_baggage(Tool tool) {
        Contract.Requires(hand.held_tool == null, "must be free in order to grab a tool");

        set_root_action(
            Action_sequential_parent.create(
                actions.Take_tool_from_bag.create(
                    this, baggage, tool
                ),
                actions.Idle_vigilant_only_arm.create(
                    this,
                    attention_target,
                    controller.transporter
                )
            )
        );

    }

    public void support_held_tool(Tool tool) {
        Contract.Requires(hand.held_tool == null, "must be free in order to grab a tool");

        current_action = Action_sequential_parent.create(
            actions.Arm_reach_holding_part_of_tool.create(
                tool.second_holding
            ),
            actions.Attach_to_holding_part_of_tool.create(
                tool.second_holding
            )
        );
        
    }

    
    public float shoulder_mirrored_target_direction {
        set {
            set_relative_mirrored_target_direction(shoulder, value);
        }
    }
    public float segment2_mirrored_target_direction {
        set {
            set_relative_mirrored_target_direction(segment2, value);
        }
    }

    
    public void draw_desired_directions(float time=0.1f) {
        rvinowise.unity.debug.Debug.DrawLine_simple(
            shoulder.position, 
            shoulder.desired_tip,
            Color.red,
            3
        );
        rvinowise.unity.debug.Debug.DrawLine_simple(
            shoulder.desired_tip, 
            upper_arm.desired_tip,
            Color.blue,
            3
        );
        rvinowise.unity.debug.Debug.DrawLine_simple(
            upper_arm.desired_tip, 
            forearm.desired_tip,
            Color.white,
            3
        );
        rvinowise.unity.debug.Debug.DrawLine_simple(
            forearm.desired_tip,
            hand.desired_tip,
            Color.yellow,
            3
        );
    }
    

    void OnDrawGizmos() {
        if (Application.isPlaying) {
            draw_desired_directions();
        }
    }

}
}