using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace geometry2d {

public struct Relative_direction {

    public Quaternion direction {
        get { 
            if (relative_to!=null) {
                return relative_to.rotation * local_direction;
            }
            return local_direction;
        }
        set {
            if (relative_to!=null) {
                local_direction = relative_to.rotation.inverse() * value;
            }
            local_direction = value;
        }
    }
    public Quaternion local_direction;
    public Transform relative_to;

    public Relative_direction(Quaternion in_direction) {
        relative_to = null;
        local_direction = in_direction;
    }

    public Relative_direction(Quaternion in_direction, Transform in_parent) {
        relative_to = in_parent;
        local_direction = in_direction;
    }

}
}