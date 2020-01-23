using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;
using UnityEditor;
using geometry;
using ClipperLib;


namespace geometry {

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    static class Polygon_splitter {
        public static List<Polygon> split_polygon_by_ray(
            Polygon polygon,
            Ray2D ray_of_split) 
        {
            Path int_polygon = float_coord_to_int(polygon);
            Path int_wedge_of_split = float_coord_to_int(get_wedge_from_ray(ray_of_split));

            Paths int_solution = new Paths();
            ClipperLib.Clipper clipper = new ClipperLib.Clipper();
            clipper.AddPath(int_polygon, PolyType.ptSubject, true);
            clipper.AddPath(int_wedge_of_split, PolyType.ptClip, true);
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
                ray_of_split.origin,
                ray_of_split.origin - (ray_of_split.direction * 0.1f),
                ray_of_split.origin+ (ray_of_split.direction.Rotate(-90f) * 0.01f),
                //new Vector2(0f,0f)
                ray_of_split.origin + (ray_of_split.direction * 0.3f)
            });
            return wedge_of_split;
        }

        static float float_int_multiplier = 10000;

        static ClipperLib.IntPoint float_coord_to_int(Vector2 vector) {
            return new ClipperLib.IntPoint(
                vector.x * float_int_multiplier,
                vector.y * float_int_multiplier
            );
        }
        static Path float_coord_to_int(Polygon float_polygon ) {
            Path int_polygon = new Path();
            foreach (Vector2 vector in float_polygon.points) {
                int_polygon.Add(float_coord_to_int(vector));
            }
            return int_polygon;
        }

        static List<Polygon> int_coord_to_float(Paths int_solution) {
            List<Polygon> float_polygons = new List<Polygon>();
            foreach (Path int_polygon in int_solution) {
                float_polygons.Add(new Polygon());
                foreach (ClipperLib.IntPoint int_point in int_polygon) {
                    float_polygons.Last().points.Add(new Vector2(
                        int_point.X / float_int_multiplier,
                        int_point.Y / float_int_multiplier
                    ));
                }
            }
            return float_polygons;
        }

    }


}