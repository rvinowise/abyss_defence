using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using UnityEditor;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.transport;
using rvinowise.unity.debug;
using rvinowise.unity.units.parts.weapons.guns.common;

using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using System.Threading;
using rvinowise.unity.extensions.pooling;

namespace rvinowise.unity.units.parts {

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Divisible_body : MonoBehaviour
,IChildren_groups_host
{
    public Sprite inside;

    public bool needs_initialisation = true; //it was added in editor and created from scratch

    
    /* IChildren_groups_host */
    
    public IList<Children_group> children_groups { get; } = new List<Children_group>();


    public Pooled_object pooled_prefab;


    void Awake() {
    }


    public List<GameObject> split_for_collider_pieces(
        List<Polygon> collider_pieces
    ) {
        Sprite body = gameObject.GetComponent<SpriteRenderer>().sprite;

        List<GameObject> piece_objects = create_objects_for_colliders(
            collider_pieces, body, inside
        );

        Children_splitter.split_children_groups(this, piece_objects);

        adjust_center_of_colliders_and_children(piece_objects, collider_pieces);

        //preserve_impulse_in_pieces(piece_objects);
        
        Destroy(gameObject);

        return piece_objects;
    }

   


    private List<GameObject> create_objects_for_colliders(
        IEnumerable<Polygon> collider_pieces,
        Sprite body,
        Sprite inside
    ) {
        detach_children();
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
    private void detach_children() {
         foreach (Transform child_transform in gameObject.direct_children()) {
             child_transform.SetParent(null, false);
         }
         foreach (Children_group children_group in children_groups) {
             children_group.hide_children_from_copying();
         }
    }

    private static int piece_counter = 0;
    private GameObject create_gameobject_from_polygon_and_texture(
        Polygon polygon, Texture2D texture
    ) 
    {
        Vector2 polygon_shift = -polygon.middle;

        Sprite body_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        
        Vector2 sprite_shift = -polygon_shift * body_sprite.pixelsPerUnit;
        
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            body_sprite.to_texture_coordinates(body_sprite.pivot + sprite_shift),
            body_sprite.pixelsPerUnit,
            0,
            SpriteMeshType.FullRect
        );

        GameObject created_part;
        /* if (pooled_prefab != null) {
            created_part = pooled_prefab.get_from_pool<Pooled_object>(
                transform.position-transform.rotation*(polygon_shift*1.1f), 
                transform.rotation
            ).gameObject;
        } else { */
            created_part = Instantiate(
                gameObject,
                transform.position-transform.rotation*(polygon_shift*1.1f*transform.lossyScale.x), 
                transform.rotation
            );
        /* } */
        created_part.name = String.Format("{0} {1}", "Spider_", piece_counter++);
        
        created_part.GetComponent<Divisible_body>().init_object_for_piece(polygon, sprite);
        return created_part;
    }

    private void init_object_for_piece(
        Polygon polygon,
        Sprite sprite
    ) {
        GetComponent<PolygonCollider2D>().SetPath(0, polygon.points.ToArray());
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<Divisible_body>().needs_initialisation = false;
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

    
    private float damaging_velocity = 5f;
    void OnCollisionEnter2D(Collision2D collision) {
        
        if (collision.get_damaging_projectile() is Projectile damaging_projectile ) {
            damage_by_projectile(damaging_projectile, collision);
        }
    }

    Task<List<Polygon>> splitting_polygon;
    private struct Received_damage {
        public Vector3 contact_point;
        public Vector3 impulse;

        public Received_damage(
            Vector2 _contact_point,
            Vector2 _impulse
        ) {
            contact_point = _contact_point;
            impulse = _impulse;
        }
    } 
    private Received_damage last_received_damage;
    private void damage_by_projectile(
        Projectile projectile, Collision2D collision
    ) {
        Vector2 contact_point = collision.GetContact(0).point;
        Ray2D ray_of_impact = new Ray2D(
            contact_point, collision.GetContact(0).relativeVelocity
        );
        
        Polygon removed_polygon = projectile.get_damaged_area(
            ray_of_impact
        );
        projectile.stop_at_position(contact_point);
        
        //Debug_drawer.instance.draw_polygon_debug(removed_polygon, 5f);

        Polygon initial_polygon = new Polygon(gameObject.GetComponent<PolygonCollider2D>().GetPath(0));
        removed_polygon = transform.InverseTransformPolygon(removed_polygon);
        
        splitting_polygon = Task.Run(()=>{
            return Polygon_splitter.remove_polygon_from_polygon(
                initial_polygon,
                removed_polygon
            );
        });
        
        last_received_damage = new Received_damage(
            contact_point,
            projectile.last_physics.velocity
        );        
        
    }

    private void on_polygon_colliders_calculated(
        Received_damage damage
    ) {
        List<GameObject> pieces = 
            split_for_collider_pieces(splitting_polygon.Result);
        push_pieces_away(
            pieces, 
            damage.contact_point, 
            damage.impulse
        );
    } 

    private void push_pieces_away(
        IEnumerable<GameObject> pieces,
        Vector2 contact_point,
        Vector3 force
    ) {
        foreach (var piece in pieces) {
            
            var rigidbody = piece.GetComponent<Rigidbody2D>();
            rigidbody.AddForce(
                calculate_push_vector(
                    piece.transform.position,
                    contact_point,
                    force.magnitude
                ),
                ForceMode2D.Impulse
            );
            rigidbody.AddTorque(
                calculate_torque(
                    rigidbody,
                    contact_point,
                    force
                ),
                ForceMode2D.Impulse
            );
        }

        Vector2 calculate_push_vector(
            Vector3 _piece_position,
            Vector3 _contact_point,
            float _force
        ) {
            return 
                (_piece_position - _contact_point).normalized *
                _force;
        }

        float calculate_torque(
            Rigidbody2D _piece,
            Vector3 _contact_point,
            Vector3 _force_vector
        ) {
            float degrees_to_piece = _contact_point.degrees_to(_piece.transform.position);
            float force_degrees = _force_vector.to_dergees();
            var rotation_to_piece = new Degree(force_degrees).angle_to(degrees_to_piece);
            //var result = rotation_to_piece.degrees * (_force_vector.magnitude / 10 / (_piece.mass*100));
            var result = Math.Sign(rotation_to_piece.degrees)*5;
            return result;
        }
    }
    

    bool polygons_calculated = false;
    void Update() {
        if (ready_to_split()) {
            on_polygon_colliders_calculated(
                last_received_damage
            );
        }
    }

    private bool ready_to_split() {
        return (
            splitting_polygon!=null 
            &&
            splitting_polygon.IsCompleted
        );
    }
    

}
}