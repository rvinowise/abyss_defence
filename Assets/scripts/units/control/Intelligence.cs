using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.actions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public struct Target {
    public Intelligence intelligence;
    public Collider2D collider2d;

    public Target(Transform transform) {
        intelligence = transform.GetComponent<Intelligence>();
        collider2d = transform.GetComponent<Collider2D>();
    }
    
    public Vector3 position => intelligence.transform.position;
    public Transform transform => intelligence.transform;
}

public class Intelligence :
    MonoBehaviour
{

    public Baggage baggage;
    
    public ISensory_organ sensory_organ;
    public ITransporter transporter;
    public IAttacker attacker;
    public IDefender defender;
    

    public Team team;
    public bool is_ignored;

    public Action_runner action_runner;
    
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
    }
    
    protected virtual void Start() {
        
        if (team != null) {
            notify_about_appearance();
        }
        action_runner.start_fallback_actions();
        
    }

    private TDevice[] get_highest_devices<TDevice>() {
        //subordinate devices are used by other devices, the intelligence shouldn't use them directly 
        List<TDevice> needed_decives = new List<TDevice>();

        return check_deeper_children(transform, needed_decives).ToArray();
        
        List<TDevice> check_deeper_children(Transform root, List<TDevice> found_devices) {
            var needed_devices = root.GetComponents<TDevice>();
            if (needed_devices.Length > 0) {
                found_devices.AddRange(needed_devices);
                return found_devices;
            }
            foreach (Transform child in root.transform) {
                if (!child.gameObject.activeSelf) {
                    continue;
                }
                check_deeper_children(child, found_devices);
            }
            return found_devices;
        }
    }

    public void init_devices() {
        var attackers = 
            get_highest_devices<IAttacker>();
        if (attackers.Length == 0) {
            attacker = new Empty_attacker();
        } else if (attackers.Length == 1) {
            attacker = attackers.First();
        }
        else {
            attacker = new Compound_attacker(attackers);
        }
        
        var transporters = get_highest_devices<ITransporter>();
        if (transporters.Length == 0) {
            transporter = new Empty_transporter();
        } else if (transporters.Length == 1) {
            transporter = transporters.First();
        }
        else {
            transporter = new Compound_transporter(transporters);
        }
        transporter.set_moved_body(GetComponent<Turning_element>());
        
        var defenders = get_highest_devices<IDefender>();
        if (defenders.Length == 0) {
            defender = new Empty_defender();
        } else if (defenders.Length == 1) {
            defender = defenders.First();
        }
        else {
            defender = new Compound_defender(defenders);
        }
        
        var sensory_organs = get_highest_devices<ISensory_organ>();
        if (sensory_organs.Length == 0) {
            sensory_organ = new Empty_sensory_organ();
        } else if (sensory_organs.Length == 1) {
            sensory_organ = sensory_organs.First();
        }
        else {
            sensory_organ = new Compound_sensory_organ(sensory_organs);
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
        on_destroyed?.Invoke(this);
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

    

    private void OnDestroy() {
        Debug.Log($"LIFETIME: OnDestroy is called for {name}");
        if (team != null) {
            
            if (team.units.Contains(this)) {
                Debug.Log("WARNING: the Intelligence is destroyed, but its team still has it");
            }
            team.remove_unit(this);
        }
    }

}
}