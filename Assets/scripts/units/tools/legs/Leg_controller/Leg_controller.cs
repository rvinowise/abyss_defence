using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEngine;
using rvinowise;

namespace rvinowise.units.equipment.limbs {

/* Leg controller */
public partial class Leg_controller: 
    Equipment_controller
   ,ITransporter
{
    public strategy.Moving_strategy moving_strategy;

    
    
    public List<Leg> legs {
        set {
            _legs = value;
            init_moving_strategy();
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

    
    
    /* Tool_controller interface */
    public override IEnumerable<Tool> tools  {
        get { return legs; }
    }

    
    
    public override IEquipment_controller copy_empty_into(User_of_equipment dst_host) {
        return new Leg_controller(dst_host);
    }

    public override void add_tool(Tool tool) {
        Contract.Requires(tool is Leg);
        Leg leg = tool as Leg;
        legs.Add(leg);
        leg.host = transform;
    }

    public override void init() {
        init_moving_strategy();
    }

    public override void update() {
        destroy_invalid_legs(); //debug
        move_legs();
    }

    
    /* ITransporter interface */
    public ITransporter get_copy() {
        Leg_controller new_leg_controller = new Leg_controller();
        return new_leg_controller; //TODO
    }
 
    public float get_possible_impulse() {
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

    public float get_possible_rotation() {
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

    
    /* Leg_controller itself */
    public Leg_controller(User_of_equipment in_user):base() {
        userOfEquipment = in_user;
    }
    
    /* i need this function only for a generic adder (constructors can't have parameters there)*/
    public Leg_controller():base()  {
    }
    
    public void init_moving_strategy() {
        moving_strategy = get_appropriate_moving_strategy();
    }
    private strategy.Moving_strategy get_appropriate_moving_strategy() {
        if (legs.Count >= 4) {
            return new strategy.Stable(legs);
        } else if (legs.Count >=2 ) {
            return new strategy.Grovelling(legs);
        } else if (legs.Count == 1) {
            return new strategy.Faltering(legs);
        }
        return null;
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
        if (leg.has_reached_aim()) {
            leg.put_down();
        } else {
            leg.move_segments_towards_desired_direction();
            leg.attach_to_attachment_points();
        }
    }

    

    

    bool can_move() {
        foreach (Leg leg in legs) {
            if (!leg.is_up) {
                return true;
            }
        }
        return false;
    }

    static float belly_friction_multiplier = 0.9f;


    

    private void OnDrawGizmos() {
        foreach (var leg in legs) {
            leg.debug.draw_positions();
        }
    }
}

enum Impulse {
    Moving,
    Rotation
}

}
