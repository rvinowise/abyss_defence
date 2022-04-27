using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts;


namespace rvinowise.unity.units.control {

public abstract class Strategic_intelligence:Intelligence {
    
    public Unit_commands unit_commands = new Unit_commands();
    public Commander commander; 

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        commander = Commander.instance;
        receive_orders();
    }

    protected void receive_orders() {
        commander.on_unit_iddling(this);
    }
}
}