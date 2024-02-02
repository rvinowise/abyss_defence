using System;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.contracts;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.transport;
using Action = rvinowise.unity.units.parts.actions.Action;

namespace rvinowise.unity.units.parts.limbs.creeping_legs {
public partial class Creeping_leg_group: 
    Children_group
    ,ITransporter
    ,IWeaponry
{
    
    #region Children_group
    public override IEnumerable<IChild_of_group> children  {
        get { return legs as IEnumerable<IChild_of_group>; }
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

    private void init_components() {
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
        _legs.Add(leg);
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
            impulse *= belly_friction_multiplier;
        }
        return impulse;
    }



    private const float rotate_faster_than_move = 100f;

    public float possible_rotation {
        get { 
            return GetComponent<Turning_element>().rotation_acceleration;
            //return possible_impulse * rotate_faster_than_move; 
        }
        set{ Contract.Assert(false, "set possible_impulse instead");}
    }

    public Quaternion direction_quaternion {
        get { return transform.rotation; }
    }

    public transport.Transporter_commands command_batch { get; } = new transport.Transporter_commands();


    private delegate void Move_in_direction(Vector2 moving_direction);
    private Move_in_direction move_in_direction;
        
    public void move_in_direction_as_rigidbody(Vector2 moving_direction) {
        Vector2 delta_movement = (rvi.Time.deltaTime * possible_impulse * moving_direction );
        rigid_body.AddForce(delta_movement*10000f);
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
        execute_commands();
        move_legs();
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

    public strategy.Moving_strategy moving_strategy {
        get { return _moving_strategy; }
        set {
            _moving_strategy = value;
            possible_impulse = calculate_possible_impulse();
        }
    }
    private strategy.Moving_strategy _moving_strategy;
    
    

    public Turning_element moved_body;

    private IReadOnlyList<ALeg> legs {
        get {
            return _legs;
        }
    }
    public ALeg left_hind_leg {
        get { return legs[0];}
    }
    public ALeg left_front_leg {
        get { return legs[1];}
    }
    public ALeg right_hind_leg {
        get { return legs[2];}
    }
    public ALeg right_front_leg {
        get { return legs[3];}
    }
    [SerializeField]
    public List<ALeg> _legs = new List<ALeg>();

    private Rigidbody2D rigid_body { get; set; }

    private void guess_moving_strategy() {
        if (stable_leg_groups.Count > 1) {
            moving_strategy = new strategy.Stable(legs, this);
        } else
        if (legs.Count > 1 ) {
            moving_strategy = new strategy.Grovelling(legs, this);
        } else if (legs.Count == 1) {
            moving_strategy = new strategy.Faltering(legs, this);
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

    #region IWeaponry
    public bool can_reach(Transform target) {
        return transform.distance_to(target.position) < reaching_distance();
    }

    public float reaching_distance() {
        return right_front_leg.get_reaching_distance();
    }

    public void attack(Transform target) {
        ILeg best_leg = get_best_leg_for_hitting(target);
        if (best_leg is Limb2 best_limb2) {
            ensure_leg_raised(best_leg);
            Hitting_with_limb2.create(
                best_limb2,
                this,
                target
            ).start_as_root(action_runner);
        }
    }

    public void ensure_leg_raised(ILeg leg) {
        if (leg.is_up()) {
            return;
        }
        moving_strategy.raise_up(leg);
    }

    public ILeg get_best_leg_for_hitting(Transform target) {
        return right_front_leg;
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
        
        Action_sequential_parent.create(
            Move_towards_target.create(
                this,
                reaching_distance(),
                GameObject.FindWithTag("player")?.transform
            )
            //,meeting_action
            
        ).start_as_root(action_runner);
    }
    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }
    #endregion
}

}

