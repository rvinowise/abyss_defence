using System.Collections.Generic;
using UnityEngine;
using geometry;

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
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth= 0.01f;
        lineRenderer.widthMultiplier = 1f;
        lineRenderer.loop = true;
         //lineRenderer.SetVertexCount(linePoints.Count);
    }
 
    void Update()
    {
        draw_splitting_wedge();
        mouse_control();
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ray = new Ray2D(
                mousePos,
                new Vector2(0.5f,1f)
            );
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
        lineRenderer.positionCount = wedge.points.Count;
        for(int i = 0; i < wedge.points.Count; i++)
        {
            lineRenderer.SetPosition(i, wedge.points[i]);
        }
    }
 
 }
