using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.equipment;
using units.equipment.transport;


namespace units.control {

/* asks tool controllers what they can do,
 orders some actions from them based on this information,
 they do these actions later in the same update step */
public abstract class Intelligence {
    
    public User_of_equipment user_of_equipment;
    
    public ITransporter transporter {
        get { return user_of_equipment.transporter; }
        set => user_of_equipment.transporter = value;
    }
    public IWeaponry weaponry {
        get { return user_of_equipment.weaponry; }
        set => user_of_equipment.weaponry = value;
    }

    public Intelligence(User_of_equipment in_user) {
        user_of_equipment = in_user;
    }
    
    public virtual void update() {

    }
}
}