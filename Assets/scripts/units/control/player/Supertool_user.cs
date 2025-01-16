using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {

public class Supertool_user: MonoBehaviour {
    
    public Baggage baggage;
    public int desired_tool_index;
    public Humanoid humanoid;
    public Action_runner action_runner;

    public int get_current_tool_index() {
        var left_tool = humanoid.arm_pair.left_tool;
        var right_tool = humanoid.arm_pair.right_tool;

        for (int tool_index = 0; tool_index < baggage.tool_sets.Count; tool_index++) {
            var tool = baggage.tool_sets[tool_index];
            if ((left_tool == tool.left_tool)&&(right_tool == tool.right_tool)) {
                return tool_index;
            }
        }
        return -1;
    }
    
    public void use_desired_supertool() {
        var supertool_description = baggage.supertool_descriptions[desired_tool_index];
        supertool_description.start_using_action(humanoid);
    }
    
    private int get_desired_tool(int wheel_steps) {
        int desired_current_equipped_set = desired_tool_index + wheel_steps;
        desired_current_equipped_set %= baggage.supertool_descriptions.Count;
        if (desired_current_equipped_set < 0) {
            desired_current_equipped_set = baggage.supertool_descriptions.Count + desired_current_equipped_set;
        }
        return desired_current_equipped_set;
    }
    
    public void switch_supertool_to_steps(int wheel_steps) {
        if (desired_tool_index == -1) {
            desired_tool_index = get_current_tool_index();
        }
        desired_tool_index = get_desired_tool(wheel_steps);
        Debug.Log($"selected supertool = {baggage.supertool_descriptions[desired_tool_index].tool_name}");
    }
}

}