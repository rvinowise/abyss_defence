using rvinowise.unity.actions;
using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity {

public static class Arm_pair_helpers {


    public static Arm get_arm_on_side(Arm_pair arm_pair, Side_type in_side) {
        if (in_side == Side_type.LEFT) {
            return arm_pair.left_arm;
        } else if (in_side == Side_type.RIGHT) {
            return arm_pair.right_arm;
        }
        return null;
    }
    
}

}