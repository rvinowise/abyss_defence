using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.limbs;


namespace rvinowise.unity.units.humanoid.init {

public class Head {

    public static void init(
        parts.head.Head head) 
    {
        head.possible_span = new Span(90,-90);
    }

}
}