using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.parts.weapons.guns.common;


namespace units.equipment.tools.weapons.guns.Pistol {

public class Penetrating_bullet: Bullet {


    public override Polygon get_damaged_area(Ray2D in_ray) {
        Polygon wedge_of_split = new Polygon(new Vector2[] {
            in_ray.origin + (in_ray.direction.rotate(-90f) * 0.01f),
            in_ray.origin + (in_ray.direction * 10f),
            in_ray.origin + (in_ray.direction.rotate(90f) * 0.01f),
            in_ray.origin - (in_ray.direction * 1f)
        });
        return wedge_of_split; 
    }

}
}