using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;

public class Hideable_leg : MonoBehaviour {

    public Degree hiding_direction;
    //public Quaternion hiding_rotation;
    public float hiding_depth;
    public float pulling_speed;
    
    protected void OnDrawGizmos() {
        float line_length = 0.15f;
        Gizmos.color = Color.cyan;
        
        var parent_rotation = Quaternion.identity;
        if (transform.parent != null){
            parent_rotation = transform.parent.transform.rotation;
        }
        var hiding_rotation = parent_rotation * Directions.degrees_to_quaternion(hiding_direction);
        Gizmos.DrawLine(transform.position, transform.position + hiding_rotation * Vector2.right * hiding_depth);
    }
}
