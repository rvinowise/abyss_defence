using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.units.control;

//using static UnityEngine.Input;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.limbs.creeping_legs;
using rvinowise.unity.units.parts.transport;
using rvinowise.unity.units.parts.weapons.guns.common;
using Action = rvinowise.unity.units.parts.actions.Action;
using Input = rvinowise.unity.ui.input.Player_input;

namespace rvinowise.unity.units.control.spider {

public class Computer_spider: 
    Strategic_intelligence
{

    private Creeping_leg_group leg_group;
    private bool is_dying = false;

    protected override void Start() {
        base.Start();
        init_components();
        start_walking(null);
    }

    private void init_components() {
        leg_group = transporter as Creeping_leg_group;
    }
    protected override void Update() {
        base.Update();
        
    }
    




    private bool is_attacking() {
        return ((Creeping_leg_group) weaponry).right_front_leg.current_action?.get_root_action() is Hitting_with_limb2;
    }


    public override void start_dying(Projectile damaging_projectile) {
        is_dying = true;
        transporter.command_batch.moving_direction_vector = damaging_projectile.last_physics.velocity.normalized;
        transporter.command_batch.face_direction_quaternion = damaging_projectile.last_physics.velocity.to_quaternion();

    }

    public void on_reached_target(Action moving) {
        Attacking_with_creeping_legs.create(
            this,
            leg_group,
            unit_commands.attack_target
        ).start_as_root(action_runner);
    }

    public override void start_walking(Action action) {
        Move_towards_target.create(
            this,
            leg_group,
            leg_group.reaching_distance(),
            unit_commands.attack_target
        ).add_finish_notifyer(on_reached_target)
        .start_as_root(action_runner);
    }

    /*public override void on_lacking_action() {
        
    }*/
}

}