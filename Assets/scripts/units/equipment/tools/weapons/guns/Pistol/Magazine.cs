using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using geometry2d;


namespace rvinowise.units.parts.weapons.guns.desert_eagle {

public class Magazine: guns.Magazine {

    public sealed override int max_rounds_qty { get; set; } = 9;

    public Magazine() {
        rounds_qty = max_rounds_qty;
    }

        
}
}