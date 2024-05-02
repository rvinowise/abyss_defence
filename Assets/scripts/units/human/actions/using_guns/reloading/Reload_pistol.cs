using UnityEngine;

namespace rvinowise.unity.actions {

public class Reload_pistol: Reload_gun {
    
    private static readonly int animation_reload_pistol = Animator.StringToHash("Base Layer.reloading_pistol");    

    private Baggage bag;
    private Gun gun;
    private Ammunition magazine;
    
    public static Reload_pistol create(
        Animator in_animator,
        Arm in_gun_arm,
        Arm in_magazine_arm,
        Baggage in_bag, 
        Gun in_gun
    ) {
        var action = (Reload_pistol)object_pool.get(typeof(Reload_pistol));
        
        action.animator = in_animator;
        action.gun_arm = in_gun_arm;
        action.ammo_arm = in_magazine_arm;
        action.bag = in_bag;
        action.gun = in_gun;
        action.init_child_actions();
        
        return action;
    }


    private void init_child_actions() {
        
        add_child(
            Action_parallel_parent.create(
                Arm_reach_relative_directions.create_assuming_left_arm(
                    gun_arm,
                    90f,
                    -50f,
                    -50f,
                    0f
                ),
                Take_ammo_from_bag.create(
                    ammo_arm,
                    bag,
                    gun
                )
            )
        );
        add_child(
            Play_recorded_animation.create(
                animator,
                animation_reload_pistol,
                should_be_flipped()
            )
        );

    }

    protected override void restore_state() {
        adjust_desired_positions();
    }

    private void adjust_desired_positions() {
        ammo_arm.segment1.set_target_rotation(ammo_arm.segment1.rotation);
        ammo_arm.segment2.set_target_rotation(ammo_arm.segment2.rotation);
        
        gun_arm.segment1.set_target_rotation(gun_arm.segment1.rotation);
        gun_arm.segment2.set_target_rotation(gun_arm.segment2.rotation);
    }


}
}