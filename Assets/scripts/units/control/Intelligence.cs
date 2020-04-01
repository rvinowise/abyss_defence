using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts;
using rvinowise.units.parts.sensors;
using rvinowise.units.parts.transport;
using Baggage = rvinowise.units.parts.Baggage;


namespace rvinowise.units.control {

/* asks tool controllers what they can do,
 orders some actions from them based on this information,
 they do these actions later in the same preserve_possible_rotations step */
public abstract class Intelligence {
    

    public Baggage baggage;

    public ISensory_organ sensory_organ;

    public ITransporter transporter { get; set; }

    public IWeaponry weaponry{ get; set; }

    public Intelligence() {
    }
    
    public virtual void update() {
        read_input();
    }

    protected virtual void read_input() {
        
    }
}
}