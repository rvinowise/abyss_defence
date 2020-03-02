using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.equipment.weapons {

public abstract class Weapon: 
    Tool,
    IWeapon 
{

    
    public abstract void fire();


    public virtual float time_to_readiness() {
        return 0;
    }
}
}