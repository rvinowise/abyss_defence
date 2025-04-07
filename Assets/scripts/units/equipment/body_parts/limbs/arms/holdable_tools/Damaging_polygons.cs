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
    
    public static Polygon get_damaging_circle(Ray2D ray) {
        return circle.get_moved(ray.origin); 
    }
    
    
    public static Polygon circle;

    private static void create_circle_polygon() {
        int points_n = 10;
        float radius = 0.2f;
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

    static Damaging_polygons() {
        create_circle_polygon();
    }

}

}