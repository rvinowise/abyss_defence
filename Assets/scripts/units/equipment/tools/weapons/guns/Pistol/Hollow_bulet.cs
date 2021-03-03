using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.weapons.guns.common;


namespace units.equipment.tools.weapons.guns.Pistol {

public class Hollow_bulet: Bullet {
    
    
    private static Polygon circle;

    static Hollow_bulet() {
        init_circle_polygon();
    }

    private static void init_circle_polygon() {
        int points_n = 10;
        float radius = 0.2f;
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