using System;
using System.Collections;
using System.Collections.Generic;
using geometry2d;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.limbs.arms;
using rvinowise.units.parts.tools;
using Object = UnityEngine.Object;


namespace rvinowise.units.parts.weapons.guns {

public class Pump_shotgun: Gun {
    
    public override float weight { set; get; } = 6f;
    public override float stock_length { get; } = 0.45f;
    
    protected override void init_holding_places() {
        main_holding = Holding_place.main(this);
        second_holding = new Holding_place(this) {
            attachment_point = new Vector2(0.54f, 0f),
            grip_gesture = Hand_gesture.Support_of_horizontal,
            grip_direction = new Degree(-70f)
        };
    }
    
    public override Object get_projectile() {
        throw new NotImplementedException();
    }
    
}
}