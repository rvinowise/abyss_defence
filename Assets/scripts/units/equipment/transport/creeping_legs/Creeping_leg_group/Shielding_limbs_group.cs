using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using rvinowise.contracts;
using rvinowise.unity.actions;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Shielding_limbs_group:
    Children_group
    ,IDefender
    ,IRunning_actions
{
    public List<Shielding_limb> shielding_limbs;
    public Creeping_leg_group creeping_leg_group;

    private Action_runner action_runner;

    private System.Action intelligence_on_shielded;
    
    
    
    #region IDefender
    
    public void start_defence(Transform danger, System.Action on_completed) {
        intelligence_on_shielded = on_completed;

        var direction_to_danger =
            this.transform.position.degrees_to(danger.position);

        var limb = find_limb_shielding_from_direction(direction_to_danger);

        if (limb != null) {
            limb.start_defence(danger, on_completed);
        }

    }

    private Shielding_limb find_limb_shielding_from_direction(Degree in_direction) {
        foreach (var limb in shielding_limbs) {
            if (limb.defended_span.to_absolute(creeping_leg_group.transform).use_minus().has_direction_inside(in_direction)) {
                return limb;
            }
        }
        return null;
    }
    
    public void finish_defence(System.Action on_completed) {
        Debug.Log("ATTACK_DEFENCE Shielding_limbs_group.finish_defence, all legs partake in moving");
        foreach (var leg in creeping_leg_group.legs) {
            Creeping_leg_partakes_in_moving.create(leg).start_as_root(action_runner);
        }
        on_completed?.Invoke();
    }
  

    protected void on_legs_are_shielding() {
        Idle.create(
            creeping_leg_group
        ).start_as_root(action_runner);
        foreach (var leg in creeping_leg_group.legs) {
            Idle.create(leg).start_as_root(action_runner);
        }
        intelligence_on_shielded();
    }
    
    #endregion
    

    #region IRunning_actions

    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }
    
    #endregion
    
    
    #region Children_group

    public override IEnumerable<IChild_of_group> children {
        get => shielding_limbs;
    }

    public override void add_child(IChild_of_group in_child) {
        shielding_limbs.Add(in_child as Shielding_limb);
    }
    
    public override void hide_children_from_copying() {
        base.hide_children_from_copying();
        shielding_limbs.Clear();
    }

    #endregion
}

}