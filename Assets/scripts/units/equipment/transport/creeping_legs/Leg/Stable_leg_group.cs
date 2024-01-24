using System;
using System.Collections.Generic;


namespace rvinowise.unity.units.parts.limbs.creeping_legs {


[Serializable]
public class Stable_leg_group {
    public List<ILeg> legs;
    
    public Stable_leg_group(List<ILeg> in_legs) {
        legs = in_legs;
        foreach(ILeg leg in legs) {
            leg.stable_group = this;
        }
    }
    
    internal bool contains(ILeg leg)
    {
        return leg.stable_group == this;
    }
    
    public bool all_down() {
        foreach(ILeg leg in legs) {
            if (leg.is_up) {
                return false;
            }
        }
        return true;
    }
}

}