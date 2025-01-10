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


namespace rvinowise.unity {

public class Leaving_persistent_residue_on_texture: 
MonoBehaviour
,ILeaving_persistent_residue
,IDestructible
{
    //public Sprite left_sprite;// or left_sprite_sheet
    
    private List<Leaving_persistent_sprite_residue> persistent_children = new List<Leaving_persistent_sprite_residue>(); 

    public SpriteRenderer sprite_renderer;
    
    private Persistent_residue_all_textures texture_holder;
    private SpriteLibrary sprite_library;

    private int captured_layer;
    private int initial_layer;
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
        captured_layer = LayerMask.NameToLayer("litter");
        initial_layer = LayerMask.NameToLayer("litter");
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
        
        persistent_children = 
            GetComponentsInChildren<Leaving_persistent_sprite_residue>().
            Where(component => component != this).
            ToList();

        texture_holder = Persistent_residue_router.instance.render_texture_holder;
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
            freeze_for_leaving_persistent_image();
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

    private void freeze_for_leaving_persistent_image() {
        deactivate_all_behaviors();
        is_frozen = true;
        texture_holder.add_piece(this);
    }

    public bool is_frozen_for_leaving_residue() {
        return is_frozen;
    }

    public void put_to_layer_for_photo() {
        //foreach (var sprite_renderer in sprite_renderers) {
        sprite_renderer.gameObject.layer = captured_layer;
        //}
    }

    private void deactivate_all_behaviors() {
        foreach (var behaviour in gameObject.GetComponents<Behaviour>()) {
            behaviour.enabled = false;
        }
    }
    
    public void on_start_dying() {
        leave_persistent_residue();
    }

#if RVI_DEBUG
    private void OnDestroy() {
        Debug.Log($"RESIDUE: Leaving_persistent_residue_on_texture::OnDestroy for residue #{number}, {gameObject}, at (({transform.position.x}, {transform.position.y})");
    }
#endif
    
    
}

}