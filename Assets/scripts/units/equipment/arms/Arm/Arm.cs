using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.arms.strategy;
using rvinowise.units.parts.weapons;


namespace rvinowise.units.parts.limbs.arms  {

public class Arm: Limb3 {

    /* constant characteristics */
    public Segment upper_arm {
        get { return segment1 as arms.Segment;}
        set { segment1 = value; }
    }
    public Segment forearm {
        get { return segment2 as arms.Segment;}
        set { segment2 = value; }
    }
    
    public Segment hand {
        get { return segment3 as arms.Segment;}
        set { segment3 = value; }
    }
    

    /* Child interface */
    public override Transform host {
        get { return upper_arm.host; }
        set { upper_arm.host = value; }
    }

    public override Vector2 attachment {
        get {
            return upper_arm.attachment;
        }
        set {
            upper_arm.attachment = value;
        }
    }
    
    /* Arm itself */

    private Gun held_tool;
    public Baggage baggage;
    public arms.strategy.Strategy strategy;
    
    public Arm(Transform inHost) {
        upper_arm = new Segment("upper_arm");
        upper_arm.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        upper_arm.host = inHost;
        
        forearm = new Segment("forearm");
        forearm.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        forearm.game_object.GetComponent<SpriteRenderer>().sortingOrder = -1;
        forearm.host = upper_arm.transform;

        hand = new Segment("hand");
        hand.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        hand.game_object.GetComponent<SpriteRenderer>().sortingOrder = -10;
        hand.host = forearm.transform;
        
        strategy = new Idle_vigilant();
        
        debug = new Arm.Debug(this);
    }

    public void update() {

        apply_moving_strategy();
        
        debug.draw_desired_directions();
        upper_arm.update();
        forearm.update();
        hand.update();
    }


    public void take_tool_from_baggage() {
        Contract.Requires(held_tool == null, "must be free in order to grab a tool");

        strategy = new strategy.Reach_into_bag(baggage.transform);
        set_desired_directions(get_orientation_touching_baggage());
    }

    private bool tool_touches_baggage() {
        return held_tool.transform.orientation() == get_orientation_touching_baggage();
    }

    private void set_desired_directions(Orientation needed_tool_orientation) {
        set_desired_directions_by_position(needed_tool_orientation.position);
        hand.desired_direction = needed_tool_orientation.rotation;
    }
    
    private Orientation get_orientation_touching_baggage() {
        return new Orientation(
            baggage.position,// + (Vector2)(aggage.rotation * hand.tip),
            baggage.rotation * Directions.degrees_to_quaternion(180f),
            null
        );
        /*return new Orientation(
            Vector2.zero,
            Quaternion.Inverse(Quaternion.identity),
            baggage.transform
        );*/
    }
    
    
    public new class Debug: Limb2.Debug {
        private readonly Arm arm;

        protected override Limb2 limb2 {
            get { return arm; }
        }

        public Debug(Arm parent):base(parent) {
            arm = parent;
        }

        public void draw_desired_directions(float time=0.1f) {
            if (debug_off) {
                return;
            }
            //base.draw_desired_directions();
            
            UnityEngine.Debug.DrawLine(
                limb2.segment1.position, 
                limb2.segment1.position +
                (Vector2)(
                    limb2.segment1.desired_direction *
                    limb2.segment1.tip
                ),
                Color.cyan,
                time
            );
            Vector2 segment2_position =
                limb2.segment1.position +
                (Vector2) (limb2.segment1.desired_direction * limb2.segment1.tip);
            UnityEngine.Debug.DrawLine(
                segment2_position, 
                segment2_position + 
                (Vector2)(
                    limb2.segment2.desired_direction *
                    limb2.segment2.tip
                ),
                Color.white,
                time
            );
            
            Vector2 hand_position =
                segment2_position +
                (Vector2) (limb2.segment2.desired_direction * limb2.segment2.tip);
            UnityEngine.Debug.DrawLine(
                hand_position, 
                hand_position +
                (Vector2)(
                    arm.hand.desired_direction *
                    arm.hand.tip
                ),
                Color.cyan,
                time
            );
        }
    }
    public Arm.Debug debug {
        get { return _debug; }
        private set { _debug = value; }
    }
    private Arm.Debug _debug;

    protected override Limb2.Debug debug_limb {
        get { return _debug; }
    }
}
}