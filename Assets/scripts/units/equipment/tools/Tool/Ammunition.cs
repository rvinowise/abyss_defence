using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity.units.parts.tools {

public class Ammunition: Tool {
    public int rounds_qty;
    public int max_rounds_qty;
    public Ammo_compatibility compatibility;

    // public abstract int get_rounds_qty();

}
}