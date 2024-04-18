using UnityEngine;


namespace rvinowise.unity {

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

    public override bool can_fire() {
        return 
            !is_on_cooldown()&&
            ammo_qty > 0;
    }




    
}
}