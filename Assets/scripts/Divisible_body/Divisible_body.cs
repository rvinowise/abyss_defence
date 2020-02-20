using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using geometry2d;
using rvinowise.units;
using Tool = rvinowise.units.Tool;


namespace rvinowise.units.equipment {

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Divisible_body : MonoBehaviour {
    public Sprite inside;


    public void split_by_ray(Ray2D ray_of_split) {

        List<Polygon> collider_pieces = Polygon_splitter.split_polygon_by_ray(
            new Polygon(gameObject.GetComponent<PolygonCollider2D>().GetPath(0)),
            transform.InverseTransformRay(ray_of_split)
        );

        Sprite body = gameObject.GetComponent<SpriteRenderer>().sprite;

        IEnumerable<GameObject> piece_objects = create_objects_for_colliders(
            collider_pieces, body, inside
        );

        Tools_splitter.split_controllers_of_tools(gameObject, piece_objects);
        Destroy(gameObject);
    }

    private IEnumerable<GameObject> create_objects_for_colliders(
        IEnumerable<Polygon> collider_pieces,
        Sprite body,
        Sprite inside
    ) {
        List<GameObject> piece_objects = new List<GameObject>();
        foreach (Polygon collider_piece in collider_pieces) {
            Texture2D texture_piece;
            if (inside) {
                texture_piece =
                    Texture_splitter.create_texture_with_insides_for_polygon(
                        body,
                        inside,
                        collider_piece
                    );
            }
            else {
                texture_piece =
                    Texture_splitter.create_texture_for_polygon(
                        body,
                        collider_piece
                    );
            }

            piece_objects.Add(
                create_gameobject_from_polygon_and_texture(
                    collider_piece, texture_piece
                )
            );
        }
        return piece_objects;
    }

    private GameObject create_gameobject_from_polygon_and_texture(
        Polygon polygon, Texture2D texture) {
        GameObject created_part = Instantiate(
            gameObject, transform.position, transform.rotation);

        Sprite body_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            body_sprite.to_texture_coordinates(body_sprite.pivot),
            body_sprite.pixelsPerUnit
        );

        created_part.GetComponent<PolygonCollider2D>().SetPath(0, polygon.points.ToArray());
        created_part.GetComponent<SpriteRenderer>().sprite = sprite;
        return created_part;
    }

    void Start() { }

    void Update() { }


}
}