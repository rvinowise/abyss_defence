using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;


namespace rvinowise.units.parts.limbs.arms  {

public class Arm: Child, ILimb3 {

    /* constant characteristics */
    public Segment upper_arm { get; set; }
    public Segment forearm{ get; set; }
    
    public Segment hand { get; set; }
    
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
    public Segment segment3 {
            get { return hand;}
        }
    
    
    /* Arm itself */
    
    public Arm(Transform inHost) {
        upper_arm = new Segment("upper_arm");
        upper_arm.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        upper_arm.host = inHost;
        
        forearm = new Segment("forearm");
        forearm.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        forearm.game_object.GetComponent<SpriteRenderer>().sortingOrder = -1;
        forearm.host = upper_arm.transform;

        hand = new Segment("hand");
        hand.game_object.GetComponent<SpriteRenderer>().sortingLayerName = "arms";
        hand.game_object.GetComponent<SpriteRenderer>().sortingOrder = -10;
        hand.host = forearm.transform;
    }

    public void update() {
        upper_arm.update();
        forearm.update();
        hand.update();
    }


}
}