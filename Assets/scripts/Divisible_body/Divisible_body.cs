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
    

    public GameObject game_object {
        get {return gameObject;}
    }
    
    public IList<Children_group> children_groups { get; } = new List<Children_group>();


    /* Divisible_body itself */
    public IEnumerable<GameObject> split_by_ray(Ray2D ray_of_split) {

        return remove_polygon(
            Polygon_splitter.get_wedge_from_ray(ray_of_split)  
        );
        
    }

    public IEnumerable<GameObject> remove_polygon(Polygon polygon_of_split) {
        List<Polygon> collider_pieces = Polygon_splitter.remove_polygon_from_polygon(
            new Polygon(gameObject.GetComponent<PolygonCollider2D>().GetPath(0)),
            transform.InverseTransformPolygon(polygon_of_split)
        );

        Sprite body = gameObject.GetComponent<SpriteRenderer>().sprite;

        List<GameObject> piece_objects = create_objects_for_colliders(
            collider_pieces, body, inside
        );

        Children_splitter.split_children_groups(gameObject, piece_objects);

        adjust_center_of_colliders_and_children(piece_objects, collider_pieces);

        preserve_impulse_in_pieces(piece_objects);
        
        Destroy(gameObject);
        
        return piece_objects;
    }



    private List<GameObject> create_objects_for_colliders(
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

    private void adjust_center_of_colliders_and_children(
        List<GameObject> piece_objects,
        List<Polygon> collider_pieces
    ) {
        for (int i_piece = 0; i_piece < piece_objects.Count; i_piece ++) {
            
            GameObject created_part = piece_objects[i_piece];
            IChildren_groups_host children_group_host = created_part.GetComponent<IChildren_groups_host>();
            
            Polygon collider_polygon = collider_pieces[i_piece];
            Vector2 polygon_shift = -collider_polygon.middle;
            collider_polygon.move(polygon_shift); 
            
            foreach (Children_group children_group in children_group_host.children_groups) {
                children_group.shift_center(polygon_shift);
            }

            
            created_part.GetComponent<PolygonCollider2D>().SetPath(0, collider_polygon.points.ToArray());
        }
    }
    
    /* to avoid duplication of the Children when duplicating the Body */
    private void detach_children(GameObject game_object) {
         //Transform[] children = GetComponentsInChildren<Transform>();
         foreach (Transform child_transform in gameObject.direct_children()) {
             child_transform.SetParent(null, false);
         }
    }

    private GameObject create_gameobject_from_polygon_and_texture(
        Polygon polygon, Texture2D texture
    ) 
    {
        

        Vector2 polygon_shift = -polygon.middle;
        //polygon.move(polygon_shift); 

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


    private void preserve_impulse_in_pieces(List<GameObject> piece_objects) {
        Rigidbody2D rigid_body = gameObject.GetComponent<Rigidbody2D>();
        if (rigid_body) {
            foreach (GameObject piece in piece_objects) {
                Rigidbody2D piece_rigid_body = piece.GetComponent<Rigidbody2D>();
                
                //piece_rigid_body.velocity = rigid_body.velocity;// / Time.deltaTime;
                //piece_rigid_body.angularVelocity= rigid_body.angularVelocity;// / Time.deltaTime;
                
                piece_rigid_body.AddForce(rigid_body.velocity / Time.deltaTime, ForceMode2D.Impulse);
                piece_rigid_body.AddTorque(rigid_body.angularVelocity / Time.deltaTime, ForceMode2D.Impulse);

            }
        }
    }
    
    
    public void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("OnCollisionEnter2D in "+this.gameObject.name);
        Projectile collided_projectile = other.gameObject.GetComponent<Projectile>();
        if (collided_projectile != null) {

            Vector2 contact_point = other.GetContact(0).point;
            Ray2D ray_of_impact = new Ray2D(
                contact_point, other.GetContact(0).relativeVelocity
            );
           
            Polygon removed_polygon = collided_projectile.get_damaged_area(
                ray_of_impact
            );
            Destroy(collided_projectile.gameObject);
            
            Debug_drawer.instance.draw_polygon_debug(removed_polygon, 5f);

            var pieces = remove_polygon(removed_polygon);
            //push_pieces_away(pieces, contact_point, collided_projectile.last_physics.velocity.magnitude);
        }
    }

    private void push_pieces_away(
        IEnumerable<GameObject> pieces,
        Vector2 contact_point,
        float force
    ) {
        foreach (var piece in pieces) {
            Vector2 push_vector = 
                ((Vector2)piece.transform.position - contact_point).normalized *
                force;
            var rigidbody = piece.GetComponent<Rigidbody2D>();
            rigidbody.AddForce(push_vector, ForceMode2D.Impulse);
            //rigidbody.AddTorque(50f);
        }
    }
    

}
}