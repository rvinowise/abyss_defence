using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using units.equipment.segments;


namespace rvinowise.units.equipment.limbs  {

public class Arm: Tool {

    /* constant characteristics */
    public readonly Segment upper_arm;
    public readonly Segment forearm;
    
    public int folding_direction; //1 of -1

    /* Tool interface */
    public override Transform host {
        get { return upper_arm.host; }
        set { upper_arm.host = value; }
    }

    public override Vector2 attachment {
        get {
            return upper_arm.attachment_point;
        }
        set {
            upper_arm.attachment_point = value;
        }
    }

    
    public Arm(Transform inHost) {
        upper_arm = new Segment("femur");
        forearm = new Segment("tibia");
        upper_arm.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        forearm.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        forearm.host = upper_arm.transform;
        host = inHost;
    }
    
}
}