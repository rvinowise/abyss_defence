//using static Unity_extension;

using System;
using UnityEngine;
using rvinowise.unity.extensions;



using Contract = rvinowise.contracts.Contract;

namespace rvinowise.unity.geometry2d {

[Serializable]
public enum Side_type: int {
    LEFT=1,
    RIGHT=-1,
    NONE=0
}


public static class Side  {
        
   

    public static Side_type mirror(Side_type side) {
        return (Side_type)((int)side * -1);
    }

    public static Side_type from_degrees(float degrees) {
        if (degrees > Turning_element.rotation_epsilon) {
            return Side_type.LEFT;
        }
        if (degrees < -Turning_element.rotation_epsilon) {
            return Side_type.RIGHT;
        }
        return Side_type.NONE;
    }

    public static Side_type from_quaternion(Quaternion quaternion) {
        return Side.from_degrees(
            quaternion.to_degree().use_minus().degrees
        );
    }

    public static float turn_degrees(Side_type side, float degrees) {
        Contract.Assume(degrees >= 0, "not sure how to handle turning negative to_float_degrees into a direction");
        //Contract.Assume(side != Side_type.NONE, "not sure how to handle turning into Side.NONE");
        return degrees * (int)side;
    }

    public static Degree turn_degree(Side_type side, float degrees) {
        Contract.Assume(side != Side_type.NONE, "not sure how to handle turning into Side.NONE");
        return new Degree(degrees * (int)side);
    }

   
 

    public static Side_type flipped(Side_type side) {
        if (side==Side_type.NONE) {
            return Side_type.NONE;
        }
        return (side == Side_type.LEFT) ? Side_type.RIGHT : Side_type.LEFT;
    }
}
}