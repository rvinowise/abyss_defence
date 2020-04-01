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

namespace rvinowise.units.hexapod_spider {

public class Hexapod_spider:Creature {
    protected override void Awake() {
        base.Awake();
        
    }
    
    public override ITransporter transporter => spider_transporter;
    private Spider_leg_group spider_transporter;


    protected override void create_equipment() {
        spider_transporter = new Spider_leg_group(divisible_body);
    }
    
    protected override void fill_equipment_with_children() {
        init.Legs.init(spider_transporter);
    }
    
    

 
}
}