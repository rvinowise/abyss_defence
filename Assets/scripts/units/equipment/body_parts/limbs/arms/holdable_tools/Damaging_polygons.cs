using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine;


namespace rvinowise.unity {

public static class Damaging_polygons {
    
    public static Polygon get_splitting_wedge(Ray2D in_ray) {
        Polygon wedge_of_split = new Polygon(new[] {
            in_ray.origin + in_ray.direction.rotate(-90f) * 0.01f,
            in_ray.origin + in_ray.direction * 10f,
            in_ray.origin + in_ray.direction.rotate(90f) * 0.01f,
            in_ray.origin - in_ray.direction * 1f
        });
        return wedge_of_split; 
    }
    
    
    private static Polygon circle;

    private static void create_circle_polygon() {
        int points_n = 5;
        float radius = 0.1f;
        float angle_step = 360f / points_n;
        circle = new Polygon(points_n);
        for (int i=0;i<points_n;i++) {
            circle.points.Add(
                Directions.degrees_to_quaternion(angle_step * i) *
                Vector2.right *
                radius
            );
        }
    }    

}

}