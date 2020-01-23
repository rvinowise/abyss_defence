using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class Divisible_body : MonoBehaviour
{
    public void test_create_path() {
        PolygonCollider2D collider = gameObject.GetComponent<PolygonCollider2D>();
        Vector2[] points =  { 
            new Vector2(0.2f,0f),
            new Vector2(0.2f,0.2f),
            new Vector2(0.25f,0.15f),
            new Vector2(0.30f,0.2f),
            new Vector2(0.30f,-0.05f),
            new Vector2(0.2f,-0.05f)
            };
        Debug.Log("paths:"+collider.pathCount);
        //collider.SetPath(collider.pathCount-1, points);
        collider.points = points;
    }
}