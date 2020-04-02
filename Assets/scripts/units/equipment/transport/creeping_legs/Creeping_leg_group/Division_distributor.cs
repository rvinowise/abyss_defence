using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.limbs.legs.strategy;


namespace rvinowise.units.parts.limbs.legs {

public partial class Creeping_leg_group {

    public override void distribute_data_across(
        IEnumerable<Children_group> new_controllers
    ) {
        base.distribute_data_across(new_controllers);
        Division_distributor.distribute_data_across(
            this,
            new_controllers
        );

    }

    public static class Division_distributor {
        
        public static void distribute_data_across(
            Creeping_leg_group base_group,
            IEnumerable<Children_group> new_controllers
        ) {
            
            IEnumerable<Creeping_leg_group> new_leg_controllers = 
                new_controllers.Cast<Creeping_leg_group>().ToList();
    
            foreach (var leg_controller in new_leg_controllers) {
                Contract.Requires(leg_controller != null);    
            }

            if (base_group.moving_strategy is strategy.Stable stable_strategy) {
                distribute_stable_legs_groups(stable_strategy, new_leg_controllers);
            }
            init_moving_strategies(new_leg_controllers);
        }

        private static void init_moving_strategies(IEnumerable<Creeping_leg_group> leg_controllers) {
            foreach (var leg_controller in leg_controllers) {
                if (leg_controller.moving_strategy == null) {
                    leg_controller.guess_moving_strategy();
                }
            }
        }

        private static void distribute_stable_legs_groups(
            strategy.Stable stable_strategy,
            IEnumerable<Creeping_leg_group> all_leg_controllers) 
        {
            foreach (var stable_leg_group in stable_strategy.stable_leg_groups) {
                if (
                    get_controller_with_all_tools_from(
                        all_leg_controllers,
                        stable_leg_group.legs
                    ) is Creeping_leg_group undivided_controller) 
                {
                    if (undivided_controller.moving_strategy == null) {
                        undivided_controller.moving_strategy = new strategy.Stable(
                            undivided_controller.legs
                        );
                    }
                    strategy.Stable new_stable_strategy = undivided_controller.moving_strategy as strategy.Stable;
                    new_stable_strategy.stable_leg_groups.Add(
                        stable_leg_group
                    );
    
                }
                else {
                    //Destroy(stable_leg_group);
                }
            }
        }
    
        private static Children_group get_controller_with_all_tools_from( //#generalize
            IEnumerable<Children_group> all_controllers,
            IEnumerable<Child> in_legs
            ) 
        {
            foreach (var controller in all_controllers) {
                if (all_tools_are_within_controller(in_legs, controller)) {
                    return controller;
                }
            }
            return null;
        }
    
        //#generalize
        private static bool all_tools_are_within_controller(
            IEnumerable<Child> in_tools, 
            Children_group controller) 
        {
            foreach (var tool in in_tools) {
                if (!controller.has_child(tool)) {
                    return false;
                }
            }
            return true;
        }
        
    }
    
    
}
}