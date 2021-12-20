using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.contracts;
using rvinowise.unity.maps;

namespace rvinowise.unity.units.parts.tools {
public class Toolset: MonoBehaviour {

    public Tool right_tool;
    public Tool left_tool;

    public Tool get_tool_on_side(Side in_side) {
        if (in_side == Side.RIGHT) {
            return right_tool;
        }
        return left_tool;
    }
}




}
