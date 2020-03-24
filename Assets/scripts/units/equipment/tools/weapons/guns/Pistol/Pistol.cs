using geometry2d;
using rvinowise.units;
using rvinowise.units.parts.limbs.arms;
using rvinowise.units.parts.tools;
using UnityEngine;

namespace rvinowise.units.parts.weapons.guns {

public class Pistol: 
    Gun
{

    protected override void init_holding_places() {
        main_holding = new Holding_place(this);
        second_holding = new Holding_place(this) {
            grip_gesture = Hand_gesture.Relaxed,
            grip_direction = new Degree(-45f)
        };
    }
    
    
    /* Gun interface */
    public override void fire() {
    }

    public override Object get_projectile() {
        throw new System.NotImplementedException();
    }

    public override float time_to_readiness() {
        return 0;
    }
    
}
}