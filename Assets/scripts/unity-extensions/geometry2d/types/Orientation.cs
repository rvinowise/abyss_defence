using UnityEngine;
using rvinowise.unity.extensions;



namespace rvinowise.unity.geometry2d {

public class Orientation {

    public Vector3 position;
    public Quaternion rotation;
    
    public Vector3 local_position;
    public Vector3 local_direction;

    public Transform parent;
    
    
    public Orientation() {}

    public Orientation(
        Transform in_parent
    ) 
    {
        position = Vector2.zero;
        rotation = Quaternion.identity;
        parent = in_parent;
    }

    public static Orientation from_transform(Transform in_transform) {
        return new Orientation() {
            position = in_transform.position,
            rotation = in_transform.rotation
        };
    }
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

    public override bool Equals(object o) {
        return this == o;
    }
    public override int GetHashCode() {
        return (position, rotation).GetHashCode();
    }

    public void adjust_to_parent() {
        position = parent.TransformPoint(local_position);
        rotation = ((Vector2)parent.TransformDirection(local_direction)).to_quaternion();
    }

    
}
}