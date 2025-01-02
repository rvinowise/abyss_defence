


using System;
using System.Collections.Generic;
using UnityEngine;

namespace rvinowise.unity.actions {

public class Actor: MonoBehaviour {
    
    public Action current_action;
    public Action_runner action_runner;
    public List<IActing_role> roles = new List<IActing_role>();

    public void on_lacking_action() {
        foreach (var role in roles) {
            role.on_lacking_action();
        }
    }

    private void Awake() {
        //action_runner = transform.root.GetComponentInChildren<Action_runner>(); //it can be attached to a combining circle slot
    }
}






}