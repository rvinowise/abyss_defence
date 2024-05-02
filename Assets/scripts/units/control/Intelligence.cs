using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.actions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {


public class Intelligence :
    MonoBehaviour
{

    public Baggage baggage;
    
    public IActor_sensory_organ sensory_organ;
    public IActor_transporter transporter;
    public IActor_attacker attacker;
    public IActor_defender defender;
    
    public float last_rotation;

    public Team team;

    public Action_runner action_runner { get; } = new Action_runner();
    
    public delegate void Evend_handler(Intelligence unit);
    public event Evend_handler on_destroyed;

    public virtual void Awake() {

        init_devices();
        
        Contract.Requires(
            GetComponentsInChildren<Baggage>().Length <= 1, "which baggage should the Intelligence use?"
        );
        baggage = GetComponentInChildren<Baggage>();
        
        if (team != null) {
            add_to_team();
        }
        add_actors_to_action_runner();
    }
    
    protected virtual void Start() {
        
        if (team != null) {
            notify_about_appearance();
        }
        action_runner.start_fallback_actions();
        
    }


    public void init_devices() {
        var attackers = GetComponentsInChildren<IActor_attacker>();
        if (attackers.Length == 0) {
            attacker = new Empty_attacker();
        } else if (attackers.Length == 1) {
            attacker = attackers.First();
        }
        else {
            attacker = new Compound_attacker(attackers);
        }
        
        var transporters = GetComponentsInChildren<IActor_transporter>();
        if (transporters.Length == 0) {
            transporter = new Empty_transporter();
        } else if (transporters.Length == 1) {
            transporter = transporters.First();
        }
        else {
            transporter = new Compound_transporter(transporters);
        }
        transporter.set_moved_body(this.GetComponent<Turning_element>());
        transporter.init_for_runner(action_runner);
        
        var defenders = GetComponentsInChildren<IActor_defender>();
        if (defenders.Length == 0) {
            defender = new Empty_defender();
        } else if (attackers.Length == 1) {
            defender = defenders.First();
        }
        else {
            defender = new Compound_defender(defenders);
        }
        defender.init_for_runner(action_runner);
        
        var sensory_organs = GetComponentsInChildren<IActor_sensory_organ>();
        if (sensory_organs.Length == 0) {
            sensory_organ = new Empty_sensory_organ();
        } else if (attackers.Length == 1) {
            sensory_organ = sensory_organs.First();
        }
        else {
            sensory_organ = new Compound_sensory_organ(sensory_organs);
        }
        sensory_organ.init_for_runner(action_runner);
    }

    public void add_actors_to_action_runner() {
        var actors = GetComponentsInChildren<IActor>();
        foreach (IActor actor in actors) {
            action_runner.add_actor(actor);
        }
        var intelligent_children = GetComponentsInChildren<IRunning_actions>();
        foreach (IRunning_actions intelligent_child in intelligent_children) {
            intelligent_child.init_for_runner(action_runner);
        }
    }

    private void add_to_team() {
        team.add_unit(this);
    }
    private void notify_about_appearance() {
        foreach (Team enemy_team in team.enemy_teams) {
            foreach (var enemy_unit in enemy_team.units) {
                if (enemy_unit == null) {
                    Debug.LogError("enemy_unit is null");
                }
                enemy_unit.on_enemy_appeared(this);
            }
        }
        foreach (Team ally_team in team.ally_teams) {
            foreach (var ally_unit in ally_team.units) {
                ally_unit.on_ally_appeared(this);
            }
        }
        foreach (var friendly_unit in team.units) {
            friendly_unit.on_friend_appeared(this);
        }
    }

    public void notify_about_destruction() {
        on_destroyed?.Invoke(this);

        if (team != null) {
            team.remove_unit(this);
            foreach (Team enemy_team in team.enemy_teams) {
                foreach (var enemy_unit in enemy_team.units) {
                    if (enemy_unit == null) {
                        Debug.LogError("enemy_unit is null");
                    }
                    enemy_unit.on_enemy_disappeared(this);
                }
            }
            foreach (Team ally_team in team.ally_teams) {
                foreach (var ally_unit in ally_team.units) {
                    ally_unit.on_ally_disappeared(this);
                }
            }
            foreach (var friendly_unit in team.units) {
                friendly_unit.on_friend_disappeared(this);
            }
        }
    }
    

    public virtual void on_enemy_appeared(Intelligence in_enemy) { }

    public virtual void on_ally_appeared(Intelligence in_ally) {
        on_friend_appeared(in_ally);
    }
    public virtual void on_friend_appeared(Intelligence in_friend) { }
    
    public virtual void on_enemy_disappeared(Intelligence in_enemy) { }

    public virtual void on_ally_disappeared(Intelligence in_ally) {
        on_friend_disappeared(in_ally);
    }
    public virtual void on_friend_disappeared(Intelligence in_friend) { }

    protected void save_last_rotation(Quaternion needed_direction) {
        float angle_difference = transform.rotation.degrees_to(needed_direction).degrees;
        last_rotation = angle_difference;
    }

    private void OnDestroy() {
        Debug.Log($"LIFETIME: OnDestroy is called for {name}");
        if (team != null) {
            
            if (team.units.Contains(this)) {
                Debug.Log("WARNING: the Intelligence is destroyed, but its team still has it");
            }
            team.remove_unit(this);
        }
    }


    public void add_action(Action action) {
        action_runner.add_action(action);
    }
}
}