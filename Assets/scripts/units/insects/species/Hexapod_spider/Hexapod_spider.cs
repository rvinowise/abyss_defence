using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.transport;

namespace rvinowise.unity.units.hexapod_spider {

public class Hexapod_spider:Creature {

    public override ITransporter transporter => sprider_transporter;
    public Creeping_leg_group sprider_transporter;


    protected override void create_equipment() {

    }
    
    protected override void fill_equipment_with_children() {
        init.Legs.init(sprider_transporter);
    }
    
    

 
}
}