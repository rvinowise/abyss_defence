using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts;
using rvinowise.unity.management;
using rvinowise.unity.units.control;

namespace rvinowise.unity.units {

public class Unit: Turning_element {
    protected Intelligence intelligence;

    protected override void Awake() {
        intelligence = GetComponent<Intelligence>();
    }


}


}