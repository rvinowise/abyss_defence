using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.equipment.limbs.arms  {

public class Arm: Child, ILimb2 {

    /* constant characteristics */
    public Segment upper_arm { get; set; }
    public Segment forearm{ get; set; }
    
    public int folding_direction { get; set; }//1 of -1



    /* Child interface */
    public override Transform host {
        get { return upper_arm.host; }
        set { upper_arm.host = value; }
    }

    public override Vector2 attachment {
        get {
            return upper_arm.attachment;
        }
        set {
            upper_arm.attachment = value;
        }
    }

    
    /* ILimb2 interface */
    public Segment segment1 {
        get { return upper_arm;}
    }
    public Segment segment2 {
        get { return forearm;}
    }
    
    
    /* Arm itself */
    
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