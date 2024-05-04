using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEngine.Experimental.U2D.Animation;
using System.Linq;
using rvinowise.rvi;
using UnityEngine.Serialization;


namespace rvinowise.unity {

public class Leaving_persistent_residue_on_texture: 
MonoBehaviour
,ILeaving_persistent_residue
,IDestructible
{
    public Sprite left_sprite;// or left_sprite_sheet
    
    private List<Leaving_persistent_sprite_residue> persistent_children = new List<Leaving_persistent_sprite_residue>(); 

    public SpriteRenderer sprite_renderer;
    
    public SpriteResolver sprite_resolver;
    private Persistent_residue_texture_holder texture_holder;
    private SpriteLibrary sprite_library;

    private int captured_layer;
    
    private void Awake() {
        if (sprite_renderer == null) {
            sprite_renderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (left_sprite == null) {
            left_sprite = sprite_renderer.sprite;
        }
        sprite_resolver = GetComponent<SpriteResolver>();
        sprite_library = GetComponent<SpriteLibrary>();
        captured_layer = LayerMask.NameToLayer("litter");
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

    
    public void leave_persistent_residue() {
        freeze_for_leaving_persistent_image();

        foreach(var persistent_child in persistent_children) {
            persistent_child.leave_persistent_residue();
        }
    }

    public void freeze_for_leaving_persistent_image() {
        deactivate_all_behaviors();
        gameObject.layer = captured_layer;
        Debug.Break();
        texture_holder.add_piece(this);
    }

    private void deactivate_all_behaviors() {
        foreach (var behaviour in gameObject.GetComponents<Behaviour>()) {
            behaviour.enabled = false;
        }
    }
    
    public void on_start_dying() {
        leave_persistent_residue();
    }
}

}