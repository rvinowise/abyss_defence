using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using geometry2d;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts;
using Input = rvinowise.ui.input.Input;

[RequireComponent(typeof(LineRenderer))]
public class Debug_drawer : MonoBehaviour
{
    

    LineRenderer lineRenderer;
    bool pressed = false;
    private Divisible_body divisible_body;

    #region singleton
    public static Debug_drawer instance {
        get;
        private set;
    }
    //private static Debug_drawer _instance;
    private void init_singleton() {
        Contract.Requires(instance == null, "Several singleton instances");
        instance = this;
    }
    #endregion singleton
    
    void Awake() {
        init_singleton();

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

        /*draw_splitting_wedge(
            new Ray2D(
                mousePos,
                new Vector2(0.5f,1f)
            )
        );*/
        
        
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

    void draw_splitting_wedge(Ray2D ray) {
        Polygon wedge = Polygon_splitter.get_wedge_from_ray(ray);
        draw_polygon(wedge);
    }

    public void draw_polygon(Polygon polygon) {
        lineRenderer.positionCount = polygon.points.Count;
        for(int i = 0; i < polygon.points.Count; i++)
        {
            lineRenderer.SetPosition(i, polygon.points[i]);
        }
    }
    
    public void draw_polygon_debug(Polygon polygon, float time = 1f) {
        Color color = Color.magenta;
        for(int i = 0; i < polygon.points.Count-1; i++)
        {
            Debug.DrawLine(polygon.points[i],polygon.points[i+1], color, time);
        }
        Debug.DrawLine(polygon.points.Last(), polygon.points.First(), color, time);
    }
 
 }
