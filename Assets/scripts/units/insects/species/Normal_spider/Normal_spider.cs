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


    public override ITransporter transporter {
        get { return sprider_transporter; }
        protected set { sprider_transporter = value as Creeping_leg_group; }
    }
    private Creeping_leg_group sprider_transporter;


    protected override void create_equipment() {
        sprider_transporter = new Creeping_leg_group(divisible_body);
    }
    
    protected override void fill_equipment_with_children() {
        init.Legs.init(sprider_transporter);
    }

  
    


    
}
}