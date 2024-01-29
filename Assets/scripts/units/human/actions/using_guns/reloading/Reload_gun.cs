using UnityEngine;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.humanoid;
using rvinowise.unity.units.parts.actions;


namespace rvinowise.unity.units.parts.limbs.arms.actions.using_guns.reloading {

public class Reload_gun: Action_sequential_parent {
    
    protected Animator animator;
    protected Humanoid body;
    
    public Arm gun_arm;
    public Arm ammo_arm;

    
    protected bool should_be_flipped() {
        if (
            gun_arm.side == Side_type.LEFT
        ) {
            return true;
        } 
        return false;
    }



    
}
}