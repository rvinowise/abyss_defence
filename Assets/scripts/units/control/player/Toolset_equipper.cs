using rvinowise.unity.actions;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity {

public class Toolset_equipper: MonoBehaviour {
    
    public Baggage baggage;
    public int desired_toolset_index = -1;
    public Humanoid humanoid;
    public Action_runner action_runner;

    public int get_current_toolset_index() {
        var left_tool = humanoid.arm_pair.left_tool;
        var right_tool = humanoid.arm_pair.right_tool;

        for (int toolset_index = 0; toolset_index < baggage.tool_sets.Count; toolset_index++) {
            var toolset = baggage.tool_sets[toolset_index];
            if ((left_tool == toolset.left_tool)&&(right_tool == toolset.right_tool)) {
                return toolset_index;
            }
        }
        return -1;
    }
    
    public void equip_tool_set(int set_index) {
        Equip_toolset.create(
            humanoid,
            baggage.tool_sets[set_index]
        ).add_marker("changing tool").start_as_root(action_runner);
    }
    
    private int get_desired_toolset(int wheel_steps) {
        int desired_current_equipped_set = desired_toolset_index + wheel_steps;
        desired_current_equipped_set %= baggage.tool_sets.Count;
        if (desired_current_equipped_set < 0) {
            desired_current_equipped_set = baggage.tool_sets.Count + desired_current_equipped_set;
        }
        return desired_current_equipped_set;
    }
    
    public void switch_toolset_to_steps(int wheel_steps) {
        if (desired_toolset_index == -1) {
            desired_toolset_index = get_current_toolset_index();
        }
        int desired_toolset = get_desired_toolset(wheel_steps);
        if (desired_toolset != desired_toolset_index) {
            Debug.Log($"Mouse wheel is triggered, {wheel_steps} steps");
            equip_tool_set(desired_toolset);
        }
    }
}

}