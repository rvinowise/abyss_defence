﻿using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

/* ClipperLib uses another coordinate system, 
 this class translates it from game coordinates */
namespace rvinowise.unity.geometry2d
{
using Path = List<IntPoint>;
using Pathes = List<List<IntPoint>>;
public static class Clipperlib_coordinates
{
    private static float float_int_multiplier = 10000;

    public static ClipperLib.IntPoint float_coord_to_int(Vector2 vector) {
        return new ClipperLib.IntPoint(
            vector.x * float_int_multiplier,
            vector.y * float_int_multiplier
        );
    }
    public static Path float_coord_to_int(Polygon float_polygon) {
        Path int_polygon = new Path(float_polygon.points.Count);
        foreach (Vector2 vector in float_polygon.points) {
            int_polygon.Add(float_coord_to_int(vector));
        }
        return int_polygon;
    }

    public static List<Polygon> int_coord_to_float(Pathes int_solution) {
        List<Polygon> float_polygons = new List<Polygon>(int_solution.Count);
        foreach (Path int_polygon in int_solution) {
            float_polygons.Add(new Polygon(int_polygon.Count));
            foreach (ClipperLib.IntPoint int_point in int_polygon) {
                float_polygons[float_polygons.Count-1].points.Add(new Vector2(
                    int_point.X / float_int_multiplier,
                    int_point.Y / float_int_multiplier
                ));
            }
        }
        return float_polygons;
    }
}
}
