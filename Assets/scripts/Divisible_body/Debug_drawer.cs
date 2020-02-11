using System.Collections.Generic;
using UnityEngine;
using geometry2d;

[RequireComponent(typeof(LineRenderer))]
public class Debug_drawer : MonoBehaviour
{
    
    public GameObject divided;

    LineRenderer lineRenderer;
    bool pressed = false;
    Ray2D ray;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth= 0.02f;
        lineRenderer.widthMultiplier = 1f;
        lineRenderer.loop = true;
         //lineRenderer.SetVertexCount(linePoints.Count);
    }
 
    void Update()
    {
        Vector2 mousePos = rvi.Input.mouse_world_position();
        ray = new Ray2D(
            mousePos,
            new Vector2(0.5f,1f)
        );

        draw_splitting_wedge();
        //mouse_control();
        
        
    }
 
    void mouse_control() {
        if (
            (!pressed)&&
            (Input.GetMouseButtonDown(0))
           ) 
        {
            pressed = true;
            //Divisible_body divisible_body = divided.GetComponent<Divisible_body>(); 
            //divisible_body.split_by_ray(ray);
        }

        if (
            (pressed)&&
            (!Input.GetMouseButtonDown(0))
           ) 
        {
            pressed = false;
        }
    }
    
    void draw_splitting_wedge() {
        Polygon wedge = Polygon_splitter.get_wedge_from_ray(ray);
        draw_polygon(wedge);
    }

    void draw_polygon(Polygon polygon) {
        lineRenderer.positionCount = polygon.points.Count;
        for(int i = 0; i < polygon.points.Count; i++)
        {
            lineRenderer.SetPosition(i, polygon.points[i]);
        }
    }
 
 }
