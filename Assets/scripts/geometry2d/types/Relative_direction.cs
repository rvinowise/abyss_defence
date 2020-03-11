using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace geometry2d {

public struct Relative_direction {

    public Quaternion direction;
    public Transform relative_to;

    public Relative_direction(Quaternion in_direction) {
        direction = in_direction;
        relative_to = null;
    }

    public Relative_direction(Quaternion in_direction, Transform in_parent) {
        direction = in_direction;
        relative_to = in_parent;
    }

}
}