using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.contracts;


namespace rvinowise.unity.actions {

public class Expose_gun_for_reloading_COMPLEX: Action_leaf {

    public Arm arm;

    //private Gun gun;
    private Gun pistol;
    
    public static Action create(
        Arm in_arm
    ) {
        Contract.Requires(
            in_arm.held_tool is Gun,
            "the arm must hold a gun to reload it"
        );
        
        var action = (Expose_gun_for_reloading_COMPLEX)object_pool.get(typeof(Expose_gun_for_reloading_COMPLEX));
        action.arm = in_arm;
        
        if (in_arm.held_tool.GetComponent<Gun>() is Gun pistol) {
            action.pistol = pistol;
        }
        
        return action;
    }
    
    protected override void on_start_execution() {
        base.on_start_execution();
        arm.hand.gesture = Hand_gesture.Support_of_horizontal;
        arm.held_tool.transform.flipY(arm.folding_side == Side_type.RIGHT);
        arm.held_tool.animator.SetBool("sideview", true);
    }

    protected override void restore_state() {
        arm.held_tool.transform.flipY(false);
        arm.held_tool.animator.SetBool("sideview", false);
    }

    public override void update() {
        base.update();
        Debug.DrawLine(
            pistol.magazine_slot.transform.position,
            
            pistol.magazine_slot.transform.position + 
            pistol.magazine_slot.transform.TransformPoint(
                Vector3.right * pistol.magazine_slot.depth
            ),
            
            Color.magenta
        );
    }
}
}