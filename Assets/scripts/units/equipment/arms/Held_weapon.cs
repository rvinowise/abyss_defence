using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.weapons;


namespace rvinowise.units.parts.limbs.arms {

public class Held_weapon: IWeapon {
    
    public Gun gun;

    //private IList<Arm> arms = new List<Arm>();
    public Arm trigger_arm;
    public Arm stock_arm;

    public float time_to_shooting(Transform target) {
        float time_for_rotation = time_to_aim_at(target);
        float need_to_wait = time_to_readiness();
        return Math.Max(need_to_wait, time_for_rotation);
    }

    public void fire() {
        gun.pull_trigger();
    }

    public void shoot(Transform target) {
        
    }
    

    public float time_to_readiness() {
        return gun.time_to_readiness();
    }

    public float time_to_aim_at(Transform target) {
        return 0;
    }
    

}
}