using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.arms;
using rvinowise.units.parts.tools;


namespace rvinowise.units.parts.weapons.guns {

public class Magazine : Ammunition {

    public virtual int max_rounds_qty { get; set; }
    [SerializeField] public int rounds_qty;

    [SerializeField] public Projectile projectile_prefab;

    public Magazine() { }

    protected override void Awake() {
        base.Awake();
    }
    
    /* Tool interface */
    protected override void init_holding_places() {
        main_holding = transform.Find("main_holding_place")?.GetComponent<Holding_place>();
    }

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
        return Instantiate(projectile_prefab, in_muzzle.position, in_muzzle.rotation);
    }
}

}