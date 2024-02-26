using System;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.actions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

/* asks tool controllers what they can do,
 orders them some actions based on this information,
 they do these actions later in the same step */
public class Intelligence :
    MonoBehaviour
{

    public Baggage baggage;
    
    public ISensory_organ sensory_organ;
    public GameObject sensory_organ_object;
    
    public ITransporter transporter;
    public GameObject transporter_object;
    
    public IAttacker attacker;
    public GameObject weaponry_object;
    
    public IDefender defender;
    public GameObject defender_object;
    
    public float last_rotation;

    public Team team;

    public Action_runner action_runner { get; } = new Action_runner();
    
    public delegate void EvendHandler(Intelligence unit);
    public event EvendHandler on_destroyed;

    protected virtual void Awake() {
        sensory_organ = sensory_organ_object.GetComponent<ISensory_organ>();
        transporter = transporter_object.GetComponent<ITransporter>();

        if (weaponry_object.GetComponents<IAttacker>().Length > 0) {
            attacker = weaponry_object.GetComponents<IAttacker>().First();
        }
        else {
            attacker = new Empty_attacker();
        }

        if (weaponry_object.GetComponents<IDefender>().Length > 0) {
            defender = weaponry_object.GetComponents<IDefender>().First();
        }
        else {
            defender = new Empty_defender();
        }
        
        Contract.Requires(
            GetComponentsInChildren<Baggage>().Length <= 1, "which baggage should the Intelligence use?"
        );
        baggage = GetComponentInChildren<Baggage>();
        
        if (team != null) {
            notify_about_appearance();
        }
    }

    protected virtual void Start() {
        
        add_actors_to_action_runner();
        action_runner.start_fallback_actions();
    }

    private void add_actors_to_action_runner() {
        var actors = GetComponentsInChildren<IActor>();
        foreach (IActor actor in actors) {
            //actor.init_for_runner(action_runner);
            action_runner.add_actor(actor);
        }
        var intelligent_children = GetComponentsInChildren<IRunning_actions>();
        foreach (IRunning_actions intelligent_child in intelligent_children) {
            intelligent_child.init_for_runner(action_runner);
        }
    }

    
    public void notify_about_appearance() {
        team.add_unit(this);
        foreach (Team enemy_team in team.enemy_teams) {
            foreach (var enemy_unit in enemy_team.units) {
                if (enemy_unit == null) {
                    Debug.LogError("enemy_unit is null");
                }
                enemy_unit.consider_enemy(this);
            }
        }
        foreach (Team ally_team in team.ally_teams) {
            foreach (var ally_unit in ally_team.units) {
                ally_unit.consider_ally(this);
            }
        }
        foreach (var friendly_unit in team.units) {
            friendly_unit.consider_friend(this);
        }
    }

    public void notify_about_destruction() {
        team.remove_unit(this);
        on_destroyed?.Invoke(this);
    }
    

    public virtual void consider_enemy(Intelligence in_enemy) { }

    public virtual void consider_ally(Intelligence in_ally) {
        consider_friend(in_ally);
    }
    public virtual void consider_friend(Intelligence in_friend) { }

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

    private void OnDestroy() {
        Contract.Assert(!team.units.Contains(this), "the Intelligence is destroyed, but its team still has it");
    }


    public void add_action(Action action) {
        action_runner.add_action(action);
    }
}
}