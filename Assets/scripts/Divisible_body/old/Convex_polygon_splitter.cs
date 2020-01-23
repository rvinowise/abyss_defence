using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using geometry;

namespace geometry {

    static class Convex_polygon_splitter {
        public static List<Polygon> split_polygon_by_ray(
            Polygon polygon,
            Ray2D ray_of_split) 
        {
            
            return new List<Polygon>();
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

      
    }


    namespace collisions {
        class Lines {

            public bool find_intersection(
                Line s1, Line s2,
                out bool lines_intersect, 
                out bool segments_intersect,
                out Vector2 intersection) 
            {
                return find_intersection(
                    s1.p1, s1.p1, s2.p1, s2.p2,
                    out lines_intersect, 
                    out segments_intersect,
                    out intersection
                );
            }
            public bool find_intersection(
                Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4,
                out bool lines_intersect, 
                out bool segments_intersect,
                out Vector2 intersection)
            {
                // Get the segments' parameters.
                float dx12 = p2.x - p1.x;
                float dy12 = p2.y - p1.y;
                float dx34 = p4.x - p3.x;
                float dy34 = p4.y - p3.y;

                // Solve for t1 and t2
                float denominator = (dy12 * dx34 - dx12 * dy34);

                float t1 =
                    ((p1.x - p3.x) * dy34 + (p3.y - p1.y) * dx34)
                        / denominator;
                if (float.IsInfinity(t1))
                {
                    // The lines are parallel (or close enough to it).
                    lines_intersect = false;
                    segments_intersect = false;
                    intersection = new Vector2(float.NaN, float.NaN);
                    return false;
                }
                lines_intersect = true;

                float t2 =
                    ((p3.x - p1.x) * dy12 + (p1.y - p3.y) * dx12)
                        / -denominator;

                // Find the point of intersection.
                intersection = new Vector2(p1.x + dx12 * t1, p1.y + dy12 * t1);

                // The segments intersect if t1 and t2 are between 0 and 1.
                segments_intersect =
                    ((t1 >= 0) && (t1 <= 1) &&
                    (t2 >= 0) && (t2 <= 1));
                
                return segments_intersect;
            }
        }
    }
}