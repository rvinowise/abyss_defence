using System.Runtime.CompilerServices;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.weapons.guns.desert_eagle;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts.limbs {

public class Segment: Turning_element {
    /* constant characteristics. they are written during construction */
    public Vector2 tip {
        get {
            return _tip;
        }
        set {
            _tip = value;
            _absolute_length = tip.magnitude * this.transform.lossyScale.x;
            sqr_length = Mathf.Pow(_absolute_length,2);
        }
    }
    public Vector2 global_tip {
        get {
            return _tip * this.transform.lossyScale.x;
        }
    }
    protected Vector2 _tip;
    
    public float absolute_length {
        get { return _absolute_length; }
        set {
            _absolute_length = value;
            _tip = new Vector2(_absolute_length / this.transform.lossyScale.x, 0);
            sqr_length = Mathf.Pow(_absolute_length,2);
        }
    }
    private float _absolute_length;
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
        

        if (spriteRenderer != null) {
            this.spriteRenderer.sprite = src.spriteRenderer.sprite;
            this.spriteRenderer.flipY = !src.spriteRenderer.flipY;
        }
    }

    public void init_length_to(Segment next_segment) {
        absolute_length = (transform.position - next_segment.transform.position).magnitude;
    }
    public Vector2 desired_tip() {
        return this.position + tip.rotate(target_quaternion);
    }

    


    
    
}

}