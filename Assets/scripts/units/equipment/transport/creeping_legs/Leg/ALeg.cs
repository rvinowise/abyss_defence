using rvinowise.unity.units.parts.limbs.arms.actions;
using UnityEngine;

namespace rvinowise.unity.units.parts.limbs.creeping_legs {

/* ILeg could substitute this class, but ALeg is needed for the inspector's serialization.
It uses composition instead of inheritance */
public abstract class ALeg : 
    Limb2
    ,ILeg
{
    public float provided_impulse => _provided_impulse;
    public float _provided_impulse = 0.2f;

    /* group of legs that being on the ground with this leg provide stability */
    public virtual Stable_leg_group stable_group{get;set;}
    
    /* where the end of the Leg should be for the maximum comfort, if the body is standing steel */
    public virtual Vector2 optimal_position_standing {
        get {return optimal_relative_position_standing_transform.position;}
        protected set {optimal_relative_position_standing_transform.position = value;}
    }
    public Transform optimal_relative_position_standing_transform => _optimal_relative_position_standing_transform;
    [SerializeField]
    private Transform _optimal_relative_position_standing_transform;
    
    /* where the tip of it should reach, according to moving plans */
    [HideInInspector]
    public Vector2 optimal_position{get;protected set;}
    
    // maximum distance from the optimal point after which the leg should be repositioned
    public float reposition_distance => _comfortable_distance;//{get;protected set;}
    [SerializeField]
    private float _comfortable_distance = 0.3f;

    public virtual float moving_offset_distance{
        protected set{_moving_offset_distance = value;}
        get{return _moving_offset_distance;}
    }
    [SerializeField]
    private float _moving_offset_distance = 0.3f;

    [HideInInspector]
    public Vector2 holding_point{get;protected set;}
    [HideInInspector]
    public virtual bool is_up{get;protected set;} = true;

    public abstract void draw_positions();
    public abstract bool hold_onto_ground();
    public abstract bool is_twisted_uncomfortably();
    public virtual void raise_up() {
        is_up = true;
    }
    public abstract void put_down();

    //public abstract void set_desired_directions_by_position(Vector2 target);

    public virtual void set_desired_position(Vector2 in_optimal_position) {
        optimal_position = in_optimal_position;
        base.set_desired_directions_by_position(optimal_position);
    }

    

    public override void on_lacking_action() {
        Creeping_leg_partakes_in_moving.create(this).start_as_root(action_runner);
    }


}

}