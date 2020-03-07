using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using geometry2d;
using rvinowise.units.parts;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.limbs.legs;
using rvinowise.units.parts.transport;


namespace rvinowise.units.normal_spider {

public class Normal_spider: Creature {
    protected override void Awake() {
        base.Awake();
    }

    

    protected override ITransporter create_transporter() {
        return init.Legs.init(
            user_of_equipment.add_equipment_controller<Leg_controller>()
        );
    }

    protected IWeaponry weaponry {
        get { throw new NotImplementedException(); }
    }
}
}