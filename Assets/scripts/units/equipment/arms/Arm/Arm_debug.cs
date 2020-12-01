using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;
using rvinowise.unity.units.parts.limbs.arms.actions;

using Action = rvinowise.unity.units.parts.actions.Action;
using Input = rvinowise.unity.ui.input.Input;



namespace rvinowise.unity.units.parts.limbs.arms  {
public partial class Arm /*, IDo_actions*/ {
    
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
                    limb2.segment1.target_quaternion *
                    limb2.segment1.tip
                ),
                Color.cyan,
                time
            );
            Vector2 segment2_position =
                limb2.segment1.position +
                (Vector2) (limb2.segment1.target_quaternion * limb2.segment1.tip);
            UnityEngine.Debug.DrawLine(
                segment2_position, 
                segment2_position + 
                (Vector2)(
                    limb2.segment2.target_quaternion *
                    limb2.segment2.tip
                ),
                Color.white,
                time
            );
            
            Vector2 hand_position =
                segment2_position +
                (Vector2) (limb2.segment2.target_quaternion * limb2.segment2.tip);
            UnityEngine.Debug.DrawLine(
                hand_position, 
                hand_position +
                (Vector2)(
                    arm.hand.target_quaternion *
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
    
    
    private void TEST_draw_debug_lines() {
        //if (this.folding_direction == Side.LEFT) {
        debug?.draw_desired_directions();
        //debug?.draw_lines(Color.red, 10f);

        /*UnityEngine.Debug.Log(
            "direction_diff: "+ 
            ((
                 upper_arm.rotation.inverse() *
                 forearm.rotation
             ).to_float_degrees())
        );
        UnityEngine.Debug.Log(
            "upper_arm: "+ 
            (
                    upper_arm.rotation
                ).to_float_degrees()
        );
        UnityEngine.Debug.Log(
            "forearm: "+ 
            (
                forearm.rotation
            ).to_float_degrees()
        );*/
        // }
    }
    
}

}