using System.Linq;

using rvinowise.unity;


namespace rvinowise.unity.actions {

public static class Changing_tools {

    public static void stop_changing_tools(
        Arm left_arm,
        Arm right_arm
    ) {
        if (left_arm.current_action?.get_root_action().marker.StartsWith("changing tool") ?? false) {
            left_arm.current_action.discard_whole_tree();
        }
        if (right_arm.current_action?.get_root_action().marker.StartsWith("changing tool") ?? false) {
            right_arm.current_action.discard_whole_tree();
        }
    } 
    
  
}
}