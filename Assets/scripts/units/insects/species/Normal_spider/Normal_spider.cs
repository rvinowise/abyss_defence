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


namespace rvinowise.unity.units.normal_spider {

public class Normal_spider: Creature {
    
    public Creeping_leg_group sprider_transporter;

    public override ITransporter transporter {
        get { return sprider_transporter; }
        protected set { sprider_transporter = value as Creeping_leg_group; }
    }

    protected override void Awake() {
        sprider_transporter = GetComponent<Creeping_leg_group>();
        base.Awake();
    }
    
    protected override void create_equipment() {
    }
    
    protected override void fill_equipment_with_children() {
        init.Legs.init(sprider_transporter);
    }

  



    
}
}