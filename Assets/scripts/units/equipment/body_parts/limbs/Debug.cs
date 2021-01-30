using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using static rvinowise.unity.geometry2d.Directions;


namespace rvinowise.unity.units.parts.limbs {

public partial class Limb2 { 
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

        
        
    }

    
}
}    