#ifndef POLYGON_SPLITTER_H
#define POLYGON_SPLITTER_H

#include "geometry_global.h"

#include <vector>
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

struct Vector2 {
    float _x;
    float _y;

    float x() const {
        return _x;
    }
    float y() const {
        return _y;
    }
    Vector2 operator-(Vector2 other) const {
        return Vector2{
            this->x() - other.x(),
            this->y() - other.y()
        };
    }
    float lengthSquared() const {
        return _x * _x + _y * _y;
    }
};

struct Line {
    Vector2 _pt1;
    Vector2 _pt2;
    inline Vector2 p1() const  {
        return _pt1;
    }
    inline Vector2 p2() const {
        return _pt2;
    }
};

struct Polygon {
    Vector2 *points;
    size_t size;

    Vector2 operator [](int i) const {
        return points[i];
    }
    size_t count() const {
        return size;
    }

    ~Polygon() {
        delete points;
    }
};

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

// interface from C# function
GEOMETRY_EXPORT size_t split_polygon(
        Polygon polygon,
        Line line,
        Polygon** out_polygons,
        size_t* out_size
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

#endif // POLYGON_SPLITTER_H
