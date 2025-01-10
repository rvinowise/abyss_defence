using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public partial class Creeping_leg_group: 
    Abstract_children_group
    ,ITransporter
    //,IAttacker
{
    
    #region Children_group
    public override IEnumerable<IChild_of_group> get_children() {
        return legs;
    }

    internal override void Awake() {
        base.Awake();
        
        if (moved_body != null) {
            set_moved_body(moved_body);
        }
        actor = GetComponent<Actor>();
    }

    protected override void Start() {
        base.Start();
        
        guess_moving_strategy();

        
    }

    private ALeg find_dominant_leg(Transform body, IEnumerable<ALeg> legs) {
        ALeg front_leg = null;
        Vector3 front_position = new Vector3(float.MinValue,0,0);
        foreach(var leg in legs) {
            var leg_position = body.InverseTransformPoint(leg.transform.position);
            if (leg_position.x > front_position.x) {
                front_position = leg_position;
                front_leg = leg;
            }
        }
        return front_leg;
    }
    

    public override void add_child(IChild_of_group compound_object) {
        Contract.Requires(compound_object is ALeg);
        ALeg leg = compound_object as ALeg;
        legs.Add(leg);
        leg.transform.SetParent(transform, false);
    }


    public override void shift_center(Vector2 in_shift) {
        foreach (ILeg leg in legs) {
            leg.transform.localPosition += (Vector3)in_shift;
            //leg.optimal_position_standing = (leg.optimal_position_standing + in_shift);
        }
    }
    #endregion

    

    #region ITransporter


    public Turning_element moved_body;

    public void set_moved_body(Turning_element in_body) {
        moved_body = in_body;
        rigid_body = in_body.GetComponent<Rigidbody2D>();
        dominant_leg =
            find_dominant_leg(moved_body.transform, legs);
    }

    public Turning_element get_moved_body() {
        return moved_body;
    }

    public static float belly_friction_multiplier = 0.9f;

    private float possible_impulse;

    private void update_possible_impulse() {
        possible_impulse = 0;
        if (moving_strategy is null) {
            return;
        }
        foreach (ILeg leg in legs) {
            possible_impulse += leg.get_provided_impulse();
        }
        if (moving_strategy.belly_touches_ground()) {
            //possible_impulse *= belly_friction_multiplier;
        }
    }



    public float rotate_faster_than_move = 100f;

    public float get_possible_rotation() {
        return get_possible_impulse() * rotate_faster_than_move; 
    }

    public float get_possible_impulse() {
        return possible_impulse;
    }


    protected void move_in_direction(Vector2 moving_direction) {
        Vector2 delta_movement = (rvi.Time.deltaTime * get_possible_impulse() * moving_direction );
        rigid_body.AddForce(delta_movement*Physics_consts.rigidbody_impulse_multiplier);
    }
    

    protected virtual void Update() {
        move_legs();
    }


    protected Vector2 moving_vector;
    public virtual void move_towards_destination(Vector2 destination) {
        moving_vector = (destination - (Vector2) this.transform.position).normalized;
        move_in_direction(moving_vector);
    }
    
    public virtual void face_rotation(Quaternion face_direction) {
        moved_body.rotation_acceleration = get_possible_rotation();
        moved_body.rotation_slowing = moved_body.rotation_acceleration;
        moved_body.set_target_rotation(face_direction);
        moved_body.rotate_to_desired_direction();
    }
    
   
    #endregion
    
    
    #region Creeping_leg_group itself
    
    /* legs that are enough to be stable if they are on the ground */
    public List<Stable_leg_group> stable_leg_groups = new List<Stable_leg_group>();
    public ALeg dominant_leg;
    public List<ALeg> legs;
    
    
    public void set_moving_strategy(Moving_strategy strategy) {
        moving_strategy = strategy;
        update_possible_impulse();
    }
    public Moving_strategy moving_strategy;


    protected Rigidbody2D rigid_body;

    public void guess_moving_strategy() {
        if (stable_leg_groups.Count > 1) {
            set_moving_strategy(new Stable(legs, this));
        } else if (legs.Count > 1 ) {
            set_moving_strategy(new Grovelling(legs, this));
        } else if (legs.Count == 1) {
            set_moving_strategy(new Faltering(legs, this));
        }
    }


    private void move_legs() {
        foreach (ILeg leg in legs) {
            if (!(leg.actor.current_action is Creeping_leg_partakes_in_moving)) {
                continue;
            }
            leg.set_desired_position(get_optimal_position_for(leg)); 
            if (leg.is_up()) {
                move_in_the_air(leg);
            } else {
                moving_strategy.move_on_the_ground(leg);
            }
        }
    }
    
    private void move_in_the_air(ILeg leg) {
        if (leg.has_reached_aim()) {
            put_down(leg);
        } else {
            leg.move_segments_towards_desired_direction();
        }
    }

    private void put_down(ILeg leg) {
        Contract.Requires(leg.is_up());
        leg.put_down();
    }

    
    private Vector2 get_optimal_position_for(ILeg leg) {
        Vector2 shift_to_moving_direction =
            moving_vector * (leg.get_moving_offset_distance() * leg.transform.lossyScale.x);

        return leg.get_optimal_position_standing() + 
               shift_to_moving_direction;
    }

    public void ensure_leg_raised(ILeg leg) {
        if (leg.is_up()) {
            return;
        }
        moving_strategy.raise_up(leg);
    }


    bool can_move() {
        foreach (ILeg leg in legs) {
            if (!leg.is_up()) {
                return true;
            }
        }
        return false;
    }

    #region IActor_attacker

    public bool is_weapon_ready_for_target(Transform target) {
        foreach (var leg in legs) {
            if (leg.is_weapon_ready_for_target(target)) {
                return true;
            }
        }
        return false;
    }

    public float get_reaching_distance() {
        if (legs.Any()) {
            return dominant_leg.get_reaching_distance();
        }
        return 0;
    }

    public void attack(Transform target, System.Action on_completed = null) {
        foreach (var leg in legs) {
            if (leg.is_weapon_ready_for_target(target)) {
                leg.attack(target,on_completed);
                break;
            }
        }
    }

    #endregion

 

    #endregion

   

    #region IActor

    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(actor).start_as_root(actor.action_runner);
    }
    
    #endregion
}

}

