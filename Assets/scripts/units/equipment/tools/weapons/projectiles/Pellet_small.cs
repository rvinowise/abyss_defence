using UnityEngine;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity.units.parts.weapons.guns.common {

public class Pellet_small : Pellet {
    private static Polygon circle;

    static Pellet_small() {
        init_circle_polygon();
    }

    private static void init_circle_polygon() {
        int points_n = 5;
        float radius = 0.1f;
        float angle_step = 360 / points_n;
        circle = new Polygon(points_n);
        for (int i=0;i<points_n;i++) {
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