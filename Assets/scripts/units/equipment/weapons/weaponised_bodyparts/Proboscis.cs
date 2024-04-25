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
    IActor_attacker

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
    public override bool can_reach(Transform target) {
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

    void check_damaged_victims() {
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
            check_damaged_victims();
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
    
    
    #region IActor

    private Action_runner action_runner;
    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }

    public Action current_action { get; set; }
    public void on_lacking_action() {
        Idle.create(this).start_as_root(action_runner);
    }

    #endregion
}

}