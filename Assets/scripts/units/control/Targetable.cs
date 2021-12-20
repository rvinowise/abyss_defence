using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.sensors;
using rvinowise.unity.units.parts.transport;
using Baggage = rvinowise.unity.units.parts.Baggage;
using rvinowise.contracts;
using System.Linq;
using rvinowise.unity.management;
using rvinowise.unity.ui.input;

namespace rvinowise.unity.units.control {

/* autoaiming can target this object */
public class Targetable: 
MonoBehaviour 
{
  
    void OnMouseOver()
    {
        if (transform != Player_input.instance.player.transform) {
            Player_input.instance.player.aim_at(transform);
        }
    }
}
}