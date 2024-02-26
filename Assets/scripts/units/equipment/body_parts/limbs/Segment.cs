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
                return this.transform.position + localTip.rotate(target_rotation);
            }
            return parent_segment.desired_tip + 
                   localTip.rotate(target_rotation);
        }
    }


    protected override void Awake() {
        base.Awake();
        if (sprite_renderer == null) {
            sprite_renderer = GetComponent<SpriteRenderer>();
        }
    }

    
    

    public static Segment create(string in_name) {
        GameObject game_object = new GameObject(in_name);
        var new_component = game_object.add_component<Segment>();
        return new_component;
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
        Gizmos.DrawSphere(this.tip, 0.02f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(this.desired_tip, 0.02f);
    }

    private bool is_leaf_segment() {
        return GetComponentsInChildren<Segment>().Count() ==1;
    }
    
}

}