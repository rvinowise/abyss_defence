﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using System.Threading.Tasks;
using rvinowise.unity.extensions.pooling;


namespace rvinowise.unity {

//[RequireComponent(typeof(PolygonCollider2D))]
//[RequireComponent(typeof(SpriteRenderer))]
public partial class Divisible_body : MonoBehaviour
,IChildren_groups_host
{
    public Sprite inside;
    public SpriteRenderer sprite_renderer;
    private Sprite outside;

    /* IChildren_groups_host */
    
    //public IList<Abstract_children_group> children_groups { get; private set; }
    public List<IChildren_group> children_groups { get; private set; }
    public IList<IChild_of_group> children { get; private set; }

    public delegate void EventHandler();
    public event EventHandler on_polygon_changed;

    private bool is_splitting_triggered;
    private Divisible_body main_divisible_body;

    public void Awake() {
        if (sprite_renderer == null) {
            sprite_renderer = GetComponent<SpriteRenderer>();
        }
        outside = sprite_renderer.sprite;

        children_groups = this.get_components_in_children_stop_at_component<IChildren_group, Divisible_body>();

        main_divisible_body = GetComponentsInParent<Divisible_body>().Last();

        //children = new List<IChild_of_group>(GetComponentsInChildren<IChild_of_group>());
    }

    
    
    public List<Divisible_body> split_for_polygon_pieces(
        List<Polygon> piece_polygons
    ) {

        detach_children();
        var polygons_with_tools = 
            Children_splitter.distribute_children_to_polygons(this, piece_polygons);
        
        List<Divisible_body> piece_objects = create_objects_for_polygons(
            polygons_with_tools, outside, inside
        );

        Children_splitter.split_children_groups(this, piece_objects);
        
        adjust_center_of_pieces(piece_objects, piece_polygons);

        if (GetComponent<Intelligence>() is {} intelligence) {
            intelligence.notify_about_destruction();
        }
        Debug.Log($"AIMING: Destroy({name})");
        Destroy(gameObject);

        return piece_objects;
    }

   


    private List<Divisible_body> create_objects_for_polygons(
        IEnumerable<Children_splitter.Polygon_with_tools> piece_polygons,
        Sprite original_sprite,
        Sprite inside
    ) {
        List<Divisible_body> object_pieces = new List<Divisible_body>();
        foreach (var piece_polygon in piece_polygons) {

            Sprite piece_sprite = create_sprite_for_polygon(piece_polygon.polygon, original_sprite, inside);

            if (piece_polygon.attached_to_something) {
                
            } else {
                object_pieces.Add(
                    create_gameobject_from_polygon_and_sprite(
                        piece_polygon.polygon, piece_sprite
                    )
                );
            }
        }
        return object_pieces;
    }

    private void adjust_center_of_pieces(
        List<Divisible_body> piece_objects,
        List<Polygon> collider_pieces
    ) {
        for (int i_piece = 0; i_piece < piece_objects.Count; i_piece ++) {
            
            Divisible_body created_part = piece_objects[i_piece];
            IChildren_groups_host children_group_host = created_part.GetComponent<IChildren_groups_host>();
            
            Polygon collider_polygon = collider_pieces[i_piece];
            Vector2 polygon_shift = -collider_polygon.middle;
            collider_polygon.move(polygon_shift); 
            
            foreach (IChildren_group children_group in children_group_host.children_groups) {
                children_group.shift_center(polygon_shift);
            }

            
            created_part.GetComponent<PolygonCollider2D>().SetPath(0, collider_polygon.points.ToArray());
        }
    }
    
    /* to avoid duplication of the Children when duplicating the Body */
    private void detach_children() {
         foreach (IChildren_group children_group in children_groups) {
             children_group.hide_children_from_copying();
         }
    }

    private static int piece_counter;

    
    public static Sprite create_sprite_for_polygon(
        Polygon polygon, Sprite original_sprite, Sprite inside
    ) 
    {
        Texture2D polygon_texture = Texture_splitter.create_texture_for_polygon(
            original_sprite,
            polygon,
            inside
        );

        Sprite polygon_sprite = create_sprite_for_piece(
            polygon, polygon_texture, original_sprite
        );
        return polygon_sprite;
    }
    public static Sprite create_sprite_for_piece(
        Polygon polygon, Texture2D texture, Sprite original_sprite
    ) 
    {
        Vector2 polygon_shift = -polygon.middle;

        Vector2 sprite_shift = -polygon_shift * original_sprite.pixelsPerUnit;
        
        return Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            original_sprite.to_texture_coordinates(original_sprite.pivot + sprite_shift),
            original_sprite.pixelsPerUnit,
            0,
            SpriteMeshType.FullRect
        );
    }
    private Divisible_body create_gameobject_from_polygon_and_sprite(
        Polygon polygon, Sprite piece_sprite
    ) 
    {
        Vector2 polygon_shift = -polygon.middle;
        GameObject created_part = Instantiate(
            gameObject,
            transform.position-transform.rotation * polygon_shift * (1.1f * transform.lossyScale.x), 
            transform.rotation
        );
        
        created_part.name = $"{name}_{piece_counter++}";
        
        Divisible_body new_body = created_part.GetComponent<Divisible_body>();
        new_body.init_object_for_piece(polygon, piece_sprite);
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
            if (main_divisible_body.is_splitting_triggered) {
                return;
            }
            main_divisible_body.is_splitting_triggered = true;

            
            Debug.Log($"AIMING: ({name})Divisible_body.OnCollisionEnter2D(projectile:{damaging_projectile.name})");
            
            var contact = collision.GetContact(0);
            damaging_projectile.stop_at_position(contact.point);
            damage_by_projectile(damaging_projectile, contact);
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

    private void damage_by_projectile(
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
            projectile.last_physics.velocity*projectile.rigid_body.mass * 40f
        );

    }

    private ISet<Damage_dealer> damage_dealers = new HashSet<Damage_dealer>();
    public void remember_damage_dealer(Damage_dealer damage_dealer) {
        damage_dealers.Add(damage_dealer);
    }
    public void forget_damage_dealer(Damage_dealer damage_dealer) {
        damage_dealers.Remove(damage_dealer);
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
        Debug.Log($"AIMING: ({name})Divisible_body.make_use_of_new_polygons");
        
        List<Divisible_body> pieces = 
            split_for_polygon_pieces(splitting_polygon.Result);
        
        push_pieces_away(
            pieces, 
            damage.contact_point, 
            damage.impulse
        );
        
        foreach(var piece in pieces) {
            if (piece.on_polygon_changed != null) {
                piece.on_polygon_changed();
            }
            piece.damage_dealers.UnionWith(this.damage_dealers);
            piece.notify_damage_dealers_of_peace();
        }
    }

    private void notify_damage_dealers_of_peace() {
        foreach (var damage_dealer in damage_dealers) {
            damage_dealer.remember_damaged_target(this.transform);
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
            var result = Math.Sign(rotation_to_piece.degrees)*force_vector.magnitude/400;
            return result;
        }
    }
    

    void Update() {
        if (split_polygons_calculated()) {
            main_divisible_body.is_splitting_triggered = false;
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