using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace rvinowise.unity {


    
public class Remembering_collision : MonoBehaviour {


    public Collider2D remembering_area;
    public readonly ISet<Collider2D> reacheble_colliders = new HashSet<Collider2D>();
    //public readonly ISet<Damage_receiver> reacheble_damageable_enemies = new HashSet<Damage_receiver>();

    public Team team;

    void OnTriggerEnter2D(Collider2D other) {
        reacheble_colliders.Add(other);
        if (
            Animated_attacker.get_damagable_enemy_from_transform(
                other.transform, 
                team
            ) is {} damageable) {
            //reacheble_damageable_enemies.Add(damageable);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        reacheble_colliders.Remove(other);
        if (
            Animated_attacker.get_damagable_enemy_from_transform(
                other.transform, 
                team
            ) is {} damageable) {
            //reacheble_damageable_enemies.Remove(damageable);
        }
    }

}

    
}