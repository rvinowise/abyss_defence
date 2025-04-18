#define RVI_DEBUG


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using rvinowise.contracts;
using UnityEditor;
using UnityEngine;


namespace rvinowise.unity.actions {

public class Action_runner: MonoBehaviour {
    private readonly List<Actor> actors = new List<Actor>();

    private readonly List<Action> current_actions = new List<Action>();
    private readonly ISet<Action> finishing_actions = new HashSet<Action>();
    private readonly List<Action> starting_actions = new List<Action>();

    #if RVI_DEBUG
    public int number = 0;
    #endif

    public IReadOnlyList<Actor> get_actors() {
        return actors;
    }
    
    private void Awake() {
        init_actors();
    }
    
    public void init_actors() {
        foreach (Actor actor in GetComponentsInChildren<Actor>()) {
            if (
                (!actors.Contains(actor))
                &&
                (actor.action_runner == this)
            ) {
                add_actor(actor);
            }
        }
    }
    
    public void add_actor(Actor actor) {
        actors.Add(actor);
        actor.action_runner = this;
        var roles = actor.GetComponents<IActing_role>();
        foreach (var role in roles) {
            role.actor = actor;
        }
        actor.roles = roles.ToList();
    }

    public void add_action(Action action) {
        current_actions.Add(action);
    }

    public void mark_action_as_finishing(Action action) {
        
        finishing_actions.Add(action);
        
        #if RVI_DEBUG
        Contract.Assert(!action.is_free_in_pool, $"action {action.get_explanation()} was free in pool, but it's completed now");
        var finishing_actions_debug = new List<string>();
        foreach (var finishing_action in finishing_actions) {
            finishing_actions_debug.Add(finishing_action.get_explanation());
        }
        Debug.Log($"mark_action_as_finishing: {action.get_explanation()}\n"+
            "finishing_actions=\n"+
            $"{String.Join(";\n", finishing_actions_debug)}"
        );
        #endif
    }
    public void mark_action_as_starting(Action action) {
        Contract.Assert(!starting_actions.Contains(action), 
            "action is marked twice!"
        );
        starting_actions.Add(action);

        #if RVI_DEBUG
        var starting_actions_debug = new List<string>();
        foreach (var starting_action in starting_actions) {
            starting_actions_debug.Add(starting_action.get_explanation());
        }
        Debug.Log($"mark_action_as_starting: {action.get_explanation()}\n"+
            "starting_actions="+
            $"{String.Join("\n;", starting_actions_debug)}"
        );
        #endif
    }
    

    
    public void update() {
        if (finishing_actions.Any()) {
            Debug.Log("Action_runner.update, first finish_actions");
            finish_actions(); // sequences & roots
        }
        if (starting_actions.Any()) {
            Debug.Log("Action_runner.update, start_actions");
            start_actions(); // sequences
            if (finishing_actions.Any()) {
                Debug.Log("Action_runner.update, second finish_actions");
                finish_actions(); // superceded by started
            }
        }
        start_fallback_actions();
        foreach (Action action in current_actions) {
            action.update();
        }
    }

    public void prepare_root_action_for_start(Action action) {
        Debug.Log($"ActionRunner.prepare_root_action_for_start {action.get_explanation()}");
        starting_actions.Add(action);
    }
    private void start_action(Action action) {
        #if RVI_DEBUG
        try {
            Debug.Log($"ActionRunner.start_action {action.get_explanation()}");
        }
        catch (Exception e) {
            Debug.Log("starting an action whose actors are probably destroyed");
        }
        #endif
        

        if (action.parent_action == null) {
            current_actions.Add(action);
            action.set_root_action(action);
            //Debug.Log($"action {action.get_explanation()} is registered as a root action");
        }
        
        action.start_execution_recursive();
        action.seize_needed_actors_recursive();
    }

    private void finish_action(Action action) {
        #if RVI_DEBUG
        Debug.Log($"ActionRunner.finish_action {action.get_explanation()}");
        #endif
        
        action.restore_state_recursive();
        action.free_actors_recursive();
        action.reset_recursive();
    }

    private void supercede_actions_which_use_same_actors() {
        foreach (Action action in current_actions) {
            if (action.superceded_as_root) {
                finish_action(action);
            }
        }
        current_actions.RemoveAll(action => action.superceded_as_root);
    }
    
    private void finish_actions() {
        foreach (Action finished_action in finishing_actions) {
            if (finished_action.is_completed) {
                finished_action.on_completed?.Invoke(finished_action);
                finished_action.on_completed_without_parameter?.Invoke();
            }
            finish_action(finished_action);
        }
        current_actions.RemoveAll(action => action.is_reset);
        starting_actions.RemoveAll(action => action.is_reset);
        finishing_actions.Clear();
    }
    private void start_actions() {
        foreach (Action action in starting_actions) {
            start_action(action);
        }
        starting_actions.Clear();
    }


    public void start_fallback_actions() {
        foreach (Actor actor in actors) {
            if (actor.current_action == null) {
                actor.on_lacking_action();
            }
        } 
    }

    private void notify_intelligence_about_finished_actions() {
        
    }
    
    public void on_root_action_completed(Action in_action) {
        Contract.Assert(!in_action.is_free_in_pool, $"action {in_action.get_explanation()} was free in pool, but it's completed now");
        finishing_actions.Add(in_action);
    }

    
#if UNITY_EDITOR
    protected void OnDrawGizmos() {
        GUIStyle myStyle = new GUIStyle {
            fontSize = 13,
            normal = {
                textColor = Color.green
            }
        };

        try {
            draw_actions(myStyle);
        } catch (Exception e) {
            Debug.LogError($"error in OnDrawGizmos for {name}: {e}, {e.Message}");
            //draw_actions(myStyle);
        }
        
    }

    private void draw_actions(GUIStyle style) {
        
        foreach (var actor in actors) {
            
            if (actor is Component actor_component) {
                if (
                    (actor.gameObject.GetComponent<ILeg>() != null)
                    ||
                    (actor.gameObject.GetComponent<Hitting_limbs_group>() != null)
                ){
                    //continue;
                }
                Handles.Label(
                    actor_component.transform.position, 
                    actor.current_action is null ? "null" : actor.current_action.ToString(), 
                    style);
            }
            else {
                Debug.LogWarning($"WARNING: an actor {actor} is not a component");
            }
        }
    }
#endif


}
}