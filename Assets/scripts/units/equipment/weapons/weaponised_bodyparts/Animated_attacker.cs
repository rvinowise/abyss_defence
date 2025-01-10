using System;
using System.Collections.Generic;
using Animancer;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {
public class Animated_attacker : 
    Attacker_child_of_group,
    IAttacker
{

    public Collider2D attacked_area;
    public Collider2D area_starting_attack;
    private readonly ISet<Collider2D> reacheble_targets = new HashSet<Collider2D>();
    public AnimancerComponent animancer;
    public AnimationClip attacking_animation;
    
    public float cooldown_seconds = 1;
    private float last_attack_time = float.MinValue;

    private System.Action intelligence_on_complete;
    
    
    
    
    #region IWeaponry interface
    public override bool is_weapon_ready_for_target(Transform target) {
        return is_ready_to_attack() && is_directed_at_target(target);

    }

    public bool is_directed_at_target(Transform target) {
        var target_collider = target.GetComponent<Collider2D>();
        return reacheble_targets.Contains(target_collider);
    }

    public override float get_reaching_distance() {
        return attacked_area.transform.position.distance_to(transform.position);
    }

    void OnTriggerEnter2D(Collider2D other) {
        // if (other.GetComponent<Damage_receiver>() is Damage_receiver damage_receiver) {
        //     damage_receiver.receive_damage();
        // }
        reacheble_targets.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other) {
        reacheble_targets.Remove(other);
    }




    public override void attack(Transform target, System.Action on_completed = null) {
        if (is_ready_to_attack()) {
            play_attack_animation();
            last_attack_time = Time.time;
            intelligence_on_complete = on_completed;
        }
        
    }
    
    #endregion IWeaponry interface

    private bool is_ready_to_attack() {
        return Time.time - last_attack_time > cooldown_seconds;
    }

    private AnimancerState animation_state;
    private void play_attack_animation() {
        animation_state = animancer.play_from_scratch(attacking_animation, on_animation_ended);
    }

    [called_in_animation]
    public void on_damage_started() {
        attacked_area.gameObject.SetActive(true);
    }
    
    [called_in_animation]
    public void on_damage_ended() {
        attacked_area.gameObject.SetActive(false);
    }
    
    [called_in_animation]
    public void on_animation_ended() {
        animation_state.Time = 0;
        animation_state.Speed = 0;
        animation_state.Events.OnEnd = null;
        intelligence_on_complete?.Invoke();
    }

    

    #region IActor
     
    

    public override void on_lacking_action() {
        Idle.create(actor).start_as_root(actor.action_runner);
    }

    #endregion

}

}