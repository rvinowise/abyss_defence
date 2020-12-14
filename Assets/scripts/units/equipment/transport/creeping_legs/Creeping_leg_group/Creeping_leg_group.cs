using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.contracts;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.transport;
using UnityEngine.Assertions;
using static rvinowise.unity.geometry2d.Directions;

namespace rvinowise.unity.units.parts.limbs.creeping_legs {
public partial class Creeping_leg_group: 
    Children_group
    ,ITransporter
{
    
    /* Children_group interface */
    public override IEnumerable<ICompound_object> children  {
        get { return legs; }
    }

    protected virtual void Awake() {
        base.Awake();
        
    }

    protected virtual void Start() {
        base.Start();

        var test = GetComponents<Turning_element>();
        Contract.Requires(GetComponents<Turning_element>().Length <= 1,
            "only one component with Turning_element is enough");
        turning_element = GetComponent<Turning_element>();
    }

    protected override void init_components() {
        rigid_body = game_object.GetComponent<Rigidbody2D>();
        if (rigid_body != null) {
            move_in_direction = move_in_direction_as_rigidbody;
        }
        else {
            move_in_direction = move_in_direction_as_transform;
        }
    }

    public override void add_child(ICompound_object compound_object) {
        Contract.Requires(compound_object is Leg);
        Leg leg = compound_object as Leg;
        legs.Add(leg);
        leg.transform.SetParent(transform, false);
    }
    public override void hide_children_from_copying() {
        children_stashed_from_copying = legs.Where(leg => leg != null) as IEnumerable<ICompound_object> ;
        legs = new List<Leg>();
    }
    

    public override void shift_center(Vector2 in_shift) {
        foreach (Leg leg in legs) {
            leg.main_object.transform.localPosition += (Vector3)in_shift;
            //leg.optimal_position_standing = (leg.optimal_position_standing + in_shift);
        }
    }
    

    

    /* ITransporter interface */
    
    public static float belly_friction_multiplier = 0.9f;
    
    public float possible_impulse { get; set; }

    private void init_possible_impulse() {
        if (moving_strategy is null) {
            possible_impulse = 0f;
        }
        float impulse = 0;
        foreach (Leg leg in legs) {
            if (!leg.is_up) {
                impulse += leg.provided_impulse;
            }
        }
        if (moving_strategy.belly_touches_ground()) {
            impulse *= belly_friction_multiplier;
        }
        possible_impulse = impulse;
    }



    private const float rotate_faster_than_move = 200f;

    public float possible_rotation {
        get { return possible_impulse * rotate_faster_than_move; }
        set{ Contract.Assert(false, "set possible_impulse instead");}
    }

    public Quaternion direction_quaternion {
        get { return transform.rotation; }
    }

    parts.Command_batch IExecute_commands.command_batch => command_batch;
    public transport.Command_batch command_batch { get; } = new transport.Command_batch();


    private delegate void Move_in_direction(Vector2 moving_direction);
    private Move_in_direction move_in_direction;
        
    public void move_in_direction_as_rigidbody(Vector2 moving_direction) {
        Vector2 delta_movement = (moving_direction * possible_impulse * rvi.Time.deltaTime);
        //rigid_body.MovePosition((Vector2)transform.position + delta_movement);
        rigid_body.AddForce(delta_movement*10000f);
    }
    public void move_in_direction_as_transform(Vector2 moving_direction) {
        Vector2 delta_movement = (moving_direction * possible_impulse * rvi.Time.deltaTime);
        transform.position += (Vector3)delta_movement;
    }
    
    public void rotate_to_direction(Quaternion face_direction) {
        /*transform.rotate_to(
            face_direction, 
            get_possible_rotation() * rvi.Time.deltaTime
        );*/
        turning_element.rotation_acceleration = possible_rotation;
        turning_element.target_quaternion = face_direction;
        turning_element.rotate_to_desired_direction();
    }
    
    
    public void update() {
        destroy_invalid_legs(); //debug_limb
        execute_commands();
        move_legs();
    }
    
    protected void execute_commands() {
        move_in_direction(command_batch.moving_direction_vector);
        rotate_to_direction(command_batch.face_direction_quaternion);
    }
    
    
    
    /* Creeping_leg_group itself */
    
    /* legs that are enough to be stable if they are on the ground */
    public IList<Stable_leg_group> stable_leg_groups = new List<Stable_leg_group>();

    public strategy.Moving_strategy moving_strategy {
        get { return _moving_strategy; }
        set {
            _moving_strategy = value;
            init_possible_impulse();
        }
    }
    private strategy.Moving_strategy _moving_strategy;
    
    

    private Turning_element turning_element;
    
    public List<Leg> legs {
        set {
            _legs = value;
            guess_moving_strategy();
        }
        get {
            return _legs;
        }
    }
    public Leg left_front_leg {
        get { return legs[1];}
        set { legs[1] = value; }
    }
    public Leg right_front_leg {
        get { return legs[3];}
        set { legs[3] = value; }
    }
    public Leg left_hind_leg {
        get { return legs[0];}
        set { legs[0] = value; }
    }
    public Leg right_hind_leg {
        get { return legs[2];}
        set { legs[2] = value; }
    }
    [SerializeField]
    public List<Leg> _legs = new List<Leg>();
    
    /* the distance that the optimal_position is moved in during walking */
    public float moving_offset_distance = 0.3f;

    private Rigidbody2D rigid_body { get; set; }
    

    

    
    
    public void guess_moving_strategy() {
        if (this.stable_leg_groups.Count > 1) {
            moving_strategy = new strategy.Stable(legs, this);
        } else
        if (legs.Count > 1 ) {
            moving_strategy = new strategy.Grovelling(legs, this);
        } else if (legs.Count == 1) {
            moving_strategy = new strategy.Faltering(legs, this);
        }
    }



    private void destroy_invalid_legs() {
        for(int i_leg = 0; i_leg < legs.Count; i_leg++) {
            Leg leg = legs[i_leg];
            if (!leg.is_valid()) {
                legs.RemoveAt(i_leg);
                Deleter.Destroy(leg);
            }
        }
    }

    private void move_legs() {
        foreach (Leg leg in legs) {
            determine_optimal_position_for(leg);
            if (leg.is_up) {
                move_in_the_air(leg);
            } else {
                moving_strategy.move_on_the_ground(leg);
            }
        }
    }
    
    private void move_in_the_air(Leg leg) {
        leg.set_desired_directions_by_position();
        if (leg.has_reached_aim()) {
            put_down(leg);
        } else {
            leg.move_segments_towards_desired_direction();
        }
    }

    private void put_down(Leg leg) {
        Contract.Requires(leg.is_up);
        leg.put_down();
        possible_impulse += leg.provided_impulse;
    }

    
    private void determine_optimal_position_for(Leg leg) {
        Vector2 shift_to_moving_direction =
            command_batch.moving_direction_vector *
            moving_offset_distance * leg.transform.lossyScale.x;

        leg.set_desired_position(
           leg.optimal_position_standing + 
            shift_to_moving_direction
        );
    }

    


    bool can_move() {
        foreach (Leg leg in legs) {
            if (!leg.is_up) {
                return true;
            }
        }
        return false;
    }

    

    public override void on_draw_gizmos() {
        foreach (Leg leg in legs) {
            leg.debug.draw_positions();
            leg.debug.draw_desired_directions(0.1f);
        }
        //draw_moving_direction();
    }

    /*private void draw_moving_direction() {
        UnityEngine.Debug.DrawLine(
            this.user_of_equipment.transform.position, 
            (Vector2)this.user_of_equipment.transform.position +
            control.moving_direction_vector,
            Color.green,
            1f
        );
    }*/
}

}

