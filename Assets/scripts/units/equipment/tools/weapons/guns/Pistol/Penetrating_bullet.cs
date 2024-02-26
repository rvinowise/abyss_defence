using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity {

public class Penetrating_bullet: Bullet {


    public override Polygon get_damaged_area(Ray2D in_ray) {
        return Damaging_polygons.get_splitting_wedge(in_ray);
    }

}
}