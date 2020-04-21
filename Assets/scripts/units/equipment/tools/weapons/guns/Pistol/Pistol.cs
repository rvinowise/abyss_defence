using geometry2d;
using rvinowise.units;
using rvinowise.units.parts.limbs.arms;
using rvinowise.units.parts.tools;
using UnityEngine;

namespace rvinowise.units.parts.weapons.guns {

public class Pistol: 
    Gun {

    [SerializeField]
    public Transform shell_ejector;
    public Slot magazine_slot { get; private set; }
    //public GameObject projectile;
    
    protected override void init_components() {
        base.init_components();
        magazine_slot = GetComponentInChildren<Slot>();
    }
    
    protected override void init_holding_places() {
        main_holding = Holding_place.main(this);
        second_holding = new Holding_place(this) {
            grip_gesture = Hand_gesture.Support_of_horizontal,
            grip_direction = new Degree(-45f)
        };
    }
    
    
    /* Gun interface */

    //public override GameObject projectile { get; set; }

    public override float time_to_readiness() {
        return 0;
    }


}
}