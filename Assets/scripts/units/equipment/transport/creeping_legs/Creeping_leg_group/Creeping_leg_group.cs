using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using rvinowise.rvi.contracts;
using geometry2d;
using rvinowise.units.parts.transport;
using static geometry2d.Directions;

namespace rvinowise.units.parts.limbs.legs {
public partial class Creeping_leg_group: 
    Children_group
    ,ITransporter
{
    
    /* Children_group interface */
    public override IEnumerable<Child> children  {
        get { return legs; }
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

    public override void add_child(Child child) {
        Contract.Requires(child is Leg);
        Leg leg = child as Leg;
        legs.Add(leg);
        leg.parent = transform;
    }


    

    

    /* ITransporter interface */
    
    private static float belly_friction_multiplier = 0.9f;
    public float get_possible_impulse() {
        if (moving_strategy is null) {
            return 0f;
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
        return impulse;
    }


    private const float rotate_faster_than_move = 3000f;

    public float get_possible_rotation() {
        if (moving_strategy is null) {
            return 0f;
        }
        float impulse = 0;
        foreach (Leg leg in legs) {
            if (!leg.is_up) {
                impulse += leg.provided_impulse * rotate_faster_than_move;
            }
        }
        if (moving_strategy.belly_touches_ground()) {
            impulse *= belly_friction_multiplier;
        }
        return impulse;
    }

    public Quaternion direction_quaternion {
        get { return transform.rotation; }
    }

    parts.Command_batch IExecute_commands.command_batch => command_batch;
    public transport.Command_batch command_batch { get; } = new transport.Command_batch();


    private delegate void Move_in_direction(Vector2 moving_direction);
    private Move_in_direction move_in_direction;
        
    public void move_in_direction_as_rigidbody(Vector2 moving_direction) {
        Vector2 delta_movement = (moving_direction * get_possible_impulse() * rvi.Time.deltaTime);
        rigid_body.MovePosition((Vector2)transform.position + delta_movement);
    }
    public void move_in_direction_as_transform(Vector2 moving_direction) {
        Vector2 delta_movement = (moving_direction * get_possible_impulse() * rvi.Time.deltaTime);
        transform.position += (Vector3)delta_movement;
    }
    
    public void rotate_to_direction(Quaternion face_direction) {
        transform.rotate_to(
            face_direction, 
            get_possible_rotation() * rvi.Time.deltaTime
        );
        /*transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            face_direction, 
            get_possible_rotation() * rvi.Time.deltaTime);*/
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
    
    public strategy.Moving_strategy moving_strategy;
    
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
        get { return legs[0];}
        private set { legs[0] = value; }
    }
    public Leg right_front_leg {
        get { return legs[1];}
        private set { legs[1] = value; }
    }
    public Leg left_hind_leg {
        get { return legs[2];}
        private set { legs[2] = value; }
    }
    public Leg right_hind_leg {
        get { return legs[3];}
        private set { legs[3] = value; }
    }
    private List<Leg> _legs = new List<Leg>();
    
    /* the distance that the optimal_position is moved in during walking */
    public float moving_offset_distance = 0.3f;

    private Rigidbody2D rigid_body { get; set; }
    

    public Creeping_leg_group(IChildren_groups_host in_host):base(in_host) {
        
        
    }

    /* i need this function only for a generic adder (constructors can't have parameters there)*/
    public Creeping_leg_group():base() {
        
    }

    
    
    public void guess_moving_strategy() {
        if (legs.Count >=2 ) {
            moving_strategy = new strategy.Grovelling(legs);
        } else if (legs.Count == 1) {
            moving_strategy = new strategy.Faltering(legs);
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
            if (leg.is_up) {
                move_in_the_air(leg);
            } else {
                moving_strategy.move_on_the_ground(leg);
            }
        }
    }
    
    void move_in_the_air(Leg leg) {
        determine_optimal_directions_for(leg);
        if (leg.has_reached_aim()) {
            leg.put_down();
        } else {
            leg.move_segments_towards_desired_direction();
        }
    }

    private void determine_optimal_directions_for(Leg leg) {
        Vector2 shift_to_moving_direction =
            command_batch.moving_direction_vector *
            moving_offset_distance;

        leg.set_optimal_position(
            (Vector2)leg.parent.TransformPoint(leg.optimal_relative_position_standing) + 
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


    public void on_draw_gizmos() {
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

