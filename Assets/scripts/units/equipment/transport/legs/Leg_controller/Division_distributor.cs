using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.equipment.limbs.strategy;


namespace rvinowise.units.equipment.limbs {

public partial class Leg_controller {

    public override void distribute_data_across(
        IEnumerable<Equipment_controller> new_controllers
    ) {
        base.distribute_data_across(new_controllers);
        Division_distributor.distribute_data_across(
            this,
            new_controllers
        );

    }

    public static class Division_distributor {
        
        public static void distribute_data_across(
            Leg_controller base_controller,
            IEnumerable<Equipment_controller> new_controllers
        ) {
            
            IEnumerable<Leg_controller> new_leg_controllers = 
                new_controllers.Cast<Leg_controller>().ToList();
    
            foreach (var leg_controller in new_leg_controllers) {
                Contract.Requires(leg_controller != null);    
            }

            if (base_controller.moving_strategy is strategy.Stable stable_strategy) {
                distribute_stable_legs_groups(stable_strategy, new_leg_controllers);
            }
            init_moving_strategies(new_leg_controllers);
        }

        private static void init_moving_strategies(IEnumerable<Leg_controller> leg_controllers) {
            foreach (var leg_controller in leg_controllers) {
                if (leg_controller.moving_strategy == null) {
                    leg_controller.guess_moving_strategy();
                }
            }
        }

        private static void distribute_stable_legs_groups(
            strategy.Stable stable_strategy,
            IEnumerable<Leg_controller> all_leg_controllers) 
        {
            foreach (var stable_leg_group in stable_strategy.stable_leg_groups) {
                if (
                    get_controller_with_all_tools_from(
                        all_leg_controllers,
                        stable_leg_group.legs
                    ) is Leg_controller undivided_controller) 
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
    
        private static Equipment_controller get_controller_with_all_tools_from( //#generalize
            IEnumerable<Equipment_controller> all_controllers,
            IEnumerable<Tool> in_legs
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
            IEnumerable<Tool> in_tools, 
            Equipment_controller controller) 
        {
            foreach (var tool in in_tools) {
                if (!controller.has_tool(tool)) {
                    return false;
                }
            }
            return true;
        }
        
    }
    
    
}
}