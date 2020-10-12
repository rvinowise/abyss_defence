using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rvinowise.units.parts.limbs.creeping_legs {



public class Stable_leg_group {
    public List<Leg> legs;
    
    public Stable_leg_group(List<Leg> in_legs) {
        legs = in_legs;
        foreach(Leg leg in legs) {
            leg.stable_group = this;
        }
    }
    
    internal bool contains(Leg leg)
    {
        return leg.stable_group == this;
    }
    
    public bool all_down() {
        foreach(Leg leg in legs) {
            if (leg.is_up) {
                return false;
            }
        }
        return true;
    }
}

}