using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using UnityEngine;
using rvinowise;

namespace units.limbs {

/* Leg controller */
public partial class Leg_controller: 
    Tool_controller
   ,units.IMover_in_space
{
    /* parameters for editor */
    public Sprite sprite_femur;// = Resources.Load<Sprite>("sprites/basic_spider/femur.png");
    public Sprite sprite_tibia;// = Resources.Load<Sprite>("sprites/basic_spider/tibia.png");
    public bool needs_initialization = true;

    strategy.Moving_strategy moving_strategy;

   
    
    internal List<Leg> legs {
        set {
            _legs = value;
            init_moving_strategy();
        }
        get {
            return _legs;
        }
    }
    internal Leg left_front_leg {
        get { return legs[0];}
        private set { legs[0] = value; }
    }
    internal Leg right_front_leg {
        get { return legs[1];}
        private set { legs[1] = value; }
    }
    internal Leg left_hind_leg {
        get { return legs[2];}
        private set { legs[2] = value; }
    }
    internal Leg right_hind_leg {
        get { return legs[3];}
        private set { legs[3] = value; }
    }
    private List<Leg> _legs = new List<Leg>();

    /* Tool_controller interface */
    public override IEnumerable<Tool> tools  {
        get { return legs; }
    }
    public override void init() {
        init_moving_strategy();
    }

    public override void add_tool(Tool tool) {
        Contract.Requires(tool is Leg);
        Leg leg = tool as Leg;
        legs.Add(leg);
        leg.host = transform;
    }

 


    void init_moving_strategy() {
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
        if (needs_initialization) {
            Legs_initializer.init_for_spider(this);
            needs_initialization = false;
        }
        if (legs == null) {
            this.enabled = false;
        }
    }

    void Update() {
        destroy_invalid_legs(); //debug
        move_legs();
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
