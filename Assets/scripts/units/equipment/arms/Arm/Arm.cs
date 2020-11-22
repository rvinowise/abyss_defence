using rvinowise.unity.geometry2d;
using UnityEngine;

using rvinowise.rvi.contracts;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.tools;
using Action = rvinowise.unity.units.parts.actions.Action;

namespace rvinowise.unity.units.parts.limbs.arms  {

public partial class Arm: 
    Limb3, IPerform_actions 

{

    /* constant characteristics */
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
    public Baggage baggage; 
    public Transform attention_target;

    

    protected void Awake() 
    {
        debug = new Arm.Debug(this);
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
    public void update() {
        current_action?.update();

        base.preserve_possible_rotations();

        TEST_draw_debug_lines();
    }
    public override void rotate_to_orientation(Orientation needed_orientation) {
        set_desired_directions_by_position(needed_orientation.position);
        hand.target_quaternion = needed_orientation.rotation;
        rotate_to_desired_directions();
    }
    public override void rotate_to_desired_directions() {
        shoulder.rotate_to_desired_direction();
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

    


    
}
}