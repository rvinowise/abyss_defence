using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using System.Threading.Tasks;
using rvinowise.unity.actions;
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


    private bool is_splitting_triggered;
    private Divisible_body main_divisible_body;
    protected Collider2D collider2d;

    public void Awake() {
        if (sprite_renderer == null) {
            sprite_renderer = GetComponent<SpriteRenderer>();
        }
        collider2d = GetComponent<Collider2D>();
        
        outside = sprite_renderer.sprite;

        children_groups = this.get_components_in_children_stop_at_component<IChildren_group, Divisible_body>();

        update_body_composition();

        //children = new List<IChild_of_group>(GetComponentsInChildren<IChild_of_group>());
    }

    public void update_body_composition() {
        main_divisible_body = GetComponentsInParent<Divisible_body>().Last();

    }
    
    
    public List<Divisible_body> split_for_polygon_pieces(
        List<Polygon> piece_polygons
    ) {

        detach_children();
        var polygons_with_tools = 
            Children_splitter.distribute_children_to_polygons(this, piece_polygons);
        
        List<Divisible_body> piece_objects = create_objects_for_polygons(
            this, polygons_with_tools, outside, inside
        );
        
        Children_splitter.split_children_groups(this, piece_objects);
        

        if (GetComponent<Intelligence>() is {} intelligence) {
            intelligence.notify_about_destruction();
        }
        //Destroy(gameObject);
        splitting_polygon.Dispose();
        splitting_polygon = null;

        return piece_objects;
    }

   


    private List<Divisible_body> create_objects_for_polygons(
        Divisible_body original_body,
        IList<Children_splitter.Polygon_with_tools> piece_polygons,
        Sprite original_sprite,
        Sprite inside
    ) {
        List<Divisible_body> object_pieces = new List<Divisible_body>();
        foreach (var piece_polygon in piece_polygons.Take(piece_polygons.Count -1)) {
            
            Texture2D polygon_texture = Texture_splitter.create_texture_for_polygon(
                original_sprite,
                piece_polygon.polygon,
                inside
            );
            
            Sprite piece_sprite = create_shifted_sprite_for_piece(
                piece_polygon.polygon, 
                polygon_texture,
                original_sprite
            );
            var piece_body = create_gameobject_from_polygon_and_sprite(
                piece_polygon.polygon, piece_sprite
            );
            
            Children_splitter.attach_children_to_host(piece_body, piece_polygon.tools);
            adjust_center_of_piece(piece_body, piece_polygon.polygon);
            if (piece_was_part_of_bigger_system(this)) {
                make_piece_self_sufficient(this, piece_body);
            }
            object_pieces.Add(piece_body);
        }
        reuse_original_body_for_piece(original_body, piece_polygons.Last(), original_sprite, inside);
        object_pieces.Add(this);
        
        return object_pieces;
    }

    private void reuse_original_body_for_piece(
        Divisible_body original_body,
        Children_splitter.Polygon_with_tools piece_polygon,
        Sprite original_sprite,
        Sprite inside
    ) {
        Children_splitter.attach_children_to_host(this, piece_polygon.tools);
        Sprite piece_sprite;
        Texture2D polygon_texture = Texture_splitter.create_texture_for_polygon(
            original_sprite,
            piece_polygon.polygon,
            inside
        );
        if (piece_polygon.attached_to_something) {
            piece_sprite = create_unshifted_sprite_for_piece(polygon_texture, original_sprite);
        }
        else {
            piece_sprite = create_shifted_sprite_for_piece(piece_polygon.polygon, polygon_texture, original_sprite);
            adjust_center_of_piece(this, piece_polygon.polygon);
        }
        original_body.init_object_for_piece(piece_polygon.polygon, piece_sprite);
        if (original_body.main_divisible_body?.GetComponent<Intelligence>() is { } intelligence) {
            intelligence.update_devices();
        }
    }

    private void adjust_center_of_pieces(
        List<Divisible_body> piece_objects,
        List<Polygon> collider_pieces
    ) {
        for (int i_piece = 0; i_piece < piece_objects.Count; i_piece ++) {
            Divisible_body created_part = piece_objects[i_piece];
            Polygon collider_polygon = collider_pieces[i_piece];
            adjust_center_of_piece(created_part, collider_polygon);
        }
    }

    public void adjust_center_of_piece(
        Divisible_body piece_body,
        Polygon piece_polygon
        ) 
    {
        IChildren_groups_host children_group_host = piece_body.GetComponent<IChildren_groups_host>();
            
        Vector2 polygon_shift = -piece_polygon.middle;
        piece_polygon.move(polygon_shift); 
            
        foreach (IChildren_group children_group in children_group_host.children_groups) {
            children_group.shift_center(polygon_shift);
        }
            
        piece_body.GetComponent<PolygonCollider2D>().SetPath(0, piece_polygon.points.ToArray());    
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

        Sprite polygon_sprite = create_shifted_sprite_for_piece(
            polygon, polygon_texture, original_sprite
        );
        return polygon_sprite;
    }
    public static Sprite create_shifted_sprite_for_piece(
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
    public static Sprite create_unshifted_sprite_for_piece(
        Texture2D texture, Sprite original_sprite
    ) 
    {
        return Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            original_sprite.to_texture_coordinates(original_sprite.pivot),
            original_sprite.pixelsPerUnit,
            0,
            SpriteMeshType.FullRect
        );
    }
    private Divisible_body create_gameobject_from_polygon_and_sprite(
        Polygon polygon, Sprite piece_sprite
    ) 
    {
        GameObject created_part = Instantiate(
            gameObject,
            transform.position+get_shift_for_split_piece(transform, polygon), 
            transform.rotation
        );
        
        created_part.name = $"{name}_{piece_counter++}";
        
        Divisible_body new_body = created_part.GetComponent<Divisible_body>();
        new_body.init_object_for_piece(polygon, piece_sprite);
        
        
        return new_body;
    }

    private static bool piece_was_part_of_bigger_system(Divisible_body original_body) {
        return ((original_body.main_divisible_body != original_body) && (original_body.main_divisible_body != null));
    }

    private static void make_piece_self_sufficient(Divisible_body original_body, Divisible_body new_body) {
        if (new_body.GetComponent<Turning_sensor>() is {} turning_sensor) {
            Destroy(turning_sensor);
        }
        
        if (new_body.GetComponent<Rigidbody2D>() == null) {
            var rigidbody2d = new_body.gameObject.AddComponent<Rigidbody2D>();
            rigidbody2d.gravityScale = 0;
            new_body.collider2d.density = 500;
            rigidbody2d.useAutoMass = true;
        }
        
        var new_action_runner = new_body.gameObject.AddComponent<Action_runner>();
        var new_intelligence = new_body.gameObject.AddComponent<Computer_intelligence>();
        var original_intelligence = original_body.main_divisible_body.GetComponent<Intelligence>();
        
        Attach_unit_parts.attach_all_team_members_to_team(new_body, original_intelligence.team);
        Attach_unit_parts.attach_all_tools_to_intelligence(new_body, new_intelligence);
        Attach_unit_parts.update_divisible_body_composition(new_body);
        new_intelligence.action_runner.update_actors();
        new_intelligence.update_devices();
    }

    public static Vector3 get_shift_for_split_piece(
        Transform original_body, Polygon piece_polygon
    ) {
        Vector2 polygon_shift = -piece_polygon.middle;
        return -(original_body.rotation * polygon_shift * (1.1f * original_body.lossyScale.x));
    }

    private void init_object_for_piece(
        Polygon polygon,
        Sprite sprite
    ) {
        GetComponent<PolygonCollider2D>().SetPath(0, polygon.points.ToArray());
        sprite_renderer.sprite = sprite;
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
            
            piece.damage_dealers.UnionWith(this.damage_dealers);
            piece.notify_damage_dealers_of_piece();
        }
    }

    private void notify_damage_dealers_of_piece() {
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
            if (rigid_body != null) {
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