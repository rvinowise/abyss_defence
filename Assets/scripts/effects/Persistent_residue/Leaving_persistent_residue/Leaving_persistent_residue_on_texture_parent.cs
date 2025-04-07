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

public static class Bounds_extension {
    public static Bounds ignore_z(this Bounds bounds) {
        bounds.extents = new Vector3(bounds.extents.x, bounds.extents.y, float.MaxValue);
        return bounds;
    }
    
}

public class Leaving_persistent_residue_on_texture_parent: 
MonoBehaviour
,ILeaving_persistent_residue_on_texture
,IDestructible
{
    
    //private List<Leaving_persistent_residue_on_texture> persistent_children = new List<Leaving_persistent_residue_on_texture>(); 
    private List<SpriteRenderer> sprite_renderers = new List<SpriteRenderer>(); 
    private Persistent_residue_all_textures texture_holder;

    private Rigidbody2D rigidbody2d;
    private void Awake() {
        // persistent_children = 
        //     GetComponentsInChildren<Leaving_persistent_residue_on_texture>().
        //     Where(component => component != this).
        //     ToList();
        
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // private void Start() {
    //     texture_holder = Persistent_residue_all_textures.instance;
    // }

    public void leave_persistent_residue() {
        sprite_renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
        transform.set_z(Map.instance.ground_z);
        freeze();
        Persistent_residue_all_textures.instance.add_piece(this);
        // foreach (var persistent_child in persistent_children) {
        //     persistent_child.leave_persistent_residue();
        // }
    }


    public Bounds get_bounds_of_children() {
        var combined_bounds = new Bounds();
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers) {
            combined_bounds.Encapsulate(renderer.bounds);
        }
        // foreach (var persistent_child in persistent_children) {
        //     persistent_child.sprite_renderer.bounds.extents;
        // }
        return combined_bounds.ignore_z();
    }
    
    
    public void destroy_object() {
        ((Component)this).destroy_object();
    }

    public bool is_residue_on_texture_holder(
        Persistent_residue_texture_holder texture_holder
    ) {
        Bounds residue_bounds = get_bounds_of_children();
        Bounds texture_bounds =
            new Bounds(
                texture_holder.transform.position,
                texture_holder.transform.localScale
            ).ignore_z();
        if (
            texture_bounds.Intersects(residue_bounds)
        ) {
            return true;
        }
        return false;
    }
    
    public void prepare_for_taking_photo() {
        //gameObject.layer = Persistent_residue_all_textures.captured_layer;
        foreach (var child_renderer in sprite_renderers) {
            child_renderer.gameObject.layer = Persistent_residue_all_textures.captured_layer;
        }
    }

    public void freeze() {
        if (rigidbody2d != null) {
            rigidbody2d.simulated = false;
        }
        disable_behaviours_with_children(this);

    }
    
    public static void disable_behaviours_with_children(Component parent) {
        foreach (var child_component in parent.GetComponentsInChildren<Behaviour>()) {
            child_component.enabled = false;
        }
    } 

    public void die() {
        leave_persistent_residue();
    }


    
    
}

}