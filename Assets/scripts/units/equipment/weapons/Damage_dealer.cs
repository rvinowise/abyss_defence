using System.Collections.Generic;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using UnityEngine;
using Action = System.Action;


namespace rvinowise.unity {
public class Damage_dealer: MonoBehaviour {
    
    public float effect_amount = 1f;
    public GameObject hit_impact_prefab;
    
    private readonly ISet<Transform> damaged_targets = new HashSet<Transform>();
    private Transform attacker;

    
    public void on_restore_from_pool() {
        attacker = null;
        damaged_targets.Clear();
    }
    
    public void remember_damaged_target(Transform target) {
        damaged_targets.Add(target);
        if (target.GetComponent<Divisible_body>() is {} divisible_body) {
            divisible_body.remember_damage_dealer(this);
        }
    }
    
    public void forget_damaged_targets() {
        foreach (var target in damaged_targets) {
            if ((target != null)&&(target.GetComponent<Divisible_body>() is {} divisible_body)) {
                divisible_body.forget_damage_dealer(this);
            }
        }
        damaged_targets.Clear();
        
    }

    public bool was_target_damaged(Transform target) {
        return damaged_targets.Contains(target);
    }


    public void set_attacker(Transform in_attacker) {
        attacker = in_attacker;
    }
    
    public bool is_ignoring_damage_receiver(Damage_receiver receiver) {
        return
            was_target_damaged(receiver.transform)
            ||
            attacker == receiver.transform;
    }
    
    
    public void create_hit_impact( 
        Vector3 in_position,
        Vector2 in_impulse
    ) {
        // var contact_with_height =
        //     in_position.with_height(0);
        if (hit_impact_prefab != null) {
            var effect = Instantiate(hit_impact_prefab, in_position, in_impulse.to_quaternion());
        }
    }
}


}
