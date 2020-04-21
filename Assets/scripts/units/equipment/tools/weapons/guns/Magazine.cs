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

public class Magazine : Tool {

    public virtual int max_rounds_qty { get; set; }
    [SerializeField] public int rounds_qty;

    [SerializeField] public Bullet bullet_prefab;

    public Magazine() { }

    protected override void Awake() {
        base.Awake();
    }
    
    /* Tool interface */
    protected override void init_holding_places() {
        main_holding =  new Holding_place(this) {
            is_main = true,
            place_on_tool = new Vector2(-0.1f, 0f),
            grip_gesture = Hand_gesture.Relaxed,
            grip_direction = new Degree(-30f)
        };
    }

    public Bullet retrieve_round() {
        if (rounds_qty > 0) {
            rounds_qty--;
            return get_bullet_object();
        }
        return null;
    }

    public bool empty() {
        Contract.Requires(rounds_qty >=0, "number of rounds can't be negative");
        return rounds_qty == 0;
    }

    protected Bullet get_bullet_object() {
        return Instantiate(bullet_prefab, transform.position, transform.rotation);
    }
}

}