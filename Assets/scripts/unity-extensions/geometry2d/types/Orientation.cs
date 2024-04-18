using UnityEngine;
using rvinowise.unity.extensions;



namespace rvinowise.unity.geometry2d {

public struct Orientation {

    public Vector3 position;
    public Quaternion rotation;
    
    public Transform parent;


    public Orientation(
        Transform in_parent
    ) 
    {
        position = Vector2.zero;
        rotation = Quaternion.identity;
        parent = in_parent;
    }

    public static Orientation from_transform(Transform in_transform) {
        return new Orientation {
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
        parent = null;
    }

    public static bool operator ==(Orientation obj1, Orientation obj2) {
        return (
            (obj1.position == obj2.position) &&
            (obj1.rotation == obj2.rotation)
        );
    }
    public static bool operator !=(Orientation obj1, Orientation obj2) {
        return !(obj1 == obj2);
    }

    public override bool Equals(object o) {
        return this == (Orientation)o;
    }
    public override int GetHashCode() {
        return (position, rotation).GetHashCode();
    }

    public Orientation adjust_to_parent() {
        return new Orientation {
            position = parent.TransformPoint(position),
            rotation = ((Vector2) parent.TransformDirection(rotation.to_vector())).to_quaternion()
        };
    }
}

    
}
