using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;


namespace rvinowise.unity {

public partial class Creeping_leg_group {

    public override void hide_children_from_copying() {
        base.hide_children_from_copying();
        legs.Clear();
    }
    
    public override void distribute_data_across(
        IEnumerable<Abstract_children_group> new_controllers
    ) {
        Creeping_leg_group_distributor.distribute_data_across(
            this,
            new_controllers
        );

    }

    public void clear_data_related_to_children() {
        stable_leg_groups.Clear();        
    }
}

public static class Creeping_leg_group_distributor {
        
    public static void distribute_data_across(
        Creeping_leg_group src_group,
        IEnumerable<Abstract_children_group> new_controllers
    ) {
        
        List<Creeping_leg_group> new_leg_controllers = 
            new_controllers.Cast<Creeping_leg_group>().ToList();

        foreach (var leg_controller in new_leg_controllers) {
            Contract.Requires(leg_controller != null);    
        }

        distribute_stable_legs_groups(src_group, new_leg_controllers);
        init_moving_strategies(new_leg_controllers);
    }

    private static void init_moving_strategies(IList<Creeping_leg_group> leg_controllers) {
        foreach (var leg_controller in leg_controllers) {
            if (leg_controller.moving_strategy == null) {
                leg_controller.guess_moving_strategy();
            }
        }
    }

    private static void distribute_stable_legs_groups(
        Creeping_leg_group divided_leg_group,
        IReadOnlyList<Creeping_leg_group> all_leg_controllers) 
    {
        foreach (var leg_group in all_leg_controllers) {
            leg_group.clear_data_related_to_children();
        }
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
        }
    }

    private static Abstract_children_group get_controller_with_all_tools_from( //#generalize
        IReadOnlyList<Abstract_children_group> all_controllers,
        IReadOnlyList<IChild_of_group> in_legs
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
        IReadOnlyList<IChild_of_group> in_tools, 
        Abstract_children_group controller) 
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