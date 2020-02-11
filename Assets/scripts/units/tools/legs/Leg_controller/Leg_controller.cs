using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace units {
namespace limbs {

/* Leg controller */
public class Leg_controller: Tool_controller
{
    /* parameters for editor */
    public Sprite sprite_femur;
    public Sprite sprite_tibia;

    /* legs that are enough to be stable if they are on the ground */
    public List<Stable_leg_group> stable_leg_groups;
    
    internal List<Leg> legs {
        set {
            _legs = value;
            left_front_leg = value[0];
            right_front_leg = value[1];
            left_hind_leg = value[2];
            right_hind_leg = value[3];
        }
        get {
            return _legs;
        }
    }
    internal Leg left_front_leg;
    internal Leg right_front_leg;
    internal Leg left_hind_leg;
    internal Leg right_hind_leg;
    private List<Leg> _legs;

    /* Tool_controller interface */
    public override IEnumerable<Tool> tools {
        get {
            return legs;
        }
    }

    public override void add_tool(Tool tool) {
        Contract.Requires(tool is Leg);
        legs.Add((Leg)tool);
    }


    void Awake()
    {
        Legs_initializer.init_for_spider(this);
    }

    void Update()
    {
        move_legs();
    }

    private void move_legs() {
        foreach (Leg leg in legs) {
            if (leg.is_up) {
                move_in_the_air(leg);
            } else {
                move_on_the_ground(leg);
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

    void move_on_the_ground(Leg leg) {
        bool can_hold = leg.hold_onto_ground();
        if (
            (leg.is_twisted_badly())||
            (!can_hold)
            ) 
        {
            leg.debug.draw_lines(Color.red);
            leg.raise_up();
            leg.attach_to_attachment_points();
        }
        if (leg.is_twisted_uncomfortably()) {
            if (is_standing_stable_without(leg)) {
                leg.raise_up();
                leg.attach_to_attachment_points();
            }
        }
    }

    
    /* all the legs from at least one stable group will be on the ground
    if the parameter leg is up */
    private bool is_standing_stable_without(Leg leg)
    {
        Contract.Requires(!leg.is_up);
        foreach (Stable_leg_group stable_leg_group in stable_leg_groups) {
            if (stable_leg_group.contains(leg)) {
               continue; 
            }
            if (stable_leg_group.all_down()) {
                return true;
            }
        }
        return false;
    }

    /* at least one leg will be on he ground if the parameter leg is raised up  */
    bool can_move_without(Leg in_leg) {
        Contract.Requires(!in_leg.is_up);
        foreach (Leg leg in legs) {
            if (leg == in_leg) {
               continue; 
            }
            if (!leg.is_up) {
                return true;
            }
        }
        return false;
    }

    bool can_move() {
        foreach (Leg leg in legs) {
            if (!leg.is_up) {
                return true;
            }
        }
        return false;
    }

    static float belly_friction = 2f;

    bool belly_touches_ground() {
        foreach (Stable_leg_group stable_leg_group in stable_leg_groups) {
            if (stable_leg_group.all_down()) {
                return false;
            }
        }
        return true;
    }

    public float get_possible_impulse() {
        float impulse = 0;
        foreach (Leg leg in legs) {
            if (!leg.is_up) {
                impulse += leg.provided_impulse;
            }
        }
        if (belly_touches_ground()) {
            impulse /= belly_friction;
        }
        return impulse;
    }

}

enum Impulse {
    Moving,
    Rotation
}

}
}