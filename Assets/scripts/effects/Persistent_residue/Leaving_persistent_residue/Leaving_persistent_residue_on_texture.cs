#define RVI_DEBUG

using System;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEngine.Experimental.U2D.Animation;
using System.Linq;
using rvinowise.rvi;
using rvinowise.unity.extensions.pooling;
using UnityEngine.Serialization;
using Math = System.Math;


namespace rvinowise.unity {

public class Leaving_persistent_residue_on_texture: 
MonoBehaviour
,ILeaving_persistent_residue_on_texture
,IDestructible
{
    //public Sprite left_sprite;// or left_sprite_sheet
    
    private List<Leaving_persistent_residue_on_texture> persistent_children = new List<Leaving_persistent_residue_on_texture>(); 

    public SpriteRenderer sprite_renderer;
    
    private Persistent_residue_all_textures texture_holder;
    private SpriteLibrary sprite_library;

    //private SpriteRenderer[] sprite_renderers;
    private bool is_frozen = false;
    
    #if RVI_DEBUG
    public static int counter = 0;
    public int number;
    #endif
    
    private void Awake() {
        if (sprite_renderer == null) {
            sprite_renderer = GetComponentInChildren<SpriteRenderer>();
        }
        // if (left_sprite == null) {
        //     left_sprite = sprite_renderer.sprite;
        // }
        sprite_library = GetComponent<SpriteLibrary>();
        //sprite_renderers = GetComponentsInChildren<SpriteRenderer>();

        #if RVI_DEBUG
        number = counter++;
        #endif

    }


    private void Start() {

        int n_frames = 1;
        if (sprite_library != null) {
            n_frames = sprite_library.get_n_frames();
        }
        
    }

    public void on_restore_from_pool() {
        //var initial_layer = GetComponent<Pooled_object>().prefab.
        sprite_renderer.gameObject.layer =
            GetComponent<Pooled_object>().
                get_prefab().
                GetComponent<Leaving_persistent_residue_on_texture>().
                sprite_renderer.gameObject.layer;

        // foreach (var sprite_renderer in sprite_renderers) {
        //     sprite_renderer.gameObject.layer = initial_layer;
        // }
    }

    public void leave_persistent_residue() {
        if (!is_frozen_for_leaving_residue()) {
            transform.set_z(Map.instance.ground_z);
            freeze_for_leaving_persistent_image(this);
            is_frozen = true;
            Persistent_residue_all_textures.instance.add_piece(this);
        } else {
#if RVI_DEBUG
            Debug.LogError(
                $"RESIDUE: leave_persistent_residue twice for #{number}, {name} at {transform.position.x}, {transform.position.y}");
#else
            Debug.LogError(
                $"RESIDUE: leave_persistent_residue twice for {name} at {transform.position.x}, {transform.position.y}");
#endif
        }
        // foreach(var persistent_child in persistent_children) {
        //     persistent_child.leave_persistent_residue();
        // }
    }
    
    public void destroy_object() {
        ((Component)this).destroy_object();
    }
    
    public bool is_residue_on_texture_holder(
        Persistent_residue_texture_holder texture_holder
    ) {
        Vector2 distance = transform.position - texture_holder.transform.position;
        Vector2 residue_extents = sprite_renderer.bounds.extents;
        if (
            (Math.Abs(distance.x) < texture_holder.transform.localScale.x/2+residue_extents.x)
            &&
            (Math.Abs(distance.y) < texture_holder.transform.localScale.y/2+residue_extents.y)
        ) {
            return true;
        }
        return false;
    }

    public void prepare_for_taking_photo() {
        sprite_renderer.gameObject.layer = Persistent_residue_all_textures.captured_layer;
    }

    public static void freeze_for_leaving_persistent_image(Component component) {
        deactivate_all_behaviors(component);
        stop_physics(component);
    }

    public bool is_frozen_for_leaving_residue() {
        return is_frozen;
    }

    public void put_to_layer_for_photo() {
        //foreach (var sprite_renderer in sprite_renderers) {
        sprite_renderer.gameObject.layer = Persistent_residue_all_textures.captured_layer;
        //}
    }

    public static void deactivate_all_behaviors(Component component) {
        foreach (var behaviour in component.GetComponents<Behaviour>()) {
            if (behaviour is AudioSource) {
                continue;
            }
            behaviour.enabled = false;
        }
    }

    public static void stop_physics(Component component) {
        if (
            (component.GetComponent<Projectile>() is null) &&
            (component.GetComponent<Rigidbody2D>() is {} rigidbody2d) && (rigidbody2d!= null)
        ){
            rigidbody2d.velocity = Vector2.zero;
            rigidbody2d.angularVelocity = 0;

        }
    }
    
    public void die() {
        leave_persistent_residue();
    }

#if RVI_DEBUG
    private void OnDestroy() {
        Debug.Log($"RESIDUE: Leaving_persistent_residue_on_texture::OnDestroy for residue #{number}, {gameObject}, at (({transform.position.x}, {transform.position.y})");
    }
#endif
    
    
}

}