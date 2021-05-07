using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.humanoid;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.units.parts.weapons.guns;
using units.human.actions;
using units;

namespace rvinowise.unity.units.parts.limbs.arms.actions.using_guns.reloading {

public class Reload_gun: Action_sequential_parent {
    
    protected Animator animator;
    protected Humanoid body;
    
    public Arm gun_arm;
    public Arm ammo_arm;

    
    protected bool should_be_flipped() {
        if (
            gun_arm.side == Side.LEFT
        ) {
            return true;
        } 
        return false;
    }



    
}
}