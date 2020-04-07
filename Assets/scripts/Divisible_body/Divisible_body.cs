using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using geometry2d;
using rvinowise.units;
using rvinowise.units.parts;
using rvinowise.units.parts.transport;


namespace rvinowise.units.parts {

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Divisible_body : MonoBehaviour
,IChildren_groups_host
{
    public Sprite inside;

    public bool needs_initialisation = true; //it was added ineditor and created from scratch

    
    /* IChildren_groups_host */
    
    public ITransporter transporter { get; set; }

    public GameObject game_object {
        get {return gameObject;}
    }
    
    public IList<Children_group> children_groups { get; } = new List<Children_group>();


    /* Divisible_body itself */
    public IEnumerable<GameObject> split_by_ray(Ray2D ray_of_split) {

        List<Polygon> collider_pieces = Polygon_splitter.split_polygon_by_ray(
            new Polygon(gameObject.GetComponent<PolygonCollider2D>().GetPath(0)),
            transform.InverseTransformRay(ray_of_split)
        );

        Sprite body = gameObject.GetComponent<SpriteRenderer>().sprite;

        IEnumerable<GameObject> piece_objects = create_objects_for_colliders(
            collider_pieces, body, inside
        );

        Children_splitter.split_children_groups(gameObject, piece_objects);

        
        Destroy(gameObject);
        
        return piece_objects;
    }

  

    private IEnumerable<GameObject> create_objects_for_colliders(
        IEnumerable<Polygon> collider_pieces,
        Sprite body,
        Sprite inside
    ) {
        detach_children(gameObject);
        List<GameObject> object_pieces = new List<GameObject>();
        foreach (Polygon collider_piece in collider_pieces)
        {
            Texture2D texture_piece =
                Texture_splitter.create_texture_for_polygon(
                    body,
                    collider_piece,
                    inside);

            object_pieces.Add(
                create_gameobject_from_polygon_and_texture(
                    collider_piece, texture_piece
                )
            );
        }
        return object_pieces;
    }

    private void detach_children(GameObject game_object) {
         //Transform[] children = GetComponentsInChildren<Transform>();
         foreach (Transform child_transform in gameObject.direct_children()) {
             child_transform.SetParent(null, false);
         }
    }

    private GameObject create_gameobject_from_polygon_and_texture(
        Polygon polygon, Texture2D texture) 
    {
        

        Vector2 polygon_shift = -polygon.middle;
        polygon.move(polygon_shift); //center the polygon

        Sprite body_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        
        Vector2 sprite_shift = -polygon_shift * body_sprite.pixelsPerUnit;
        
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            body_sprite.to_texture_coordinates(body_sprite.pivot + sprite_shift),
            body_sprite.pixelsPerUnit
        );

        GameObject created_part = Instantiate(
            gameObject,
            transform.position-transform.rotation*(polygon_shift*1.1f), 
            transform.rotation
        );
        
        created_part.GetComponent<PolygonCollider2D>().SetPath(0, polygon.points.ToArray());
        created_part.GetComponent<SpriteRenderer>().sprite = sprite;
        created_part.GetComponent<Divisible_body>().needs_initialisation = false;
        return created_part;
    }


    
    

}
}