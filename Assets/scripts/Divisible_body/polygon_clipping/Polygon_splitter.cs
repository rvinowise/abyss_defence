using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;

using UnityEditor;
using rvinowise.unity.geometry2d;
using ClipperLib;
using static rvinowise.unity.geometry2d.Clipperlib_coordinates;

namespace rvinowise.unity.geometry2d {

    using Path = List<IntPoint>;
    using Pathes = List<List<IntPoint>>;

    public static class Polygon_splitter {
        public static List<Polygon> split_polygon_by_ray(
            Polygon polygon,
            Ray2D ray_of_split) 
        {
            return remove_polygon_from_polygon(
                polygon, 
                get_wedge_from_ray(ray_of_split)
            );
        }
        
        public static List<Polygon> remove_polygon_from_polygon(
            Polygon base_polygon,
            Polygon removed_polygon) 
        {
            Path int_base_polygon = float_coord_to_int(base_polygon);
            Path int_removed_polygon = float_coord_to_int(removed_polygon);

            Pathes int_solution = new Pathes();
            ClipperLib.Clipper clipper = new ClipperLib.Clipper();
            clipper.AddPath(int_base_polygon, PolyType.ptSubject, true);
            clipper.AddPath(int_removed_polygon, PolyType.ptClip, true);
            clipper.Execute(ClipType.ctDifference, int_solution);

            List<Polygon> result = int_coord_to_float(int_solution);

            return result;
        }

        static void log(Polygon[] polygons) {
            Debug.Log("polygons qty= "+polygons.Length);
            int i_polygon = 1;
            foreach (Polygon polygon in polygons) {
                Debug.Log("Polygon# "+i_polygon+" points= "+polygon.points.Count);
                for(int i_point = 0; i_point<polygon.points.Count; i_point++) {
                    Vector2 point = polygon.points[i_point];
                    Debug.Log("-- Point# "+i_point+" x= "+point.x+" y= "+point.y);
                }
                i_polygon++;
            }
        }


        static public Polygon get_wedge_from_ray(Ray2D ray_of_split) {
            Polygon wedge_of_split = new Polygon(new Vector2[] {
                ray_of_split.origin + (ray_of_split.direction.rotate(-90f) * 0.01f),
                ray_of_split.origin + (ray_of_split.direction * 10f),
                ray_of_split.origin + (ray_of_split.direction.rotate(90f) * 0.01f),
                ray_of_split.origin - (ray_of_split.direction * 1f)
            });
            return wedge_of_split;
        }

        

    }


}