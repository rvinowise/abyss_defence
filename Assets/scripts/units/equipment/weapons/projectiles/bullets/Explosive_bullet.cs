using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;


namespace rvinowise.unity {

public class Explosive_bullet: MonoBehaviour {

    public PolygonCollider2D damage_area_editor;
    //private static Polygon polygon;

    /*static Explosive_bullet() {
        init_damaging_polygon();
    }*/

    /*private static void init_damaging_polygon() {
        int points_n = 4;
        float inner_radius = 0.2f;
        float outer_radius = 1.0f;
        float angle_step = 90 / points_n;
        polygon = new Polygon(points_n);
        
        polygon.points.Add(Vector2.zero);
        for (int i=0;i<points_n;i++) {
            
            polygon.points.Add(
                Directions.degrees_to_quaternion(angle_step * i) *
                Vector2.right *
                outer_radius
            );
            
            polygon.points.Add(
                Directions.degrees_to_quaternion(angle_step * i) *
                Vector2.right *
                inner_radius
            );
        }
        
    }*/
    public Polygon get_damaged_area(Ray2D in_ray) {

        Polygon damaged_area = new Polygon(
            damage_area_editor.GetPath(0)
        );
        damaged_area.
            rotate(in_ray.direction.to_quaternion()).
            move(in_ray.origin);

        return damaged_area;    
    }
    
    

}
}