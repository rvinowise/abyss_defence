using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace geometry2d {

public class Orientation {

    public Vector2 position;
    public Quaternion rotation;

    public Transform parent;
    
    
    public Orientation() {}

    public Orientation(
        Vector2 in_position, 
        Quaternion in_rotation,
        Transform in_parent) 
    {
        position = in_position;
        rotation = in_rotation;
        parent = in_parent;
    }
    public Orientation(Vector2 in_position, Quaternion in_rotation) {
        position = in_position;
        rotation = in_rotation;
    }

    public static bool operator ==(Orientation obj1, Orientation obj2) {
        if (obj1 == null || obj2 == null) {
            return false;
        }
        return (
            (obj1.position == obj2.position) &&
            (obj1.rotation == obj2.rotation)
        );
    }
    public static bool operator !=(Orientation obj1, Orientation obj2) {
        return !(obj1 == obj2);
    }
}
}