using System;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public partial class Creeping_leg_group: 
    Children_group
    ,ITransporter
{
    
    #region Children_group
    public override IEnumerable<IChild_of_group> children {
        get => legs;
    }

    protected override void Awake() {
        base.Awake();
        init_components();
    }

    protected override void Start() {
        base.Start();
        
        guess_moving_strategy();

        Contract.Requires(GetComponents<Turning_element>().Length <= 1,
            "only one component with Turning_element is enough");
        moved_body = GetComponent<Turning_element>();
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
    
    private void init_components() {
        dominant_leg =
            find_dominant_leg(moved_body.transform, legs);
            
        
        rigid_body = gameObject.GetComponent<Rigidbody2D>();
        if (rigid_body != null) {
            move_in_direction = move_in_direction_as_rigidbody;
        }
        else {
            move_in_direction = move_in_direction_as_transform;
        }
        
    }

    public override void add_child(IChild_of_group compound_object) {
        Contract.Requires(compound_object is ALeg);
        ALeg leg = compound_object as ALeg;
        legs.Add(leg);
        leg.transform.SetParent(transform, false);
    }

    public Type child_type() {
        return typeof(ILeg);
    }


  
    

    public override void shift_center(Vector2 in_shift) {
        foreach (ILeg leg in legs) {
            leg.transform.localPosition += (Vector3)in_shift;
            //leg.optimal_position_standing = (leg.optimal_position_standing + in_shift);
        }
    }
    #endregion

    

    #region ITransporter
    
    public static float belly_friction_multiplier = 0.9f;
    
    public float possible_impulse { get; set; }

    private float calculate_possible_impulse() {
        if (moving_strategy is null) {
            return 0f;
        }
        float impulse = 0;
        foreach (ILeg leg in legs) {
            if (!leg.is_up()) {
                impulse += leg.get_provided_impulse();
            }
        }
        if (moving_strategy.belly_touches_ground()) {
            //impulse *= belly_friction_multiplier;
        }
        return impulse;
    }



    public float rotate_faster_than_move = 100f;

    public float possible_rotation {
        get { 
            //return GetComponent<Turning_element>().rotation_acceleration;
            return possible_impulse * rotate_faster_than_move; 
        }
        set{ Contract.Assert(false, "set possible_impulse instead");}
    }

 

    public Transporter_commands command_batch { get; } = new Transporter_commands();


    private delegate void Move_in_direction(Vector2 moving_direction);
    private Move_in_direction move_in_direction;
        
    public void move_in_direction_as_rigidbody(Vector2 moving_direction) {
        Vector2 delta_movement = (rvi.Time.deltaTime * possible_impulse * moving_direction );
        rigid_body.AddForce(delta_movement*Physics_consts.rigidbody_impulse_multiplier);
    }
    public void move_in_direction_as_transform(Vector2 moving_direction) {
        Vector2 delta_movement = (rvi.Time.deltaTime * possible_impulse * moving_direction );
        moved_body.transform.position += (Vector3)delta_movement;
    }
    
    public void rotate_to_direction(Quaternion face_direction) {

        moved_body.rotation_acceleration = possible_rotation;
        moved_body.target_rotation = face_direction;
        moved_body.rotate_to_desired_direction();
    }
    

    void Update() {
        move_legs();
    }
    
    private void FixedUpdate() {
        execute_commands();
    }

    protected void execute_commands() {
        move_in_direction(command_batch.moving_direction_vector);
        if (moved_body != null) {
            rotate_to_direction(command_batch.face_direction_quaternion);
        }
    }
    #endregion
    
    
    #region Creeping_leg_group itself
    
    /* legs that are enough to be stable if they are on the ground */
    public List<Stable_leg_group> stable_leg_groups = new List<Stable_leg_group>();
    public ALeg dominant_leg;
    public List<ALeg> legs;
    public Turning_element moved_body;
    
    public Moving_strategy moving_strategy {
        get { return _moving_strategy; }
        set {
            _moving_strategy = value;
            possible_impulse = calculate_possible_impulse();
        }
    }
    private Moving_strategy _moving_strategy;
    
    
    private Rigidbody2D rigid_body { get; set; }

    private void guess_moving_strategy() {
        if (stable_leg_groups.Count > 1) {
            moving_strategy = new Stable(legs, this);
        } else
        if (legs.Count > 1 ) {
            moving_strategy = new Grovelling(legs, this);
        } else if (legs.Count == 1) {
            moving_strategy = new Faltering(legs, this);
        }
    }


    private void move_legs() {
        foreach (ILeg leg in legs) {
            if (!(leg.current_action is Creeping_leg_partakes_in_moving)) {
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
        possible_impulse += leg.get_provided_impulse();
    }

    
    private Vector2 get_optimal_position_for(ILeg leg) {
        Vector2 shift_to_moving_direction =
            command_batch.moving_direction_vector * (leg.get_moving_offset_distance() * leg.transform.lossyScale.x);

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

    

    public void OnDrawGizmos() {
        foreach (ILeg leg in legs) {
            if (leg != null) {
                if (Application.isPlaying) {
                    leg.draw_positions();
                    leg.draw_desired_directions();
                }
                leg.draw_directions(Color.white);
            }
        }
    }
    

    #endregion

   

    #region IActor
    public Action current_action { get; set; }
    private Action_runner action_runner { get; set; }
    public void on_lacking_action() {
        // var animator = GetComponent<Animator>();
        // Action meeting_action = null;
        // if (animator != null) {
        //     meeting_action =
        //         Haymaker_with_creeping_leg.create(
        //             GetComponent<Animator>(),
        //             this,
        //             GameObject.FindWithTag("player")?.transform
        //         );
        // }
        // else {
        //     meeting_action =
        //         Keep_distance_from_target.create(
        //             this,
        //             10,
        //             GameObject.FindWithTag("player")?.transform
        //         );
        // }
        
        // Action_sequential_parent.create(
        //     Move_towards_target.create(
        //         this,
        //         reaching_distance(),
        //         GameObject.FindWithTag("player")?.transform
        //     )
        //     //,meeting_action
        //     
        // ).start_as_root(action_runner);
    }
    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }
    #endregion
}

}

