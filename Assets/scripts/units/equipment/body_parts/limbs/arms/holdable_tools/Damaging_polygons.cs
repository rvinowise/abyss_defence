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
}

}