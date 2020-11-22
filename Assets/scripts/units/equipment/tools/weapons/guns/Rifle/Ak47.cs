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

public class Ak47: Gun {
    
    public override float weight { set; get; } = 4f;
    public override float stock_length { get; } = 0.45f;
    
    protected override void init_holding_places() {
        main_holding = Holding_place.main(this);
        second_holding = Holding_place.create(this.transform);
        second_holding.place_on_tool = new Vector2(0.45f, 0f);
        second_holding.grip_gesture = Hand_gesture.Support_of_horizontal;
        second_holding.grip_direction = new Degree(-80f);
    }
    


}
}