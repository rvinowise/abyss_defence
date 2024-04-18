using System;
using rvinowise.unity.actions;
using UnityEngine;
using Action = System.Action;


namespace rvinowise.unity {

/* ILeg could substitute this class, but ALeg is needed for the inspector's serialization.
It uses composition instead of inheritance */
public abstract class ALeg : 
    Limb2
    ,ILeg
    ,IActor_attacker
{
    public float provided_impulse = 0.2f;
    public float get_provided_impulse() => provided_impulse;

    /* group of legs that being on the ground with this leg provide stability */
    public virtual Stable_leg_group stable_group{get;set;}
    
    /* where the end of the Leg should be for the maximum comfort, if the body is standing steel */
    public Transform optimal_relative_position_standing_transform;
    public virtual Vector2 get_optimal_position_standing() => optimal_relative_position_standing_transform.position;
    public Transform get_optimal_relative_position_standing_transform() => optimal_relative_position_standing_transform;
    
    /* where the tip of it should reach, according to moving plans */
    [HideInInspector]
    public Vector2 optimal_position{get;protected set;}
    
    // maximum distance from the optimal point after which the leg should be repositioned
    public float reposition_distance = 0.3f; //_reposition_istance formerly serialised
    public float get_reposition_distance() => reposition_distance;//{get;protected set;}

    
    public float moving_offset_distance = 0.3f;
    public virtual float get_moving_offset_distance() => moving_offset_distance;

    [HideInInspector]
    public Vector2 holding_point{get;protected set;}

    public bool up = true;
    public virtual bool is_up() => up;

    [SerializeField] public String action_label;
    
    public abstract void draw_positions();
    public abstract bool hold_onto_ground();
    public abstract bool is_twisted_uncomfortably();
    public virtual void raise_up() {
        up = true;
    }
    public abstract void put_down();


    public virtual void set_desired_position(Vector2 in_optimal_position) {
        optimal_position = in_optimal_position;
        base.set_desired_directions_by_position(optimal_position);
    }


    public virtual void Awake() {
        Init_segmented_limbs.init_segmented_limbs(this.gameObject);
    }

    
    #region IActor_attacker
    public override void on_lacking_action() {
        Creeping_leg_partakes_in_moving.create(this).start_as_root(action_runner);
    }

    public bool can_reach(Transform target) {
        var distance_to_target =
            ((Vector2)target.position - (Vector2)transform.position).magnitude;
        
        return get_reaching_distance() >= distance_to_target;
    }


    public abstract void attack(Transform target, Action on_completed = null);

    #endregion


 
}

}