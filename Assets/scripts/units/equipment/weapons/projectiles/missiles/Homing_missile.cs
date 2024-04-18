using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.actions;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Homing_missile :
    MonoBehaviour
{

    public IActor_transporter transporter;
    public GameObject transporter_object;
    
    
    public float last_rotation;

    public Team team;
    public Transform target;

    public Explosion explosion_prefab;

    public Action_runner action_runner { get; } = new Action_runner();
    
    public delegate void Evend_handler(Intelligence unit);
    public event Evend_handler on_destroyed;

    public virtual void Awake() {

        init_devices();
        
        Contract.Requires(
            GetComponentsInChildren<Baggage>().Length <= 1, "which baggage should the Intelligence use?"
        );

        register_actor_parts();
        
    }

    private void Start() {
        fly_towards_target(target);
    }

    public void init_devices_from_inspector() {
        if ((transporter_object != null)&&(transporter_object.GetComponents<IActor_transporter>().Length > 0)) {
            transporter = transporter_object.GetComponents<IActor_transporter>().First();
        }
        else {
            transporter = new Empty_transporter();
        }
    }

    public void init_devices() {
        
        transporter = GetComponentInChildren<IActor_transporter>();
        
        transporter.set_moved_body(this.GetComponent<Turning_element>());
        transporter.init_for_runner(action_runner);
        
    }

    public void register_actor_parts() {
        add_actors_to_action_runner();
        action_runner.start_fallback_actions();
    }
    
    private void add_actors_to_action_runner() {
        var actors = GetComponentsInChildren<IActor>();
        foreach (IActor actor in actors) {
            action_runner.add_actor(actor);
        }
        var intelligent_children = GetComponentsInChildren<IRunning_actions>();
        foreach (IRunning_actions intelligent_child in intelligent_children) {
            intelligent_child.init_for_runner(action_runner);
        }
    }


    public void fly_towards_target(Transform in_target) {
        this.target = in_target;
        Move_towards_target.create(
            transporter,
            0,
            in_target
        ).start_as_root(action_runner);
    }

    private void FixedUpdate() {
        action_runner.update();
    }
}
}