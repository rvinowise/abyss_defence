using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise.unity.geometry2d;
using rvinowise.contracts;


namespace rvinowise.unity {

[RequireComponent(typeof(LineRenderer))]
public class Debug_drawer : MonoBehaviour
{
    

    LineRenderer line_renderer;
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

        line_renderer = GetComponent<LineRenderer>();
        line_renderer.startWidth = 0.02f;
        line_renderer.endWidth= 0.02f;
        line_renderer.widthMultiplier = 1f;
        line_renderer.loop = true;
        
    }

    

    void Update()
    {
        Vector2 mousePos = Player_input.instance.mouse_world_position;

        /* wedge = Polygon_splitter.get_wedge_from_ray(
            new Ray2D(
                mousePos,
                new Vector2(0.5f, 1f)
            )
        );
        
        draw_polygon(wedge); */
        
    }
 
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
        line_renderer.positionCount = polygon.points.Count;
        for(int i = 0; i < polygon.points.Count; i++)
        {
            line_renderer.SetPosition(i, polygon.points[i]);
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
    
    public static void draw_polygon_gizmos(Polygon polygon) {
        if (polygon == null) {
            return;
        }
        for(int i = 0; i < polygon.points.Count-1; i++)
        {
            Gizmos.DrawLine(polygon.points[i],polygon.points[i+1]);
        }
        Gizmos.DrawLine(polygon.points.Last(), polygon.points.First());
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