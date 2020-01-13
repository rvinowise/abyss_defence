using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace geometry {

    using Polygon = List<Vector2>;
    using Segment = Tuple<Vector2, Vector2>;
    
    class Polygon_divider {

        public List<Polygon> divide_polygon(
            Vector2[] points,
            Vector2 position_of_split,
            Vector2 direction_of_split) 
        {
            return divide_polygon(new List<Vector2>(points),position_of_split,direction_of_split);
        }
        public List<Polygon> divide_polygon(
            List<Vector2> points,
            Vector2 position_of_split,
            Vector2 direction_of_split) 
        {
            for (int i_point = 0; i_point < points.Length; i_point++) {
                if (
                    find_intersection()
                    )
                {

                }
            }
        }
    }

    namespace collisions {
        class Lines {

            public bool find_intersection(
                Segment s1, Segment s2,
                out bool lines_intersect, 
                out bool segments_intersect,
                out Vector2 intersection) 
            {
                return find_intersection(
                    s1.Item1, s1.Item2, s2.Item1, s2.Item2,
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
        }
    }
}