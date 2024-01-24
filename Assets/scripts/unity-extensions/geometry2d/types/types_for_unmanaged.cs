using System.Runtime.InteropServices;
using UnityEngine;


namespace rvinowise.unity.geometry2d {


namespace for_unmanaged {
    /* structs for interface to C++ - they are mapped to the unmanaged memory */

    [StructLayout(LayoutKind.Sequential)]
    public struct Point {
        public float x;
        public float y;

        public Point(Vector2 vector) {
            x = vector.x;
            y = vector.y;
        }

        public Vector2 to_Vector2() {
            return new Vector2(this.x, this.y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Polygon {
        public Point[] points;
        public int size;

        public Polygon(Vector2[] vectors) {
            points = new Point[vectors.Length];
            for(int i=0; i<vectors.Length; i++) {
                points[i] = new Point(vectors[i]);
            }
            size = vectors.Length;
        }
        public Polygon(Point[] in_points) {
            points = new Point[in_points.Length];
            for(int i=0; i<in_points.Length; i++) {
                points[i] = in_points[i];
            }
            size = in_points.Length;
        }

        public Vector2[] vectors2() {
            Vector2[] out_vectors2 = new Vector2[points.Length];
            for(int i=0; i<points.Length; i++) {
                out_vectors2[i] = points[i].to_Vector2();
            }
            return out_vectors2;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Line {
        public Point p1;
        public Point p2;

        public Line(Point _p1, Point _p2) {
            p1 = _p1;
            p2 = _p2;
        }
        public Line(Vector2 _p1, Vector2 _p2) {
            p1 = new Point(_p1);
            p2 = new Point(_p2);
        }
    }
}

}