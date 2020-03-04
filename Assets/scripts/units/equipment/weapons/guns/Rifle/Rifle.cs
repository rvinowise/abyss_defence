using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.equipment.weapons;


namespace rvinowise.units.equipment.guns {

public class Rifle: Weapon {
    
    
    /* Child interface */
    public override Transform host {
        get { return _host; }
        set { _host = value; }
    }
    private Transform _host;

    private GameObject game_object;

    public override void fire() {
        
    }
}
}