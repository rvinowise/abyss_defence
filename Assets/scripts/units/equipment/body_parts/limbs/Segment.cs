using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.helpers.graphics;
using System.Linq;

namespace rvinowise.unity.units.parts.limbs {

public class Segment: Turning_element {
    /* constant characteristics. they are written during construction */

    [HideInInspector]
    public SpriteRenderer sprite_renderer;
    public Direction_adjustor direction_adjustor;

    public Vector2 tip {
        get {
            return transform.position + transform.rotation * localTip;
        }
    }

    public Vector2 localTip {
        get {
            return Vector2.right * length;
        }
        set {
            Contract.Requires(
                value.y == 0,
                "tip of a Segment should lay on the straight line"
            );
            length = value.magnitude;
        }
    }
    
    [HideInInspector]
    public float length; 

    
    protected Segment parent_segment;

    public virtual Vector3 desired_tip {
        get {
            if (parent_segment == null) {
                return this.transform.position + localTip.rotate(target_rotation);
            }
            return parent_segment.desired_tip + 
                   localTip.rotate(target_rotation);
        }
    }


    protected override void Awake() {
        base.Awake();
        if (direction_adjustor == null) {
            direction_adjustor = this.GetComponentInDirectChildren<Direction_adjustor>();
        }
        if (sprite_renderer == null) {
            if (direction_adjustor != null) {
                sprite_renderer = direction_adjustor.GetComponent<SpriteRenderer>();
            } else {
                sprite_renderer = GetComponent<SpriteRenderer>();
            }
        }

        
        
    }

    

    protected virtual void Start() {
        //base.Start();
        
    }

    public void init() {
        if (transform.parent) {
            parent_segment = transform.parent.GetComponent<Segment>();
        }
        init_lengths();
    }

    /* private void init_lengths() {
        if (parent_segment != null) {
            parent_segment.localTip = this.transform.localPosition;
        }
    } */
    private void init_lengths() {
        Segment next_segment = this.GetComponentInDirectChildren<Segment>();
        Transform tip_tramsform = transform.Find("tip");
        Contract.Assert(
            (next_segment==null)!=(tip_tramsform==null),
            "tip of a segment should be assigned either by next segment of by the tip-transform"
        );
        if (next_segment!=null) {
            localTip = next_segment.transform.localPosition;
        } else if (tip_tramsform != null) {
            localTip = tip_tramsform.localPosition;
        }
    }

    public static Segment create(string in_name) {
        GameObject game_object = new GameObject(in_name);
        var new_component = game_object.add_component<Segment>();
        return new_component;
    }
    
    public virtual void mirror_from(limbs.Segment src) {
        transform.localPosition = new Vector2(
            src.transform.localPosition.x,
            -src.transform.localPosition.y
        );
        
        possible_span = src.possible_span.mirror().init_for_direction(-src.local_degrees);
        
        if (sprite_renderer != null) {
            sprite_renderer.sprite = src.sprite_renderer.sprite;
            sprite_renderer.flipY = !src.sprite_renderer.flipY;
        }
        if (direction_adjustor != null) {
            direction_adjustor.transform.localRotation = 
                direction_adjustor.transform.localRotation.inverse();
        }
    }

    public void init_length_to(Segment next_segment) {
        length = (transform.position - next_segment.transform.position).magnitude;
    }


    

    
    public void debug_draw_line(Color color, float time = 0.1f) {
        rvinowise.unity.debug.Debug.DrawLine_simple(
            transform.position, 
            tip, 
            color,
            3f
        );
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
       /*  if (is_leaf_segment()) {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(this.tip, 0.04f);
        } */
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(this.tip, 0.04f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(this.desired_tip, 0.04f);
    }

    private bool is_leaf_segment() {
        return GetComponentsInChildren<Segment>().Count() ==1;
    }
    
}

}