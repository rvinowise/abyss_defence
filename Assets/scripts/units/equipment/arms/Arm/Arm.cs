using rvinowise.unity.geometry2d;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.tools;
using System;
using rvinowise.unity.units.parts.limbs.arms.humanoid;
using rvinowise.unity.extensions;
using rvinowise.unity.ui.input;
using rvinowise.unity.units.parts.weapons.guns;

namespace rvinowise.unity.units.parts.limbs.arms  {

public partial class Arm: 
    Limb3, IChild_of_group, IReceive_recoil

{

    public Arm_pair pair;
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
    public Holding_place held_part {
        get {return hand.held_part;}
    }

    public bool is_holding_tool() {
        return held_tool != null;
    }

    public float length {
        get { return upper_arm.length + forearm.length + hand.length; }
    }
    
    

    #region IReceive_recoil
    public void push_with_recoil(float in_impulse) {
        Side_type side = (Side_type) folding_side;
        shoulder.current_rotation_inertia += (float)side * in_impulse;
        upper_arm.current_rotation_inertia += (float)side * in_impulse;
        forearm.current_rotation_inertia += (float)Side.flipped(side) * in_impulse * 1.2f;
        hand.current_rotation_inertia += (float)side * in_impulse * 1.2f;
    }
    #endregion
    

    public Baggage baggage; 
    public Transform attention_target;

    

    protected void Awake() 
    {
        attention_target = ui.input.Player_input.instance.cursor.transform;
    }
    
   
    public void start_idle_action() {
        Idle_vigilant_only_arm.create(
            this,
            attention_target,
            pair.transporter
        ).start_as_root(action_runner);
    }
    public void aim_at(Transform in_target) {
        Aim_at_target.create(
            this,
            in_target,
            pair.user
        ).start_as_root(action_runner);
    }
    public void Update() {
        if (!controlled_by_animation()) {
            shoulder.preserve_possible_rotations();
            base.preserve_possible_rotations();
        }
    }

    private bool controlled_by_animation() {
        return pair.user.animator.isActiveAndEnabled;
    }
    
    public override void rotate_to_orientation(Orientation needed_orientation) {
        set_desired_directions_by_position(needed_orientation.position);
        hand.target_rotation = needed_orientation.rotation;
        rotate_to_desired_directions();
    }
    public override void rotate_to_desired_directions() {
        shoulder.rotate_to_desired_direction();
        base.rotate_to_desired_directions();
    }

    public void take_tool_from_baggage(Tool tool) {

        Action_sequential_parent.create(
            actions.Take_tool_from_bag.create(
                this, baggage, tool
            ),
            actions.Idle_vigilant_only_arm.create(
                this,
                attention_target,
                pair.transporter
            )
        ).start_as_root(action_runner);

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

    public float get_aiming_distance_to(Transform in_target) {
        Quaternion needed_direction = transform.position.quaternion_to(in_target.position);
        return Math.Abs(hand.transform.rotation.degrees_to(needed_direction));
    }
    
    public bool aiming_automatically() {
        return 
        (held_tool is Gun gun)&&
        (gun.aiming_automatically);
    }

    public void draw_desired_directions(float time=0.1f) {
        rvinowise.unity.debug.Debug.DrawLine_simple(
            shoulder.position, 
            shoulder.desired_tip,
            Color.white,
            2
        );
        rvinowise.unity.debug.Debug.DrawLine_simple(
            shoulder.desired_tip, 
            upper_arm.desired_tip,
            Color.white,
            2
        );
        rvinowise.unity.debug.Debug.DrawLine_simple(
            upper_arm.desired_tip, 
            forearm.desired_tip,
            Color.white,
            2
        );
        rvinowise.unity.debug.Debug.DrawLine_simple(
            forearm.desired_tip,
            hand.desired_tip,
            Color.white,
            2
        );
    }

    public void drop_tool() {
        hand.detach_tool();
    }

    public void grab_tool(Holding_place in_place) {
        hand.attach_holding_part(in_place);
    }

    void OnDrawGizmos() {
        if (Application.isPlaying) {
            draw_desired_directions();
        }
    }
    
    public override void on_lacking_action() {
        Idle_vigilant_only_arm.create(
            this,
            Player_input.instance.cursor.transform,
            pair.transporter
        ).start_as_root(action_runner);
    }

}
}