using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace geometry2d {

public class Polygon {
    public List<Vector2> points;


    public Polygon(int capacity = 0) {
        points = new List<Vector2>(capacity);
    }
    public Polygon(Polygon src) {
        this.points = new List<Vector2>(src.points);
    }

    public Polygon(Vector2[] vectors) {
        points = new List<Vector2>(vectors);
    }

/*    static public Polygon borrow(Vector2[] vectors) {
        Polygon polygon = new Polygon();
        polygon.points = vectors;
        return polygon;
    }*/

    public Vector2 middle {
        get {
            if (float.IsNaN(_middle.x)) {
                _middle = calculate_middle();
            }
            return _middle;
        }
        private set {
            _middle = value;
        }
    }
    private Vector2 _middle = new Vector2(float.NaN,float.NaN);
    private Vector2 calculate_middle() {
        Vector2 middle = new Vector2(0f,0f);
        foreach(Vector2 point in points) {
            middle += point;
        }
        middle /= points.Count;
        return middle;
    }

    public void scale(float scale) {
        move(-middle);
        scale_relative_to_zero(scale);
        move(middle); // it's not obvious that Middle wasn't changed in previous functions
    }
    public void move(Vector2 offset) {
        for(int i_point=0;i_point < points.Count;i_point++) {
            points[i_point] += offset;
        }
    }
    private void scale_relative_to_zero(float scale) {
        for(int i_point=0;i_point < points.Count;i_point++) {
            points[i_point] *= scale;
        }
    }
}

}