using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using Object = UnityEngine.Object;


namespace rvinowise.units.parts.weapons.guns {

public class Rifle: Gun {
    
    
    /* Child interface */
    public override Transform host {
        get { return _host; }
        set { _host = value; }
    }
    private Transform _host;

    private GameObject game_object;

    public override void fire() {
        
    }

    public override Object get_projectile() {
        throw new NotImplementedException();
    }
}
}