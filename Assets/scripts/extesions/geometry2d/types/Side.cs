//using static Unity_extension;
using UnityEngine;

using Contract = rvinowise.rvi.contracts.Contract;

namespace geometry2d {

public class Side : Headspring.Enumeration<Side, int> {
    public static readonly Side LEFT = new Side(1, "LEFT");
    public static readonly Side RIGHT = new Side(-1, "RIGHT");
    public static readonly Side NONE = new Side(0, "NONE");
        
    private Side(int value, string displayName) : base(value, displayName) { }
        
    
    public static Side operator - (Side in_side) {
        return in_side.mirror();
    } 
    public Side mirror() {
        return Side.FromValue(this.Value * -1);
    }

    private static float epsilon = 0.1f;
    public static Side from_degrees(float degrees) {
        if (degrees > epsilon) {
            return Side.LEFT;
        } else if (degrees < -epsilon) {
            return Side.RIGHT;
        }
        return Side.NONE;
    }

    public static Side from_quaternion(Quaternion quaternion) {
        return Side.from_degrees(
            quaternion.to_degrees().use_minus().degrees
        );
    }

    public float turn_degrees(float degrees) {
        Contract.Assume(degrees >= 0, "not sure how to handle turning negative to_float_degrees into a direction");
        Contract.Assume(this != NONE, "not sure how to handle turning into Side.NONE");
        return degrees * this.Value;
    }

    public Degree turn_degree(float degrees) {
        //Contract.Assume(to_float_degrees >= 0, "not sure how to handle turning negative to_float_degrees into a direction");
        Contract.Assume(this != NONE, "not sure how to handle turning into Side.NONE");
        return new Degree(degrees * this.Value);
    }

    public Quaternion turn_quaternion(Quaternion rotation) {
        //Contract.Assume(to_float_degrees >= 0, "not sure how to handle turning negative to_float_degrees into a direction");
        Contract.Assume(this != NONE, "not sure how to handle turning into Side.NONE");
        return new Degree(rotation.eulerAngles.z * this.Value).to_quaternion();
    }

    public static float operator * (Side side, float degrees) {
        return side.turn_degrees(degrees);
    }
}
}