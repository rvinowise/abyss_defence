using System.Collections.Generic;
using UnityEngine;
using geometry2d;
using rvinowise;
using rvinowise.units.parts;
using Input = rvinowise.ui.input.Input;

[RequireComponent(typeof(LineRenderer))]
public class Debug_drawer : MonoBehaviour
{
    

    LineRenderer lineRenderer;
    bool pressed = false;
    Ray2D ray;
    private Divisible_body divisible_body;
    
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth= 0.02f;
        lineRenderer.widthMultiplier = 1f;
        lineRenderer.loop = true;
        
        divisible_body = gameObject.GetComponent<Divisible_body>(); 

         //lineRenderer.SetVertexCount(linePoints.Count);
    }
 
    void Update()
    {
        Vector2 mousePos = Input.instance.mouse_world_position;
        ray = new Ray2D(
            mousePos,
            new Vector2(0.5f,1f)
        );

        draw_splitting_wedge();
        //mouse_control();
        
        
    }
 
    /*void mouse_control() {
        if (
            (!pressed)&&
            (UnityEngine.Input.GetMouseButtonDown(0))
           ) 
        {
            pressed = true;
            divisible_body.split_by_ray(ray);
        }

        if (
            (pressed)&&
            (!UnityEngine.Input.GetMouseButtonDown(0))
           ) 
        {
            pressed = false;
        }
    }*/

    /*void UnityApiMouseEvents() {
        RaycastHit hit;
        if (Physics.Raycast(hit)) {
            if (hit.rigidbody != null)
                hit.rigidbody.gameObject.SendMessage("OnMouseDown");
            else
                hit.collider.SendMessage("OnMouseDown");
        }
    }*/

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
