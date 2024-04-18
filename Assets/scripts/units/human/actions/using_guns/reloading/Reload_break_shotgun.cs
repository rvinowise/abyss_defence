using UnityEngine;

namespace rvinowise.unity.actions {

public class Reload_break_shotgun: Reload_gun {
    
    private static readonly int animation_reloading = Animator.StringToHash("Base Layer.reloading_break_shotgun");    

    
    private Baggage bag;
    private Break_shotgun gun;
    private Ammunition ammo;
    

    
    
    public static Reload_break_shotgun create(
        Animator in_animator,
        Arm in_gun_arm,
        Arm in_ammo_arm,
        Baggage in_bag, 
        Break_shotgun in_tool,
        Ammunition in_ammo
    ) {
        var action = (Reload_break_shotgun)object_pool.get(typeof(Reload_break_shotgun));
        
        action.animator = in_animator;
        action.gun_arm = in_gun_arm;
        action.ammo_arm = in_ammo_arm;
        action.bag = in_bag;
        action.gun = in_tool;
        action.ammo = in_ammo;
        action.init_child_actions();
        
        return action;
    }
    

    public void init_child_actions() {
        

        this.add_child(
            Action_parallel_parent.create(
                actions.Arm_reach_relative_directions.create_assuming_left_arm(
                    gun_arm,
                    90f,
                    -50f,
                    -50f,
                    0f
                ),
                actions.Take_tool_from_bag.create(
                    ammo_arm,
                    bag,
                    ammo
                )
            )
        );
        this.add_child(
            Play_recorded_animation.create(
                animator,
                animation_reloading,
                should_be_flipped()
            )
        );
        

    }

 



    
}
}