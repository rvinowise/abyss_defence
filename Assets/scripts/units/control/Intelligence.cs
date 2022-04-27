using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.sensors;
using rvinowise.unity.units.parts.transport;
using Baggage = rvinowise.unity.units.parts.Baggage;
using rvinowise.contracts;
using System.Linq;
using rvinowise.unity.management;
using rvinowise.unity.ui.input;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.weapons.guns.common;
using Action = rvinowise.unity.units.parts.actions.Action;

namespace rvinowise.unity.units.control {

/* asks tool controllers what they can do,
 orders them some actions based on this information,
 they do these actions later in the same step */
public abstract class Intelligence :
    MonoBehaviour,
    IRun_actions
{

    public Baggage baggage;
    public ISensory_organ sensory_organ;
    public ITransporter transporter { get; set; }
    public IWeaponry weaponry { get; set; }
    public float last_rotation;

    public Team team;

    public Action_runner action_runner { get; set; } = new Action_runner();


    protected virtual void Awake() {
        sensory_organ = GetComponent<ISensory_organ>();
        transporter = GetComponent<ITransporter>();
        weaponry = GetComponent<IWeaponry>();
        Contract.Requires(
            GetComponentsInChildren<Baggage>().Length <= 1, "which baggage should the Intelligence use?"
        );
        baggage = GetComponentInChildren<Baggage>();
        
    }

    protected virtual void Start() {
        if (team != null) {
            notify_other_units();
        }
        add_actors_to_action_runner();
        action_runner.start_fallback_actions();
    }

    private void add_actors_to_action_runner() {
        var actors = GetComponentsInChildren<IActor>();
        foreach (IActor actor in actors) {
            actor.init_for_runner(action_runner);
            action_runner.add_actor(actor);
        }
    }

    private void notify_other_units() {
        team.units.Add(this);

        foreach (Team enemy_team in team.enemies) {
            foreach (var enemy_unit in enemy_team.units) {
                enemy_unit.consider_enemy(this);
            }
        }
    }



    public virtual void consider_enemy(Intelligence in_enemy) { }

    protected virtual void Update() {
        read_input();
        action_runner.update();
    }

    protected virtual void FixedUpdate() {
        
    }

    

    protected virtual void read_input() { }

    protected void save_last_rotation(Quaternion needed_direction) {
        float angle_difference = transform.rotation.degrees_to(needed_direction).degrees;
        last_rotation = angle_difference;
    }

    public virtual void start_dying(Projectile damaging_projectile) {
        
    }
    
    public virtual void on_root_action_finished() {
        
    }

    public virtual void start_walking(Action in_action) {
        action_runner.mark_action_as_finishing(in_action);
    }

    /*#region IActor
    public Action current_action { get; set; }
    public abstract void on_lacking_action();

    #endregion*/


    public void add_action(Action action) {
        action_runner.add_action(action);
    }
}
}