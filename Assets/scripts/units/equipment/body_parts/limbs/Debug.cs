using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using static geometry2d.Directions;


namespace rvinowise.units.parts.limbs {

public partial class Limb2 : ICompound_object { 
    public class Debug {
        protected virtual Limb2 limb2 { get; }
        protected Color problem_color = new Color(255,50,50);
        protected Color optimal_color = new Color(50,255,50);
        protected const float sphere_size = 0.05f;
        public string name;
        protected bool debug_off {
            get { return rvinowise.units.debug.Debugger.is_off; }
        }

        public Debug(Limb2 _parent_limb2) {
            limb2 = _parent_limb2;
        }

        public void draw_lines(Color color, float time=1f) {
            if (debug_off) {
                return;
            }
            UnityEngine.Debug.DrawLine(
                limb2.segment1.position, 
                limb2.segment1.transform.TransformPoint(limb2.segment1.tip), 
                color,
                time
            );
            UnityEngine.Debug.DrawLine(
                limb2.segment2.position, 
                limb2.segment2.transform.TransformPoint(limb2.segment2.tip), 
                color,
                time
            );
        }
        public void draw_desired_directions(float time=0.1f) {
            if (debug_off) {
                return;
            }
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
        }
        
    }

    
}
}    