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

namespace rvinowise.unity.units.control {

/* Global intelligence, controlling all units of a team */
public class Commander: 
MonoBehaviour 
{
    
    List<Intelligence> units; 

    public Transform enemy;

    public static Commander instance{get;private set;}
    void Awake () {
        Contract.Requires(instance == null, "singleton");
        instance = this;
    }


    public void on_unit_iddling(Strategic_intelligence in_intelligence) {
        in_intelligence.unit_commands.attack_target = enemy;
    }


}
}