#ifndef POLYGON_SPLITTER_H
#define POLYGON_SPLITTER_H

#include "geometry_global.h"

/*#include <vector>
#include <list>
#include <QLineF>
#include <QPointF>
#include <QPolygonF>
#include <QVector2D>

enum class LineSide
{
    On,
    Left,
    Right,
};
*/
struct Point {
    float _x;
    float _y;

    Point(){}
    Point(float in_x, float in_y):
        _x{in_x},
        _y{in_y}
    {}
    float x() const {
        return _x;
    }
    float y() const {
        return _y;
    }
    Point operator-(Point other) const {
        return Point{
            this->x() - other.x(),
            this->y() - other.y()
        };
    }
    float lengthSquared() const {
        return _x * _x + _y * _y;
    }
};

struct Line {
    Point _pt1;
    Point _pt2;
    inline Point p1() const  {
        return _pt1;
    }
    inline Point p2() const {
        return _pt2;
    }
};

struct Polygon {
    Point *points{nullptr};
    int size{0};

    Polygon():
        size{0},
        points{nullptr}
    {}
    Polygon(int in_size):
        size{in_size}
    {
        points = new Point[in_size];
    }
    Point operator [](int i) const {
        return points[i];
    }
    int count() const {
        return size;
    }

    ~Polygon() {
        delete points;
    }

};
/*
// splits convex and (!) concanve polygons along a line.
// all sorts of evil configurations tested, except things
// like non-manifold vertices, crossing edges and alike.
class PolySplitter
{
public:
    struct PolyEdge
    {
        PolyEdge(const QPointF &startPos, LineSide side) :
            StartPos(startPos),
            StartSide(side),
            Next(nullptr),
            Prev(nullptr),
            DistOnLine(0.0f),
            IsSrcEdge(false),
            IsDstEdge(false),
            Visited(false)
        {
        }

        QPointF             StartPos;   // start position on edge
        LineSide            StartSide;  // start position's side of split line
        PolyEdge *          Next;       // next polygon in linked list
        PolyEdge *          Prev;       // previous polygon in linked list
        float               DistOnLine; // distance relative to first point on split line
        bool                IsSrcEdge;  // for visualization
        bool                IsDstEdge;  // for visualization
        bool                Visited;    // for collecting split polygons
    };

public:
    std::vector<QPolygonF>  Split(const QPolygonF &poly, const QLineF &line);

private:
    void                    SplitEdges(const QPolygonF &poly, const QLineF &line);
    void                    SortEdges(const QLineF &line);
    void                    SplitPolygon();
    std::vector<QPolygonF>  CollectPolys();
    void                    VerifyCycles() const;
    void                    CreateBridge(PolyEdge *srcEdge, PolyEdge *dstEdge);

public:
    std::list<PolyEdge>     SplitPoly;
    std::vector<PolyEdge *> EdgesOnLine;
};

extern "C"
{

// interface from C# function
GEOMETRY_EXPORT int split_polygon_two(
        Polygon polygon,
        Line line,
        Polygon** out_polygons,
        int* out_size
)
{
    QPolygonF qpolygon;
    for (size_t i_point = 0; i_point < polygon.size; i_point++) {
        qpolygon << QPointF(
                            polygon.points[i_point].x(),
                            polygon.points[i_point].y()
                            );
    }
    QLineF qline(
                line.p1().x(),
                line.p1().y(),
                line.p2().x(),
                line.p2().y()
                );

    PolySplitter polySplitter;
    std::vector<QPolygonF> pieces = polySplitter.Split(qpolygon, qline);

    *out_polygons = new Polygon[pieces.size()];
    for(size_t i_polygon = 0; i_polygon < pieces.size(); i_polygon++) {
        QPolygonF &src_polygon = pieces[i_polygon];
        Polygon &dst_polygon = *out_polygons[i_polygon];
        dst_polygon.size = src_polygon.count();
        dst_polygon.points = new Vector2[dst_polygon.size];
        for (size_t i_point = 0; i_point < src_polygon.size(); i_point++) {
            dst_polygon.points[i_point]._x = src_polygon[i_point].x();
            dst_polygon.points[i_point]._y = src_polygon[i_point].y();
        }
    }
    *out_size = pieces.size();
    return pieces.size();
}

GEOMETRY_EXPORT float add_floats_lol(
        float f1, float f2,
        float* out
)
{
    QPolygonF qpolygon;
    for (size_t i_point = 0; i_point < 5; i_point++) {
        qpolygon << QPointF(
                            i_point,
                            i_point*2
                            );
    }
    QLineF qline(
                0.1,
                0.1,
                2,
                2
                );

    *out = qline.p1().x();
    return f1 + f2;
}
*/
extern "C" {

__declspec(dllexport) int add_floats(
        Polygon polygon,
        Polygon** out_polygons
)
{
    int out_n = polygon.size+1;
    *out_polygons = new Polygon[out_n];
    for (int i=0;i<out_n;i++) {
        for (int i_point = 0; i_point < (*out_polygons)[i].size; i_point++) {
            (*out_polygons)[i].points[i_point] = Point(
                        polygon.points[i_point].x()*i,
                        polygon.points[i_point].y()*i
                        );
        }
    }
    return 1;
}




__declspec(dllexport) float add_floats_mul(float a, float b) {
    return (a + b) * 2;
}

}


#endif // POLYGON_SPLITTER_H
