using geometry2d;
using UnityEngine;

namespace rvinowise.units.parts.limbs {

public class Segment: Turning_element {
    /* constant characteristics. they are written during construction */
    public Vector2 tip {
        get {
            return _tip;
        }
        set {
            _tip = value;
            length = tip.magnitude;
            sqr_length = Mathf.Pow(length,2);
        }
    }
    private Vector2 _tip;
    
    public float length {
        get;
        private set;
    }
    public float sqr_length {
        get;
        private set;
    }
    

    /* current characteristics */

    public Segment(string name): base(name) {
        
    }

    public Segment(string name, GameObject prefab) : base(name, prefab) {
        
    }
    
    public virtual void mirror_from(limbs.Segment src) {
        this.attachment = new Vector2(
            src.attachment.x,
            -src.attachment.y
        );
        this.local_position = new Vector2(
            src.local_position.x,
            -src.local_position.y
        );
        
        this.possible_span = src.possible_span.mirror();
        this.tip = new Vector2(
            src.tip.x,
            -src.tip.y
        );
        
        //this.folding_direction = src.folding_direction * -1;

        this.spriteRenderer.sprite = src.spriteRenderer.sprite;
        this.spriteRenderer.flipY = !src.spriteRenderer.flipY;
    }

    


    
    
}

}