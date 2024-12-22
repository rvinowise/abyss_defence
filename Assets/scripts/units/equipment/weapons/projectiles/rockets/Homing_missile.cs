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
    
    public Team team;
    public Transform target;

    public Action_runner action_runner;
    

    public virtual void Awake() {

        init_devices();
        
        Contract.Requires(
            GetComponentsInChildren<Baggage>().Length <= 1, "which baggage should the Intelligence use?"
        );

        register_actor_parts();
        
    }

    private void Start() {
        fly_towards_target();
    }

   

    public void init_devices() {
        
        transporter = GetComponentInChildren<IActor_transporter>();
        
        transporter.set_moved_body(this.GetComponent<Turning_element>());
        transporter.init_for_runner(action_runner);
        
    }

    public void register_actor_parts() {
        action_runner.start_fallback_actions();
    }
    

    public void fly_towards_target() {
        if (target==null) {
            return;
        }
        Move_towards_target.create(
            transporter,
            0,
            target
        ).start_as_root(action_runner);
    }

    private void FixedUpdate() {
        action_runner.update();
    }
}
}