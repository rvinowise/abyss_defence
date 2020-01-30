using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace geometry2d {

using Point = Vector2;

public class Line {
    public Vector2 p1;
    public Vector2 p2;

    public Line(Vector2 _p1, Vector2 _p2) {
        p1 = _p1;
        p2 = _p2;
    }
}

public class Polygon {
    public List<Vector2> points;

    public Polygon() {
        points = new List<Vector2>();
    }
    public Polygon(Vector2[] vectors) {
        points = new List<Vector2>(vectors);
    }
}



}