using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;
using Animancer;


namespace rvinowise.unity {
public class Fighting_head :
    MonoBehaviour,
    //Attacker_child_of_group,
    IAttacker,
    ISensory_organ
{

    enum State {
        CALM,
        PREPARED_FOR_ATTACK
    }


    //private State fighting_state;

    public Animated_attacker animated_attacker;
    public AnimancerComponent animancer;
    
    public AnimationClip prepare_for_attack_clip;
    public AnimationClip attack_clip;
    public AnimationClip stop_preparing_for_attack_clip;

    public Turning_element turning_element;

    public Transform target;

    public float distance_of_fight = 3;

    private void Awake() {
        assume_calm_state();
    }

    private void assume_calm_state() {
        var animation_state = animancer.play_from_scratch(prepare_for_attack_clip, on_prepared_for_attack);
        animation_state.IsPlaying = false;
        //animation_state.NormalizedTime = 0;
    }

    private void Update() {
        if (target != null) {
            var rotation_to_target = transform.quaternion_to(target.position);
            turning_element.rotate_towards(rotation_to_target);
            var vector_to_target = target.position - transform.position;
            var distance_to_target = vector_to_target.magnitude;
            if (is_needed_preparing_for_attack(distance_to_target)) {
                if (is_calm()) {
                    animancer.play_from_scratch(prepare_for_attack_clip, on_prepared_for_attack);
                }
            } else {
                if (is_prepared_for_attack()) {
                    animancer.play_from_scratch(stop_preparing_for_attack_clip, on_calmed);
                }
            }
        }
    }

    public bool is_needed_preparing_for_attack(float distance_to_target) {
        return distance_to_target <= distance_of_fight;
    }

    private void on_prepared_for_attack() {
        //fighting_state = State.PREPARED_FOR_ATTACK;
        animancer.States.Current.IsPlaying = false;
        animancer.States.Current.Events.OnEnd = null;
    }
    private void on_calmed() {
        //fighting_state = State.CALM;
        assume_calm_state();
    }

    private void on_attack_completed() {
        assume_calm_state();
        on_attack_completed_callback.Invoke();
    }
    
    private bool is_calm() {
        var animation_state = animancer.States.Current;
        return
            !animation_state.IsPlaying
            &&
            animation_state.Clip == prepare_for_attack_clip
            &&
            animation_state.NormalizedTime == 0;
    }
    private bool is_prepared_for_attack() {
        var animation_state = animancer.States.Current;
        return
            !animation_state.IsPlaying
            &&
            animation_state.Clip == prepare_for_attack_clip
            &&
            animation_state.NormalizedTime >= 1;
    }
    

    #region Sensor
    public void pay_attention_to_target(Transform target) {
        this.target = target; 
        
    }

    public bool is_focused_on_target() {
        return true;
    }
    #endregion

    #region IWeaponry interface
    public bool is_weapon_ready_for_target(Transform target) {
        return 
            is_prepared_for_attack()
            &&
            animated_attacker.is_weapon_ready_for_target(target);
    }

    public IEnumerable<Damage_receiver> get_targets() {
        if (is_prepared_for_attack()) {
            return animated_attacker.get_targets();
        }
        return Enumerable.Empty<Damage_receiver>();
    }

    public float get_reaching_distance() {
        return animated_attacker.get_reaching_distance();
    }

    private System.Action on_attack_completed_callback;
    public void attack(Transform target, System.Action on_completed = null) {
        on_attack_completed_callback = on_completed;
        animated_attacker.attack(target, on_attack_completed);
    
    }
    
    #endregion

    

    #region IActor
     
    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(actor).start_as_root(actor.action_runner);
    }

    #endregion

}

}