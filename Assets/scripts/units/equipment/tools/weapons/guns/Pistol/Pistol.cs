using rvinowise.unity.geometry2d;
using rvinowise.unity.units;
using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.unity.units.parts.tools;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts.weapons.guns {

public abstract class Pistol: 
    Gun {

    [SerializeField]
    public Transform shell_ejector;
    public Slot magazine_slot { get; private set; }
    //public GameObject projectile;
    
    protected override void init_components() {
        base.init_components();
        magazine_slot = GetComponentInChildren<Slot>();
    }
    
    
    /* Gun interface */

    //public override GameObject projectile { get; set; }

    public override float time_to_readiness() {
        return 0;
    }

    protected override bool can_fire() {
        return ready_to_fire()&&
               (ammo_qty > 0);
    }




    
}
}