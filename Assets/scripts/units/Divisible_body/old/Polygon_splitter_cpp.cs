using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using rvinowise.unity.geometry2d.for_unmanaged;

namespace geometry_cpp {

    
    static class Polygon_splitter {

        [DllImport("geometry", CallingConvention = CallingConvention.Cdecl)]
        private static extern int split_polygon2(
            Polygon polygon,
            Line line,
            ref Polygon[] pieces,
            ref int n_pieces
            );


        public static List<Polygon> split_polygon_by_ray(
            Polygon polygon,
            Ray2D ray_of_split) 
        {
            Line line_of_split = new Line(
                ray_of_split.origin,
                ray_of_split.origin + (ray_of_split.direction * 10)
            );

            Point[] points = new Point[3]{
                new Point(new Vector2(1f, 2f)),
                new Point(new Vector2(1.1f, 2.1f)),
                new Point(new Vector2(1.2f, 2.2f))
            };
            Polygon in_polygon = new Polygon(points);

            Polygon[] out_polygons = new Polygon[4] {
                new Polygon(points),
                new Polygon(points),
                new Polygon(points),
                new Polygon(points)
            };
            int res_ret = add_floats(in_polygon, ref out_polygons);

            log(out_polygons);
            
            return new List<Polygon>();
        }

        static void log(Polygon[] polygons) {
            Debug.Log("polygons qty= "+polygons.Length);
            int i_polygon = 1;
            foreach (Polygon polygon in polygons) {
                Debug.Log("Polygon# "+i_polygon+" points= "+polygon.points.Length);
                for(int i_point = 0; i_point<polygon.points.Length; i_point++) {
                    Point point = polygon.points[i_point];
                    Debug.Log("-- Point# "+i_point+" x= "+point.x+" y= "+point.y);
                }
                i_polygon++;
            }
        }

        [DllImport("geometry", CallingConvention = CallingConvention.Cdecl)]
        private static extern int add_floats(Polygon polygon, ref Polygon[] out_polygons);

         [DllImport("geometry", CallingConvention = CallingConvention.Cdecl)]
        private static extern float add_floats_lol(float f1, float f2, ref float res);

        [DllImport("geometry", CallingConvention = CallingConvention.Cdecl)]
        private static extern float add_floats_mul(float f1, float f2);

        [DllImport("ASimplePlugin", CallingConvention = CallingConvention.Cdecl)]
        private static extern float AddTwoFloats_mul(float f1, float f2);
    }

    

    namespace collisions {
        /*class Lines {

            public bool find_intersection(
                Segment s1, Segment s2,
                out bool lines_intersect, 
                out bool segments_intersect,
                out Vector2 intersection) 
            {
                return find_intersection(
                    s1.p1, s1.p1, s2.p1, s2.p2,
                    out bool lines_intersect, 
                    out bool segments_intersect,
                    out Vector2 intersection
                );
            }
            public bool find_intersection(
                Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4,
                out bool lines_intersect, 
                out bool segments_intersect,
                out Vector2 intersection)
            {
                // Get the segments' parameters.
                float dx12 = p2.X - p1.X;
                float dy12 = p2.Y - p1.Y;
                float dx34 = p4.X - p3.X;
                float dy34 = p4.Y - p3.Y;

                // Solve for t1 and t2
                float denominator = (dy12 * dx34 - dx12 * dy34);

                float t1 =
                    ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                        / denominator;
                if (float.IsInfinity(t1))
                {
                    // The lines are parallel (or close enough to it).
                    lines_intersect = false;
                    segments_intersect = false;
                    intersection = new Vector2(float.NaN, float.NaN);
                    close_p1 = new Vector2(float.NaN, float.NaN);
                    close_p2 = new Vector2(float.NaN, float.NaN);
                    return;
                }
                lines_intersect = true;

                float t2 =
                    ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                        / -denominator;

                // Find the point of intersection.
                intersection = new Vector2(p1.X + dx12 * t1, p1.Y + dy12 * t1);

                // The segments intersect if t1 and t2 are between 0 and 1.
                segments_intersect =
                    ((t1 >= 0) && (t1 <= 1) &&
                    (t2 >= 0) && (t2 <= 1));
                
                return segments_intersect;
            }
        }*/
    }
}