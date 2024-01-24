using UnityEngine;


namespace units.human {

public class User_of_tools: MonoBehaviour {

    /*void change_main_tool_animation(AnimationEvent in_event) {
        Arm arm = arm_controller.right_arm;
        Contract.Requires(
            arm.held_tool != null,
            "must hold a tool to change its animation"
        );
        Tool held_tool = arm.held_tool;
        switch (in_event.stringParameter) {
            case "sideview":
                held_tool.animator.SetBool(in_event.stringParameter, Convert.ToBoolean(in_event.intParameter));
                held_tool.transform.localScale = new Vector3(1,1,1);
                break;
            case "slide":
                held_tool.animator.SetTrigger(in_event.stringParameter);
                break;
        }
        
    }*/

}
}