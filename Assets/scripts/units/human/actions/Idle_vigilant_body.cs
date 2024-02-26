using UnityEngine;
using rvinowise.unity;


namespace rvinowise.unity.actions {

public class Idle_vigilant_body: Action_parallel_parent {


    private Arm left_arm;
    private Arm right_arm;
    private Transform target;
    private ITransporter transporter; // movements of arms depend on where the body is moving
    
    public static Idle_vigilant_body create(   
        Arm in_left_arm,
        Arm in_right_arm,
        Transform in_target,
        ITransporter in_transporter
    ) {
        Idle_vigilant_body action = (Idle_vigilant_body)pool.get(typeof(Idle_vigilant_body));
        action.left_arm = in_left_arm;
        action.right_arm = in_right_arm;
        action.target = in_target;
        action.transporter = in_transporter;
        
        action.init_child_actions();
        return action;
    }
    
    private void init_child_actions() {
        child_actions.Add(
            Idle_vigilant_only_arm.create(left_arm, target, transporter)
        );
        child_actions.Add(
            Idle_vigilant_only_arm.create(right_arm, target, transporter)
        );
  
    }
}
}