using UnityEngine;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.limbs.actions;
using rvinowise.unity.units.parts.limbs.creeping_legs;


namespace rvinowise.unity.units.parts.limbs.arms.actions {

public class Attacking_with_creeping_legs: Action_parallel_parent {

    private Creeping_leg_group leg_group;
    private Transform target;
    private ILeg attacking_leg;
    public static Attacking_with_creeping_legs create(
        Creeping_leg_group leg_group,
        Transform in_target
    ) {
        var action = (Attacking_with_creeping_legs)pool.get(typeof(Attacking_with_creeping_legs));
        action.leg_group = leg_group;
        
        action.target = in_target;

        return action;
    }


    protected override void on_start_execution() {
        attacking_leg = leg_group.get_best_leg_for_hitting(target) ;
        if (attacking_leg is Limb2 best_limb2) {
            
            add_children(
                Keep_distance_from_target.create(
                    leg_group, 
                    best_limb2.get_reaching_distance(),
                    target
                ),
                Hitting_with_limb2.create(best_limb2, leg_group, target)
            );
        }
        leg_group.ensure_leg_raised(attacking_leg);
    }


}
}