using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_monitor : MonoBehaviour
{
    private Vector2[] points;

    public void update_gizmos() {
        PolygonCollider2D collider = gameObject.GetComponent<PolygonCollider2D>();
        if (collider) {
            //points = collider.points;
        }
    }

    void FixedUpdate() {
        if (points == null || points.Length == 0) {
            return;
        }
		// for every point (except for the last one), draw line to the next point
		for(int i = 0; i < points.Length-1; i++)
		{
			Debug.Log("OnDrawGizmos draw point №"+i );
            Debug.DrawLine(
                transform.TransformPoint(points[i]), 
                transform.TransformPoint(points[i+1]),
                Color.green
            );
		}
        Debug.DrawLine(
            transform.TransformPoint(points[0]), 
            transform.TransformPoint(points[points.Length-1]),
            Color.green
        );
	}
}
