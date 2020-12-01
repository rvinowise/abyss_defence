using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;
using rvinowise.unity.units.parts.limbs.creeping_legs.strategy;


namespace rvinowise.unity.units.parts.limbs.creeping_legs {

public partial class Creeping_leg_group {

    public override void distribute_data_across(
        IEnumerable<Children_group> new_controllers
    ) {
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

            distribute_stable_legs_groups(base_group, new_leg_controllers);
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
            Creeping_leg_group divided_leg_group,
            IEnumerable<Creeping_leg_group> all_leg_controllers) 
        {
            foreach (var stable_leg_group in divided_leg_group.stable_leg_groups) {
                if (
                    get_controller_with_all_tools_from(
                        all_leg_controllers,
                        stable_leg_group.legs
                    ) is Creeping_leg_group undivided_controller) 
                {
                    undivided_controller.stable_leg_groups.Add(
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
            IEnumerable<ICompound_object> in_legs
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
            IEnumerable<ICompound_object> in_tools, 
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