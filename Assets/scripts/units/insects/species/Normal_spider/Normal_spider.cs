﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using geometry2d;
using rvinowise.units.equipment;
using rvinowise.units.equipment.limbs;
using units.equipment.transport;


namespace rvinowise.units.normal_spider {

public class Normal_spider: Creature {
    protected override void Awake() {
        base.Awake();
    }

    

    protected override ITransporter create_transporter() {
        Leg_controller leg_controller =
            user_of_equipment.add_equipment_controller<Leg_controller>();
        init.Legs.init(
            leg_controller
        );
        //leg_controller.init();
        return leg_controller;
    }

    protected IWeaponry weaponry {
        get { throw new NotImplementedException(); }
    }
}
}