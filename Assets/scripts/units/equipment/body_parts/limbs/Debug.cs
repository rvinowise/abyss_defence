using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using static rvinowise.unity.geometry2d.Directions;


namespace rvinowise.unity.units.parts.limbs {

public partial class Limb2 : IChild_of_group { 
    public class Debug {
        protected virtual Limb2 limb2 { get; }
        protected Color problem_color = new Color(255,50,50);
        protected Color optimal_color = new Color(50,255,50);
        protected const float sphere_size = 0.08f;
        public string name;
        protected bool debug_off {
            get { return rvinowise.unity.debug.Debugger.is_off; }
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

        public void draw_directions(float time = 0.1f) {
            UnityEngine.Debug.DrawLine(
                limb2.segment1.position, 
                limb2.segment1.global_tip,
                Color.yellow,
                time
            );
           
            UnityEngine.Debug.DrawLine(
                limb2.segment2.position,
                limb2.segment2.global_tip,
                Color.yellow,
                time
            );
        }
        public void draw_desired_directions(float time=0.1f) {
            if (debug_off) {
                return;
            }
            UnityEngine.Debug.DrawLine(
                limb2.segment1.position, 
                limb2.segment1.get_desired_tip(),
                Color.cyan,
                time
            );
           
            UnityEngine.Debug.DrawLine(
                limb2.segment1.get_desired_tip(),
                limb2.segment2.get_desired_tip(),
                Color.white,
                time
            );

            /* rvinowise.unity.debug.Debug.DrawLine(
                limb2.segment1.position, 
                limb2.segment1.position+limb2.segment1.target_direction.rotation * (Vector2.right/2),
                Color.yellow,
                3,
                time
            );*/
            
        }
        
    }

    
}
}    