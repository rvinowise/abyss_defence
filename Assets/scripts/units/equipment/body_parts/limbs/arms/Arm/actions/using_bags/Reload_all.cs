using rvinowise.contracts;
using rvinowise.unity.geometry2d;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Reload_all: Action_sequential_parent {

    private Humanoid user;
    private Human_intelligence intelligence;
    private Arm_pair arm_pair;
    private Arm left_arm;
    private Arm right_arm;
    private Baggage baggage;
    private ITransporter transporter;
    
    private Tool left_tool => left_arm.held_tool;
    private Tool right_tool => right_arm.held_tool;

    
    public static Reload_all create(
        Humanoid in_user, 
        Human_intelligence in_intelligence
    ) {
        var action = (Reload_all)object_pool.get(typeof(Reload_all));
        action.user = in_user;
        action.intelligence = in_intelligence;
        
        Arm_pair arm_pair = in_user.arm_pair;
        action.arm_pair = arm_pair;
        action.left_arm = arm_pair.left_arm;
        action.right_arm = arm_pair.right_arm;
        action.transporter = arm_pair.transporter;

        action.baggage = in_user.baggage;
        
        return action;
    }

    protected override void on_start_execution() {
        base.on_start_execution();
        reload_all();
    }
    

    private Side_type first_side;
    private Toolset reloaded_toolset;

    private void reload_all() {
        first_side = get_side_with_less_ammo();
        Arm gun_arm = arm_pair.get_arm_on_side(first_side);
        Arm ammo_arm = arm_pair.other_arm(gun_arm);
        reloaded_toolset = baggage.tool_sets[intelligence.toolset_equipper.desired_toolset_index];

        Action first_reloading_action = get_reloading_action_for(first_side).add_marker("first reloading");
        
        add_child(
            first_reloading_action
        );
    }

    private bool wanted_to_stop;
    
    public override void on_child_completed(Action sender_child) {
        
        if (sender_child.marker.StartsWith("first reloading")) {
            
            add_child(
                Equip_toolset.create(
                    user,
                    reloaded_toolset
                )
            );
            Action second_reloading_action = get_reloading_action_for(Side.flipped(first_side));
            add_child(
                second_reloading_action
            );
            
            add_child(
                Equip_toolset.create(
                    user,
                    reloaded_toolset
                )
            );
            
        }

        base.on_child_completed(sender_child);
    }

    private Action get_reloading_action_for(Side_type in_side) {
        Arm gun_arm = arm_pair.get_arm_on_side(in_side);
        Arm ammo_arm = arm_pair.other_arm(gun_arm);
        Tool reloaded_tool = reloaded_toolset.get_tool_on_side(in_side);
        if (reloaded_tool.GetComponent<Gun>() is {} pistol) {

            Ammunition magazine = user.baggage.get_ammo_object_for_gun(pistol);
            Contract.Requires(magazine != null);
            
            return Reload_pistol.create(
                user.animator,
                gun_arm,
                ammo_arm,
                user.baggage,
                pistol
            );
        }
        // if (reloaded_tool is Pump_shotgun shotgun) {
        //     return Reload_shotgun.create(
        //         user.animator,
        //         gun_arm,
        //         ammo_arm,
        //         user.baggage,
        //         shotgun,
        //         user.baggage.get_ammo_object_for_gun(shotgun)
        //     );
        // }
        // if (reloaded_tool is Break_shotgun break_shotgun) {
        //     return Reload_break_shotgun.create(
        //         user.animator,
        //         gun_arm,
        //         ammo_arm,
        //         user.baggage,
        //         break_shotgun,
        //         user.baggage.get_ammo_object_for_gun(break_shotgun)
        //     );
        // }
        return null;
    }
    
    private Side_type get_side_with_less_ammo() {
        if (left_tool == null && right_tool == null) {
            return Side_type.NONE;
        }
        var left_gun = left_tool.GetComponent<Gun>();
        var right_gun = left_tool.GetComponent<Gun>();
        
        int left_lacking_ammo = (left_gun?.max_ammo_qty - left_gun?.ammo_qty) ?? 0;
        int right_lacking_ammo = (right_gun?.max_ammo_qty - right_gun?.ammo_qty) ?? 0;

        int left_lacking_value = left_lacking_ammo;
        int right_lacking_value = right_lacking_ammo;

        if (left_lacking_value > right_lacking_value) {
            return Side_type.LEFT;
        }
        return Side_type.RIGHT;
    }




  
    
  
}
}