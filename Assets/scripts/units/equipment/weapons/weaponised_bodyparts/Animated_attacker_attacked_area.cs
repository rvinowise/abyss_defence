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
public class Animated_attacker_attacked_area : MonoBehaviour
{

    public Collider2D attacked_area;
    public Collider2D area_starting_attack;
    public readonly ISet<Collider2D> reacheble_colliders = new HashSet<Collider2D>();
    public readonly ISet<Damage_receiver> reacheble_damageable_enemies = new HashSet<Damage_receiver>();

    public Team team;
    

    private void Start() {
        if (GetComponentInParent<Animated_attacker>() is Animated_attacker animated_attacker) {
            team = animated_attacker.intelligence?.team;
        }
        else if (GetComponentInParent<Intelligence>() is {} intelligence) {
            team = intelligence.team;
        }
    }


    void OnTriggerEnter2D(Collider2D other) {
        reacheble_colliders.Add(other);
        if (
            Animated_attacker.get_damagable_enemy_from_transform(
                other.transform, 
                team
            ) is {} damageable) {
            reacheble_damageable_enemies.Add(damageable);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        reacheble_colliders.Remove(other);
        if (
            Animated_attacker.get_damagable_enemy_from_transform(
                other.transform, 
                team
            ) is {} damageable) {
            reacheble_damageable_enemies.Remove(damageable);
        }
    }




    


}

}