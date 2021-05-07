using System;
using System.Collections;
using System.Collections.Generic;
using MoreLinq;
using UnityEngine;
using rvinowise.unity.extensions;

namespace units {

public interface IFlippable_actor {

    void flip_for_animation(bool in_flipped);
    void restore_after_flipping();
    
}
}