using System;
using System.Collections.Generic;
using System.Linq;
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
    public float dealt_damage = 1;
    private readonly ISet<Collider2D> reacheble_targets = new HashSet<Collider2D>();
    public AnimancerComponent animancer;
    public AnimationClip attacking_animation;
    
    public float cooldown_seconds = 1;
    private float last_attack_time = float.MinValue;

    private System.Action on_complete_callback;
    
    
    
    
    #region IWeaponry interface
    public override bool is_weapon_ready_for_target(Transform target) {
        return is_ready_to_attack() && is_directed_at_target(target);

    }

    public bool is_directed_at_target(Transform target) {
        var target_colliders = target.GetComponents<Collider2D>();
        return reacheble_targets.Intersect(target_colliders).Any();
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
            on_complete_callback = on_completed;
        }
        
    }
    
    #endregion IWeaponry interface

    private bool is_ready_to_attack() {
        return Time.time - last_attack_time > cooldown_seconds;
    }

    private AnimancerState animation_state;
    private void play_attack_animation() {
        animation_state = animancer.play_from_scratch(attacking_animation, on_animation_ended);
        //animation_state.SetWeight(1);
    }

    [called_in_animation]
    public void on_damage_started() {
        //attacked_area.gameObject.SetActive(true);
        foreach (var target in reacheble_targets) {
            if (target.GetComponent<Damage_receiver>() is { } damagable) {
                damagable.receive_damage(dealt_damage);
            }
            if (target.GetComponent<IBleeding_body>() is { } bleeding) {
                bleeding.create_splash(target.transform.position, transform.rotation.to_vector());
            }
        }
    }
    
    [called_in_animation]
    public void on_damage_ended() {
        //attacked_area.gameObject.SetActive(false);
    }
    
    [called_in_animation]
    public void on_animation_ended() {
        animation_state.IsPlaying = false;
        animation_state.Events.OnEnd = null;
        on_complete_callback?.Invoke();
    }

    

    #region IActor
     
    

    public override void on_lacking_action() {
        Idle.create(actor).start_as_root(actor.action_runner);
    }

    #endregion

}

}