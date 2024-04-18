using System.Collections.Generic;
using rvinowise.unity.actions;
using UnityEngine;
using Action = System.Action;


namespace rvinowise.unity {
public class Damage_dealer: MonoBehaviour {
    public ISet<Transform> targets = new HashSet<Transform>();


    public void remember_damaged_target(Transform target) {
        targets.Add(target);
        if (target.GetComponent<Divisible_body>() is {} divisible_body) {
            divisible_body.remember_damage_dealer(this);
        }
    }
    
    public void forget_damaged_targets() {
        foreach (var target in targets) {
            if ((target != null)&&(target.GetComponent<Divisible_body>() is {} divisible_body)) {
                divisible_body.forget_damage_dealer(this);
            }
        }
        targets.Clear();
        
    }

    public bool was_target_damaged(Transform target) {
        return targets.Contains(target);
    }
}


}
