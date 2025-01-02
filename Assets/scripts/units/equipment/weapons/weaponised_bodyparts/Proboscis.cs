using System;
using System.Collections.Generic;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {
public class Proboscis : 
    Attacker_child_of_group,
    IAttacker

{
    public Transform beginning;
    public Transform tip;

    public float max_length;
    public Animator animator;
    private static readonly int strike = Animator.StringToHash("strike");

    private readonly ISet<Collider2D> damaged_targets = new HashSet<Collider2D>();
    private bool has_damaged_target = false;

    private System.Action intelligence_on_completed;
    
    protected void Awake() {
        animator = GetComponent<Animator>();
    }

    #region IWeaponry interface
    public override bool is_weapon_targeting_target(Transform target) {
        var distance_to_target =
            transform.position.distance_to(target.position); 
        var angle_to_target =
            transform.rotation.to_degree().angle_to(transform.position.degrees_to(target.position));

        return 
            distance_to_target <= max_length
            &&
            Math.Abs(angle_to_target.degrees) <= 10f;
    }

    public override float get_reaching_distance() {
        return max_length;
    }

    public override void attack(Transform target, System.Action on_completed = null) {
        animator.SetTrigger(strike);
        intelligence_on_completed = on_completed;
    }
    
    #endregion IWeaponry interface

    public bool is_attacking() {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("strike");
    }

    void damage_victims() {
        var damaging_vector =
            tip.position - beginning.position;

        var hits = Physics2D.RaycastAll(
            beginning.position,
            damaging_vector,
            damaging_vector.magnitude

        );
        foreach (var hit in hits) {
            if (
                hit.collider != null &&
                //(!damaged_targets.Contains(hit.collider))&&
                hit.collider.GetComponent<Divisible_body>() is { } divisible
            ) {
                //damaged_targets.Add(hit.collider);
                Debug.Log($"ATTACK: Attacker '{this.name}' owned by '{this.transform.parent}' has damaged '{divisible.name}'");
                has_damaged_target = true;
                divisible.damage_by_impact(
                    Damaging_polygons.get_splitting_wedge(
                        new Ray2D(beginning.position, transform.rotation.to_vector())
                    ),
                    hit.point,
                    transform.rotation.to_vector()
                );
            }
        }
        
    }
    
    
    protected void FixedUpdate() {
        if (
            is_attacking()&&
            !has_damaged_target
        )
        {
            damage_victims();
        }
    }

    [called_in_animation]
    public void on_completed() {
        if (this == null) {
            return;
        }
        forget_damaged_targets();
        intelligence_on_completed?.Invoke();
    }

    private void forget_damaged_targets() {
        damaged_targets.Clear();
        has_damaged_target = false;
    }
    
    
}

}