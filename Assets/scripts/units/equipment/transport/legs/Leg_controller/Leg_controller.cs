using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using rvinowise;
using rvinowise.units;
using units.equipment.transport;
using rvinowise.rvi.contracts;
using rvinowise.units.equipment.limbs.legs.strategy;

namespace rvinowise.units.equipment.limbs.legs {
public partial class Leg_controller: 
    Equipment_controller
    ,ITransporter
{
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

    
    
    /* Equipment_controller interface */
    public override IEnumerable<Child> tools  {
        get { return legs; }
    }

    public override IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
        return new Leg_controller(dst_host);
    }
    
    protected override void init_components() {
        rigid_body = game_object.GetComponent<Rigidbody2D>();
        Contract.Requires(rigid_body != null);
    }

    public override void add_tool(Child child) {
        Contract.Requires(child is Leg);
        Leg leg = child as Leg;
        legs.Add(leg);
        leg.host = transform;
    }


    public override void update() {
        destroy_invalid_legs(); //debug
        execute_commands();
        move_legs();
    }

    /* ITransporter interface */

    private void execute_commands() {
        transport.Command_batch command_batch = get_combined_commands();
        move_in_direction(command_batch.moving_direction_vector);
        rotate_to_direction(command_batch.face_direction_degrees);
    }
    
    public void move_in_direction(Vector2 moving_direction) {
        Vector2 delta_movement = (moving_direction * get_possible_impulse() * rvi.Time.deltaTime);
        //transform.position += (Vector3)delta_movement;
        rigid_body.MovePosition((Vector2)transform.position + delta_movement);
    }
    public void move_in_direction(float direction) {
        throw new NotImplementedException();
    }
    
    public void rotate_to_direction(float face_direction) {
        transform.rotate_to(
            face_direction, 
            get_possible_rotation() * rvi.Time.deltaTime
        );
    }
    
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


    static float belly_friction_multiplier = 0.9f;
    public float get_possible_rotation() {
        if (moving_strategy is null) {
            return 0f;
        }
        float impulse = 0;
        foreach (Leg leg in legs) {
            if (!leg.is_up) {
                impulse += leg.provided_impulse * 100;
            }
        }
        if (moving_strategy.belly_touches_ground()) {
            impulse *= belly_friction_multiplier;
        }
        return impulse;
    }


    public Command_batch command_batch {
        get { return _command_batch; }
    }
    private Command_batch _command_batch = new Command_batch();
    
    
    
    /* Leg_controller itself */
    
    /* the distance that the optimal_position is moved in during walking */
    public float moving_offset_distance = 0.3f;

    private Rigidbody2D rigid_body { get; set; }
    

    public Leg_controller(User_of_equipment in_user):base(in_user) {
        
        
    }

    /* i need this function only for a generic adder (constructors can't have parameters there)*/
    public Leg_controller():base() {
        
    }

    
    
    public void guess_moving_strategy() {
        if (legs.Count >=2 ) {
            moving_strategy = new strategy.Grovelling(legs);
        } else if (legs.Count == 1) {
            moving_strategy = new strategy.Faltering(legs);
        }
    }


    void Awake()
    {

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
            leg.attach_to_attachment_points();
        }
    }

    private void determine_optimal_directions_for(Leg leg) {
        Vector2 shift_to_moving_direction =
            command_batch.moving_direction_vector *
            moving_offset_distance;

        leg.set_optimal_position(
            (Vector2)leg.host.TransformPoint(leg.optimal_relative_position_standing) + 
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
            leg.debug.draw_desired_directions(Color.green, 0.1f);
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

