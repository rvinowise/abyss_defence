using rvinowise.unity.actions;
using UnityEngine;


namespace rvinowise.unity {

public class Toolset_equipper: MonoBehaviour {
    
    public Baggage baggage;
    public int current_equipped_set;
    public Humanoid humanoid;
    public Action_runner action_runner;
    
    public void equip_tool_set(int set_index) {
        current_equipped_set = set_index;
        Equip_toolset.create(
            humanoid,
            baggage.tool_sets[set_index]
        ).add_marker("changing tool").start_as_root(action_runner);
    }
    
    
}

}