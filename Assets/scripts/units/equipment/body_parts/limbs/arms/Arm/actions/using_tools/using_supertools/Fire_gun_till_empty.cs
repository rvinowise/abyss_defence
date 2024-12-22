using System.Linq;

using rvinowise.unity;
using rvinowise.unity.extensions;


namespace rvinowise.unity.actions {

public class Fire_gun_till_empty: Action_sequential_parent {

    private Humanoid user;
    private Arm left_arm;
    private Arm right_arm;
    private Baggage baggage;

    public IGun gun;
    public Tool tool;
    public IReloadable reloadable;
    public Ammo_compatibility ammo;
    
    public static Fire_gun_till_empty create(
        Humanoid in_user,
        IGun in_gun,
        Tool in_tool,
        IReloadable reloadable,
        Ammo_compatibility ammo
    ) {
        var action = (Fire_gun_till_empty)object_pool.get(typeof(Fire_gun_till_empty));
        action.user = in_user;
        
        Arm_pair arm_pair = action.user.arm_pair;
        action.left_arm = arm_pair.left_arm;
        action.right_arm = arm_pair.right_arm;

        action.baggage = action.user.baggage;
        action.gun = in_gun;
        action.tool = in_tool;
        action.reloadable = reloadable;
        action.ammo = ammo;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        Changing_tools.stop_changing_tools(left_arm,right_arm);
        start_using_supertool();
    }

    
    public void start_using_supertool() {
        var closest_arm = Find_best_arm_for_using_tool.get_best_arm_for_supertool(baggage, user.arm_pair);
        use_supertool_with_hand(closest_arm, gun, tool);
    }

    
    
    private void use_supertool_with_hand(Arm arm, IGun gun, Tool tool) {
        var previous_tool = arm.held_tool;
        add_children(
            Take_reloaded_tool_from_bag.create(
                arm,
                baggage,
                tool,
                reloadable,
                ammo
            ).add_marker("use supertool"),
            Fire_gun_at_target.create(
                gun,
                arm,
                Player_input.instance.cursor.transform,
                user.transform
            ),
            Take_tool_from_bag.create(
                arm,
                baggage,
                previous_tool
            ).add_marker("take previous tool after using a supertool supertool")
        );
        
    }
    

    
  
}
}