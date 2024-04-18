using System;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using static rvinowise.unity.geometry2d.Directions;

namespace rvinowise.unity {

public class Span_component: MonoBehaviour {
    
    public Span span;

    protected virtual void Awake() {
        span.init_for_direction(this.transform.localRotation);
    }

    
    protected void OnDrawGizmos() {
        draw_span();
        draw_direction_within_span();
    }

    private void draw_span() {
        float line_length = 0.1f;
        Gizmos.color = Color.green;
        
        var parent_rotation = Quaternion.identity;
        if (transform.parent != null){
            parent_rotation = transform.parent.transform.rotation;
        }
        var min_rotation = parent_rotation * Directions.degrees_to_quaternion(span.min);
        var max_rotation = parent_rotation * Directions.degrees_to_quaternion(span.max);
        Gizmos.DrawLine(transform.position, transform.position+min_rotation * Vector2.right * line_length);
        Gizmos.DrawLine(transform.position, transform.position+max_rotation * Vector2.right * line_length);
    }
    
    private void draw_direction_within_span() {
        float line_length = 0.05f;
        Gizmos.color = Color.cyan;
        
        Gizmos.DrawLine(transform.position, transform.position+transform.rotation * Vector2.right * line_length);
    }

}
}