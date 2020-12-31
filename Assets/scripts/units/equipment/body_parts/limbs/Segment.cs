using System.Runtime.CompilerServices;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.weapons.guns.desert_eagle;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts.limbs {

[RequireComponent(typeof(SpriteRenderer))]
public class Segment: Turning_element {
    /* constant characteristics. they are written during construction */

    [HideInInspector]
    public SpriteRenderer sprite_renderer;

    public Transform parent => transform.parent;

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
    public Vector2 sized_tip {
        get {
            return _tip * this.transform.lossyScale.x;
        }
    }
    public Vector2 global_tip {
        get {
            return transform.position + sized_tip.rotate(base.rotation);
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

    private float degrees2tip;
    private Quaternion quaternion2tip;

    private Segment parent_segment;
    /* public float direction_practic2unity(float in_degrees) {

    } */

    public override Quaternion target_quaternion {
        get { 
            return base.target_quaternion * quaternion2tip;
        }
        set { 
            base.target_quaternion = value * quaternion2tip.inverse();
        }
    }

    public override Quaternion rotation {
        get {
            return base.rotation * quaternion2tip;
        }
        set {
            base.rotation = value * quaternion2tip.inverse();
        }
    }

    /* current characteristics */

    protected override void Awake() {
        base.Awake();
        sprite_renderer = GetComponent<SpriteRenderer>();
        
    }

    protected void Start() {
        degrees2tip = tip.to_dergees();
        quaternion2tip = tip.to_quaternion();

        parent_segment = transform.parent?.GetComponent<Segment>();
    }

    public static Segment create(string in_name) {
        GameObject game_object = new GameObject(in_name);
        var new_component = game_object.add_component<Segment>();
        return new_component;
    }
    
    public virtual void mirror_from(limbs.Segment src) {
        this.transform.localPosition = new Vector2(
            src.transform.localPosition.x,
            -src.transform.localPosition.y
        );
        this.transform.localPosition = new Vector2(
            src.transform.localPosition.x,
            -src.transform.localPosition.y
        );
        
        this.possible_span = src.possible_span.mirror();
        this.tip = new Vector2(
            src.tip.x,
            -src.tip.y
        );
        

        if (sprite_renderer != null) {
            this.sprite_renderer.sprite = src.sprite_renderer.sprite;
            this.sprite_renderer.flipY = !src.sprite_renderer.flipY;
        }
    }

    public void init_length_to(Segment next_segment) {
        absolute_length = (transform.position - next_segment.transform.position).magnitude;
    }
    public Vector3 get_desired_tip() {
        if (parent_segment == null) {
            return this.transform.position + tip.rotate(target_quaternion);
        }
        return parent_segment.get_desired_tip() + 
                        target_quaternion * (Vector2.right * this.absolute_length);/*  * global_tip */;
    }

    
    public void debug_draw_line(Color color, float time = 0.1f) {
        rvinowise.unity.debug.Debug.DrawLine(
            transform.position, 
            transform.TransformPoint(tip), 
            color,
            3f,
            time
        );
    }

    
    
}

}