using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using System.Threading.Tasks;
using rvinowise.unity.extensions.pooling;


namespace rvinowise.unity {

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Divisible_body : MonoBehaviour
,IChildren_groups_host
{
    public Sprite inside;
    public Sprite outside;

    /* IChildren_groups_host */
    
    public IList<Children_group> children_groups { get; private set; }


    public Pooled_object pooled_prefab;

    public delegate void EventHandler();
    public event EventHandler on_polygon_changed;

    void Awake() {
        outside = gameObject.GetComponent<SpriteRenderer>().sprite;

        children_groups = new List<Children_group>(GetComponents<Children_group>());

    }

    private void deactivate_pieces_without_children(List<Divisible_body> pieces) {
        foreach (var piece in pieces) {
            bool piece_has_children = false;
            foreach (var children_group in piece.children_groups) {
                if (children_group.children.Any()) {
                    piece_has_children = true;
                    break;
                }
            }
            if (!piece_has_children) {
                var intelligence = piece.gameObject.GetComponent<Intelligence>();
                intelligence.notify_about_destruction();
                Destroy(piece.gameObject.GetComponent<Targetable>());
                Destroy(intelligence);
                foreach (var children_group in piece.gameObject.GetComponents<Children_group>()) {
                    Destroy(children_group);
                }
                piece.children_groups.Clear();
            }
        }
    }
    
    public List<Divisible_body> split_for_collider_pieces(
        List<Polygon> collider_pieces
    ) {

        List<Divisible_body> piece_objects = create_objects_for_colliders(
            collider_pieces, outside, inside
        );

        Children_splitter.split_children_groups(this, piece_objects);

        adjust_center_of_colliders_and_children(piece_objects, collider_pieces);
        deactivate_pieces_without_children(piece_objects);

        //preserve_impulse_in_pieces(piece_objects);
        
        if (GetComponent<Intelligence>() is {} damage_receiver) {
            damage_receiver.notify_about_destruction();
        }
        Destroy(gameObject);

        return piece_objects;
    }

   


    private List<Divisible_body> create_objects_for_colliders(
        IEnumerable<Polygon> collider_pieces,
        Sprite body,
        Sprite inside
    ) {
        detach_children();
        List<Divisible_body> object_pieces = new List<Divisible_body>();
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
        List<Divisible_body> piece_objects,
        List<Polygon> collider_pieces
    ) {
        for (int i_piece = 0; i_piece < piece_objects.Count; i_piece ++) {
            
            Divisible_body created_part = piece_objects[i_piece];
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
    private Divisible_body create_gameobject_from_polygon_and_texture(
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
                transform.position-transform.rotation * polygon_shift * (1.1f * transform.lossyScale.x), 
                transform.rotation
            );
        /* } */
        created_part.name = $"{name}_{piece_counter++}";
        var test = created_part.GetComponent<Divisible_body>();
        
        Divisible_body new_body = created_part.GetComponent<Divisible_body>();
        new_body.init_object_for_piece(polygon, sprite);
        return new_body;
    }

    private void init_object_for_piece(
        Polygon polygon,
        Sprite sprite
    ) {
        GetComponent<PolygonCollider2D>().SetPath(0, polygon.points.ToArray());
        GetComponent<SpriteRenderer>().sprite = sprite;
        outside = sprite;
    }


    void OnCollisionEnter2D(Collision2D collision) {
        
        if (collision.get_damaging_projectile() is Projectile damaging_projectile ) {
            var contact = collision.GetContact(0);
            damaging_projectile.stop_at_position(contact.point);
            if (collision.otherCollider.gameObject == this.gameObject) {
                damage_by_projectile(damaging_projectile, contact);
            }
        }
    }

    Task<List<Polygon>> splitting_polygon;
    private struct Received_damage {
        public readonly Vector2 contact_point;
        public readonly Vector2 impulse;

        public Received_damage(
            Vector2 contact_point,
            Vector2 impulse
        ) {
            this.contact_point = contact_point;
            this.impulse = impulse;
        }
    } 
    private Received_damage last_received_damage;
    public void damage_by_projectile(
        Projectile projectile, ContactPoint2D contact
    ) {
        Vector2 contact_point = contact.point;
        Ray2D ray_of_impact = new Ray2D(
            contact_point, contact.relativeVelocity
        );
        
        Polygon removed_polygon = projectile.get_damaged_area(
            ray_of_impact
        );
        
        damage_by_impact(
            removed_polygon,
            contact_point,
            projectile.last_physics.velocity*projectile.rigid_body.mass
        );

    }
    
    public void damage_by_impact(
        Polygon removed_polygon, 
        Vector2 contact_point,
        Vector2 impact_vector
    ) {
        Debug_drawer.instance.draw_polygon_debug(removed_polygon, 5f);

        Polygon initial_polygon = new Polygon(gameObject.GetComponent<PolygonCollider2D>().GetPath(0));
        removed_polygon = transform.InverseTransformPolygon(removed_polygon);
        
        splitting_polygon = Task.Run(()=> Polygon_splitter.remove_polygon_from_polygon(
            initial_polygon,
            removed_polygon
        ));
        
        last_received_damage = new Received_damage(
            contact_point,
            impact_vector
        );        
        
    }

    private void make_use_of_new_polygons(
        Received_damage damage
    ) {
        List<Divisible_body> pieces = 
            split_for_collider_pieces(splitting_polygon.Result);
        push_pieces_away(
            pieces, 
            damage.contact_point, 
            damage.impulse
        );
        foreach(var piece in pieces) {
            if (piece.on_polygon_changed != null) {
                piece.on_polygon_changed();
            }
        }
    } 

    private void push_pieces_away(
        IEnumerable<Divisible_body> pieces,
        Vector2 contact_point,
        Vector2 force
    ) {
        foreach (var piece in pieces) {
            
            var rigid_body = piece.GetComponent<Rigidbody2D>();
            rigid_body.AddForce(
                calculate_push_vector(
                    piece.transform.position,
                    contact_point,
                    force.magnitude
                ),
                ForceMode2D.Impulse
            );
            rigid_body.AddTorque(
                calculate_torque(
                    rigid_body,
                    contact_point,
                    force
                ),
                ForceMode2D.Impulse
            );
        }

        Vector2 calculate_push_vector(
            Vector3 piece_position,
            Vector3 contact_point,
            float force
        ) {
            return 
                (piece_position - contact_point).normalized *
                force;
        }

        float calculate_torque(
            Rigidbody2D piece,
            Vector3 contact_point,
            Vector3 force_vector
        ) {
            float degrees_to_piece = contact_point.degrees_to(piece.transform.position);
            float force_degrees = force_vector.to_dergees();
            var rotation_to_piece = new Degree(force_degrees).angle_to(degrees_to_piece);
            //var result = rotation_to_piece.degrees * (_force_vector.magnitude / 10 / (_piece.mass*100));
            var result = Math.Sign(rotation_to_piece.degrees)*5;
            return result;
        }
    }
    

    void Update() {
        if (split_polygons_calculated()) {
            make_use_of_new_polygons(
                last_received_damage
            );
            
        }
    }

    private bool split_polygons_calculated() {
        return (
            splitting_polygon!=null 
            &&
            splitting_polygon.IsCompleted
        );
    }
    

}
}