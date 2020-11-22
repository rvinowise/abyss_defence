using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.unity.units.parts;
using Input = rvinowise.unity.ui.input.Input;
using Debug = UnityEngine.Debug;

namespace rvinowise.unity.debug {

[RequireComponent(typeof(LineRenderer))]
public class Debug_drawer : MonoBehaviour
{
    

    LineRenderer lineRenderer;
    bool pressed = false;
    private Polygon wedge;

    #region singleton
    public static Debug_drawer instance {
        get;
        private set;
    }
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
        
    }

    

    /* void Update()
    {
        Vector2 mousePos = Input.instance.mouse_world_position;

        wedge = Polygon_splitter.get_wedge_from_ray(
            new Ray2D(
                mousePos,
                new Vector2(0.5f, 1f)
            )
        );
        
        //draw_polygon(wedge);
        
    } */
 
    /* void mouse_control() {
        if (
            (!pressed)&&
            (UnityEngine.Input.GetMouseButtonDown(0))
           ) 
        {
            pressed = true;
            divisible_body?.remove_polygon(wedge);
        }

        if (
            (pressed)&&
            (!UnityEngine.Input.GetMouseButtonDown(0))
           ) 
        {
            pressed = false;
        }
    } */
    
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
            UnityEngine.Debug.DrawLine(polygon.points[i],polygon.points[i+1], color, time);
        }
         UnityEngine.Debug.DrawLine(polygon.points.Last(), polygon.points.First(), color, time);
    }

    public void draw_collision(Ray2D in_ray, float time = 1f) {
        Polygon collision_polygon = new Polygon(new Vector2[] {
            in_ray.origin + (in_ray.direction.rotate(-90f) * 0.02f),
            in_ray.origin + (in_ray.direction * 0.02f),
            in_ray.origin + (in_ray.direction.rotate(90f) * 0.02f),
            in_ray.origin - (in_ray.direction * 0.5f)
        });
        draw_polygon_debug(collision_polygon, time);
    }
 
}

}