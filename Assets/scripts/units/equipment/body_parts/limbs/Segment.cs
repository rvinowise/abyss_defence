using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using System.Linq;

namespace rvinowise.unity {

public class Segment: Turning_element {

    public SpriteRenderer sprite_renderer;

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
    
    public float length; 

    
    public Segment parent_segment;

    public virtual Vector3 desired_tip {
        get {
            if (parent_segment == null) {
                return this.transform.position + localTip.rotate(get_target_rotation());
            }
            return parent_segment.desired_tip + 
                   localTip.rotate(get_target_rotation());
        }
    }


    protected override void Awake() {
        base.Awake();
        if (sprite_renderer == null) {
            sprite_renderer = GetComponent<SpriteRenderer>();
        }
    }
    

    public void init_length_to(Segment next_segment) {
        length = (transform.position - next_segment.transform.position).magnitude;
    }


    public void debug_draw_line(Color color) {
        rvinowise.unity.debug.Debug.DrawLine_simple(
            transform.position, 
            tip, 
            color,
            3f
        );
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
       
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(this.tip, 0.02f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(this.desired_tip, 0.02f);

        if (!is_within_span()) {
            debug_draw_line(Color.red);
        }
    }
#endif
    
    private bool is_leaf_segment() {
        return GetComponentsInChildren<Segment>().Count() ==1;
    }
    
}

}