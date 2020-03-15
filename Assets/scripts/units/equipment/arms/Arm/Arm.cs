using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.arms.strategy;
using rvinowise.units.parts.weapons;
using Input = rvinowise.ui.input.Input;

namespace rvinowise.units.parts.limbs.arms  {

public class Arm: Limb3, IUse_strategies {

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

    /* parameters assigned by creators */
    public Baggage baggage; // where to take tools from
    public Transform idle_target; // pay attention to it, when idle
    
    private Gun held_tool;
    public arms.strategy.Strategy strategy;
    
    private Strategy_builder strategy_builder;
    
    public Arm(Transform inHost, Transform in_idle_target) {
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

        idle_target = in_idle_target;
        
        strategy_builder = new Strategy_builder(this);
        strategy = new Idle_vigilant(this, idle_target);


        debug = new Arm.Debug(this);
    }

    public void update() {
        strategy?.update();

        upper_arm.update();
        forearm.update();
        hand.update();
        
        debug?.draw_desired_directions();
    }




    public void take_tool_from_baggage() {
        Contract.Requires(held_tool == null, "must be free in order to grab a tool");

        /*strategy = new strategy.Put_hand_before_bag(this, baggage);
        strategy.next = new strategy.Move_hand_into_bag(this, baggage);
        strategy.next.next = new strategy.Put_hand_before_bag(this, baggage);
        strategy.next.next.next = new strategy.Idle_vigilant(this, idle_target);
        strategy.start();*/
        
        strategy = new strategy.Move_hand_into_loose_bag(this, baggage);
        strategy.next = new strategy.Idle_vigilant(this, idle_target);
        strategy.start();
        
        /*strategy_builder.add( );
        
        strategy_builder.add(
            Put_hand_before_bag.add(baggage).next = Move_hand_into_bag.add(baggage)
        ).add(
                
        )*/

    }

    /*private bool tool_touches_baggage() {
        return held_tool.transform.orientation() == get_orientation_touching_baggage();
    }*/

    
    
    
    
    
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