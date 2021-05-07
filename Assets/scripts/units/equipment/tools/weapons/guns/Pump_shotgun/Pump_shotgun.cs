using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts.limbs.arms;
using rvinowise.unity.units.parts.tools;
using Object = UnityEngine.Object;


namespace rvinowise.unity.units.parts.weapons.guns {

public class Pump_shotgun: Gun {
    
    [SerializeField]
    public Transform shell_ejector;
    public Slot reloading_slot { get; private set; }
    
    public override float weight { set; get; } = 6f;
    public override float stock_length { get; } = 0.45f;


    public int max_rounds = 12;
    
    /* current values */
    public int rounds_n;
    
    protected override void init_holding_places() {
        main_holding = Holding_place.main(this);
        second_holding = Holding_place.create(transform);
        second_holding.place_on_tool = new Vector2(0.54f, 0f);
        second_holding.grip_gesture = Hand_gesture.Support_of_horizontal;
        second_holding.grip_direction = new Degree(-70f);
    }
    
    protected override void init_components() {
        base.init_components();
        reloading_slot = GetComponentInChildren<Slot>();
    }

    public override void insert_ammunition(Ammunition in_ammunition) {
        if (rounds_n < max_rounds) {
            rounds_n++;
        }
    }

    public bool can_apply_ammunition(Ammunition in_ammunition) {
        return rounds_n < max_rounds;
    }

    protected override void fire() {

    }
}
}