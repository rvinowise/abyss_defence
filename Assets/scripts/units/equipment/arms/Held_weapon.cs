using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.equipment.limbs.arms;
using rvinowise.units.equipment.weapons;


namespace rvinowise.units.equipment.limbs {

public class Held_weapon: IWeapon {
    
    public Weapon weapon;

    //private IList<Arm> arms = new List<Arm>();
    public Arm trigger_arm;
    public Arm stock_arm;

    public float time_to_shooting(Transform target) {
        var needed_direction = ((Vector2)target.position - position);
        float time_for_rotation = time_to_rotate_to(needed_direction);
        float need_to_wait = time_to_readiness();
        return Math.Max(need_to_wait, time_for_rotation);
    }

    public void fire() {
        weapon.pull_trigger();
    }

    public void shoot(Transform target) {
        
    }
    

    public float time_to_readiness() {
        return 0;
    }

    public float time_to_rotate_to(Vector2 needed_direction) {
        
    }
    

}
}