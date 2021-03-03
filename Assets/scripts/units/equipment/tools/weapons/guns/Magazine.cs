using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;
using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.units.parts.weapons.guns.common;

namespace rvinowise.unity.units.parts.weapons.guns {

public class Magazine : Ammunition {

    public int max_rounds_qty;// { get; set; }

    [SerializeField] public Projectile projectile_prefab;


    protected override void Awake() {
        base.Awake();
    }
    
    /* Tool interface */
    

    public Projectile retrieve_round(Transform in_muzzle) {
        if (rounds_qty > 0) {
            rounds_qty--;
            return get_bullet_object(in_muzzle);
        }
        return null;
    }

    public bool empty() {
        Contract.Requires(rounds_qty >=0, "number of rounds can't be negative");
        return rounds_qty == 0;
    }

    protected Projectile get_bullet_object(Transform in_muzzle) {
        Projectile projectile = projectile_prefab.get_from_pool<Projectile>(
            in_muzzle.position, in_muzzle.rotation
        );
        return projectile;
    }
}

}