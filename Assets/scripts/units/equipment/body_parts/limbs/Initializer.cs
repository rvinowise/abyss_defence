using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using UnityEditor;


namespace rvinowise.units.parts.limbs.init{

public static class Initializer {

    public static void mirror(ILimb2 dst, ILimb2 src) {
        // the base direction is to the right
        dst.segment1.mirror_from(src.segment1);
        dst.segment2.mirror_from(src.segment2);

        dst.folding_direction = src.folding_direction * -1;
    }
    
    public static void mirror(ILimb3 dst, ILimb3 src) {
        mirror((ILimb2)dst, (ILimb2)src);
        dst.segment3.mirror_from(src.segment3);
    }

    
    
}
}