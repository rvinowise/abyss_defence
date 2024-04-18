using UnityEngine;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity {

public class Hollow_bulet: Bullet {
    
    
    private static Polygon circle;

    static Hollow_bulet() {
        init_circle_polygon();
    }

    private static void init_circle_polygon() {
        const int points_n = 10;
        const float radius = 0.2f;
        const float angle_step = 360 / points_n;
        circle = new Polygon(points_n);
        for (var i=0;i<points_n;i++) {
            circle.points.Add(
                Directions.degrees_to_quaternion(angle_step * i) *
                Vector2.right *
                radius
            );
        }
    }
    public override Polygon get_damaged_area(Ray2D in_ray) {
        Polygon damaged_area = circle.get_moved(in_ray.origin);
        return damaged_area;    
    }

}
}