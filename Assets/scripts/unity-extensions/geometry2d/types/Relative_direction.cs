using UnityEngine;
using rvinowise.unity.extensions;



namespace rvinowise.unity.geometry2d {

public struct Relative_direction {

    public Quaternion rotation {
        get { 
            if (relative_to!=null) {
                return relative_to.rotation * local_rotation;
            }
            return local_rotation;
        }
        set {
            if (relative_to!=null) {
                local_rotation = relative_to.rotation.inverse() * value;
            }
            local_rotation = value;
        }
    }
    public Quaternion local_rotation;
    public Transform relative_to;

    public Relative_direction(Quaternion in_direction) {
        relative_to = null;
        local_rotation = in_direction;
    }

    public Relative_direction(Quaternion in_direction, Transform in_parent) {
        relative_to = in_parent;
        local_rotation = in_direction;
    }
    public Relative_direction(float in_degrees, Transform in_parent) {
        relative_to = in_parent;
        local_rotation = Directions.degrees_to_quaternion(in_degrees);
    }

    public Relative_direction adjust_to_side_for_left(Side in_side) {
        if (in_side == Side.RIGHT) {
            local_rotation = Quaternion.Inverse(local_rotation);
        }
        return this;
    }

}
}