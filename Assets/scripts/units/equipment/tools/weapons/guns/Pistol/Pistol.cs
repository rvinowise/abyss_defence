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
        main_holding = Holding_place.create(transform);
        main_holding.is_main = true;
        main_holding.grip_gesture = Hand_gesture.Grip_of_vertical;
        
        second_holding = Holding_place.create(transform);
        second_holding.is_main = false;
        second_holding.grip_gesture = Hand_gesture.Support_of_horizontal;
        second_holding.grip_direction = new Degree(-45f);
    }
    
    
    /* Gun interface */

    //public override GameObject projectile { get; set; }

    public override float time_to_readiness() {
        return 0;
    }

    protected override bool can_fire() {
        return ready_to_fire()&&
               (magazine!=null) &&
               !magazine.empty();
    }

    public void insert_magazine(Magazine in_magazine) {
        magazine = in_magazine;
    }

    public override void apply_ammunition(Ammunition in_ammunition) {
        base.apply_ammunition(in_ammunition);
    }
}
}