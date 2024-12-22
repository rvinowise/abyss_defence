using System.Linq;

using rvinowise.unity;
using rvinowise.unity.extensions;


namespace rvinowise.unity.actions {

public static class Find_best_arm_for_using_tool {

    // private Humanoid user;
    // private Arm left_arm;
    // private Arm right_arm;
    // private Baggage baggage;
    //
    // public IGun gun;
    // public Tool tool;
    
    

    public static Arm get_best_arm_for_supertool(Baggage baggage, Arm_pair arm_pair) {
        var arm = arm_holding_supertool(baggage, arm_pair);
        if (arm == null) {
            return get_hand_closest_to_baggage(baggage, arm_pair);
        }
        return arm;
    }
    
    public static Arm get_hand_closest_to_baggage(Baggage baggage, Arm_pair arm_pair) {
        var distance_right = arm_pair.right_arm.hand.transform.sqr_distance_to(baggage.transform.position);
        var distance_left = arm_pair.left_arm.hand.transform.sqr_distance_to(baggage.transform.position);
        if (distance_left < distance_right) {
            return arm_pair.left_arm;
        }
        return arm_pair.right_arm;
    }

    public static Arm arm_holding_supertool(Baggage baggage, Arm_pair arm_pair) {
        if (is_arm_holding_supertool(baggage, arm_pair.right_arm)) {
            return arm_pair.right_arm;
        }
        if (is_arm_holding_supertool(baggage, arm_pair.left_arm)) {
            return arm_pair.left_arm;
        }
        return null;
    }

    public static bool is_arm_holding_supertool(Baggage baggage, Arm arm) {
        foreach (var supertool in baggage.supertools) {
            if (arm.held_tool == supertool) {
                return true;
            }
        }
        return false;
    }

   
    

    
  
}
}