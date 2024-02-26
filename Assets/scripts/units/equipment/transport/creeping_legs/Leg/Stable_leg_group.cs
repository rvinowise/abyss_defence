using System;
using System.Collections.Generic;


namespace rvinowise.unity {


[Serializable]
public class Stable_leg_group {
    public List<ALeg> legs;
    
    public Stable_leg_group(List<ALeg> in_legs) {
        legs = in_legs;
        foreach(ALeg leg in legs) {
            leg.stable_group = this;
        }
    }
    
    internal bool contains(ALeg leg)
    {
        return leg.stable_group == this;
    }
    
    public bool all_down() {
        foreach(ALeg leg in legs) {
            if (leg.is_up()) {
                return false;
            }
        }
        return true;
    }
}

}