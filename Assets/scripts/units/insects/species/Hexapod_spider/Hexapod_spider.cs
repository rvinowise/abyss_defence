using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using geometry2d;
using rvinowise.units.parts;
using rvinowise.units.parts.limbs;
using rvinowise.units.parts.limbs.creeping_legs;
using rvinowise.units.parts.transport;

namespace rvinowise.units.hexapod_spider {

public class Hexapod_spider:Creature {
    protected override void Awake() {
        base.Awake();
        
    }
    
    public override ITransporter transporter => _creeping_transporter;
    private Creeping_leg_group _creeping_transporter;


    protected override void create_equipment() {
        _creeping_transporter = new Creeping_leg_group(divisible_body, this);
        _creeping_transporter = new Creeping_leg_group(divisible_body, this);
    }
    
    protected override void fill_equipment_with_children() {
        init.Legs.init(_creeping_transporter);
    }
    
    

 
}
}