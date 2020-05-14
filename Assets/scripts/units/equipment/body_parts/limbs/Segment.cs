using System.Runtime.CompilerServices;
using geometry2d;
using rvinowise.units.parts.weapons.guns.desert_eagle;
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
            _length = tip.magnitude;
            sqr_length = Mathf.Pow(length,2);
        }
    }
    protected Vector2 _tip;
    
    public float length {
        get { return _length; }
        set {
            _length = value;
            _tip = new Vector2(_length, 0);
            sqr_length = Mathf.Pow(length,2);
        }
    }
    private float _length;
    public float sqr_length {
        get;
        private set;
    }
    

    /* current characteristics */

  

    public static Segment create(string in_name) {
        GameObject game_object = new GameObject(in_name);
        var new_component = game_object.add_component<Segment>();
        return new_component;
    }
    
    public virtual void mirror_from(limbs.Segment src) {
        this.local_position = new Vector2(
            src.local_position.x,
            -src.local_position.y
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

        if (spriteRenderer != null) {
            this.spriteRenderer.sprite = src.spriteRenderer.sprite;
            this.spriteRenderer.flipY = !src.spriteRenderer.flipY;
        }
    }

    public void init_length_to(Segment next_segment) {
        length = (transform.position - next_segment.transform.position).magnitude;
    }
    public Vector2 desired_tip() {
        return this.position + tip.rotate(desired_direction);
    }

    


    
    
}

}