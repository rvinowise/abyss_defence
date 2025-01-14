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
    private readonly ISet<Collider2D> reacheble_colliders = new HashSet<Collider2D>();
    private readonly ISet<Damage_receiver> reacheble_damageable_enemies = new HashSet<Damage_receiver>();
    public AnimancerComponent animancer;
    public AnimationClip attacking_animation;
    
    public float cooldown_seconds = 1;
    private float last_attack_time = float.MinValue;

    private System.Action on_complete_callback;

    public Intelligence intelligence;

    private void Awake() {
        intelligence = GetComponentInParent<Intelligence>();
    }

    #region IWeaponry interface
    public override bool is_weapon_ready_for_target(Transform target) {
        return is_ready_to_attack() && is_directed_at_target(target);

    }

    //public IList<Damage_receiver> targets = new List<Damage_receiver>();
    public override IEnumerable<Damage_receiver> get_targets() {
        if (is_ready_to_attack()) {
            return reacheble_damageable_enemies;
        }
        return Enumerable.Empty<Damage_receiver>();
    }
    
    // public static IEnumerable<Damage_receiver> get_enemies_from_collisions(
    //     IEnumerable<Collider2D> colliders    
    // ) {
    //     IEnumerable<Damage_receiver> damageable_targets;
    //     foreach (var collider in colliders) {
    //         
    //     }
    //     colliders.Select(hit => collider.)
    //
    //     return damageable_targets;
    // }

    public bool is_directed_at_target(Transform target) {
        var target_colliders = target.GetComponents<Collider2D>();
        return reacheble_colliders.Intersect(target_colliders).Any();
    }

    public override float get_reaching_distance() {
        return attacked_area.transform.position.distance_to(transform.position);
    }


    // public static Damage_receiver get_damageable_enemy_from_collider(
    //     Team my_team,
    //     Collider2D collider
    // ) {
    //     if (collider.GetComponent<Damage_receiver>() is { } damage_receiver) {
    //         if (damage_receiver.intelligence.team.is_enemy_team(my_team)) {
    //             return damage_receiver;
    //         }
    //     }
    //     return null;
    // }
    public static Damage_receiver get_damageable_enemy_from_transform(
        Transform target,
        Team my_team
    ) {
        if (target.GetComponent<Damage_receiver>() is { } damage_receiver) {
            if (damage_receiver.intelligence.team.is_enemy_team(my_team)) {
                return damage_receiver;
            }
        }
        return null;
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        reacheble_colliders.Add(other);
        if (get_damageable_enemy_from_transform(other.transform, intelligence.team) is {} damageable) {
            reacheble_damageable_enemies.Add(damageable);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        reacheble_colliders.Remove(other);
        if (get_damageable_enemy_from_transform(other.transform, intelligence.team) is {} damageable) {
            reacheble_damageable_enemies.Remove(damageable);
        }
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
        foreach (var target in reacheble_colliders) {
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