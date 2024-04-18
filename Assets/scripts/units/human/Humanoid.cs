using UnityEngine;
using rvinowise.contracts;
using rvinowise.unity.actions;
using Action = rvinowise.unity.actions.Action;

namespace rvinowise.unity {


[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Humanoid:
    MonoBehaviour
    ,IFlippable_actor
    ,IActor
{

    /* parts of the human*/
    public Arm_pair arm_pair;
    public Head head;
    //public units.parts.humanoid.Legs legs;
    public Baggage baggage;
    
    
    //private IChildren_groups_host user_of_equipment;
    
    
    /* components */
    private SpriteRenderer sprite_renderer;
    public Animator animator;
    
    
    #region IActor
    public Action current_action { set; get; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(action_runner);
    }

    private Action_runner action_runner;
    public void init_for_runner(Action_runner action_runner) {
        this.action_runner = action_runner;
    }
    #endregion


    /* Humanoid itself */

    public void pick_up(Tool in_tool) {
        if (in_tool is Ammunition ammo) {
            baggage.change_ammo_qty(ammo.compatibility, ammo.rounds_qty);
        }
    }

    public void pick_up(Ammunition in_ammo) {
        baggage.change_ammo_qty(
            in_ammo.compatibility,
            in_ammo.rounds_qty
            );
    }
    protected void Awake()
    {
        init_components();
    }

    private void init_components() {
        animator = GetComponent<Animator>();
        if (animator){
            animator.enabled = false;
        }
        arm_pair = GetComponent<Arm_pair>();
    }

    


    void FixedUpdate() {
        head.rotate_to_desired_direction();
    }

  
    
    /* [called_in_animation]
    void change_main_tool_animation(AnimationEvent in_event) {
        
        switch (in_event.stringParameter) {
            case "sideview":
               
                break;
            case "slide":
                
                break;
            
            case "opened":
                break;
        }
        
    } */

    /* IFlippable_actor interface */
    public void flip_for_animation(bool in_flipped) {
        if (in_flipped != is_flipped()) {
            float scale = Mathf.Abs(transform.localScale.x);
            if (in_flipped) {
                transform.localScale = new Vector3(scale, -scale, 1);
            } else {
                transform.localScale = new Vector3(scale, scale, 1);
            }
            
            switch_tools();
        }
    }
    public bool is_flipped() {
        return transform.localScale.y < 0;
    }

    public void restore_after_flipping() {
        Contract.Requires(is_flipped(), "restoring after flipping is only makes sense if flipped");
        arm_pair.switch_arms_angles();
        stop_arms();
        flip_for_animation(false);
        //Debug.Break();
    }
    
    private void stop_arms() {
        arm_pair.left_arm.shoulder.current_rotation_inertia = 0;
        arm_pair.left_arm.segment1.current_rotation_inertia = 0;
        arm_pair.left_arm.segment2.current_rotation_inertia = 0;
        arm_pair.left_arm.hand.current_rotation_inertia = 0;
        arm_pair.right_arm.shoulder.current_rotation_inertia = 0;
        arm_pair.right_arm.segment1.current_rotation_inertia = 0;
        arm_pair.right_arm.segment2.current_rotation_inertia = 0;
        arm_pair.right_arm.hand.current_rotation_inertia = 0;
    }
    
    private void switch_tools() {
        arm_pair.switch_tools();
    }
    
    private void OnDestroy()
    {
        var st = new System.Diagnostics.StackTrace(true);
        var sfs = st.GetFrames();
        foreach (var sf in sfs)
        {
            Debug.Log(sf.GetFileName() + ", " + sf.GetFileLineNumber());
        }
    }
}
}