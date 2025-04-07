using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEngine.Experimental.U2D.Animation;
using System.Linq;
using rvinowise.rvi;


namespace rvinowise.unity {

public class Leaving_persistent_sprite_residue: 
MonoBehaviour
,ILeaving_persistent_residue
,IDestructible
{

    public int max_images = 1500;
    public Sprite left_sprite;// or left_sprite_sheet
    
    private List<Leaving_persistent_sprite_residue> persistent_children = new List<Leaving_persistent_sprite_residue>(); 

    /* since Unity messes up the imported sprite sheet, i have to keep its size in separate fields */
    //private Dimensionf residue_dimension; //if left_sprite is a sprite_sheet
    
    public SpriteRenderer sprite_renderer;
    public SpriteResolver sprite_resolver;
    
    private Persistent_residue_sprite_holder holder;
    private SpriteLibrary sprite_library;

    private void Awake() {
        if (sprite_renderer == null) {
            sprite_renderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (left_sprite == null) {
            left_sprite = sprite_renderer.sprite;
        }
        sprite_resolver = GetComponent<UnityEngine.Experimental.U2D.Animation.SpriteResolver>();
        sprite_library = GetComponent<UnityEngine.Experimental.U2D.Animation.SpriteLibrary>();
    }


    private void Start() {

        int n_frames = 1;
        if (sprite_library != null) {
            n_frames = sprite_library.get_n_frames();
        }
        holder = Persistent_residue_router.instance.provide_holder_for_texture(
            left_sprite,
            max_images,
            n_frames
        );
        persistent_children = 
            GetComponentsInChildren<Leaving_persistent_sprite_residue>().
            Where(component => component != this).
            ToList();
 
    }

    
    public void leave_persistent_residue() {
        freeze_for_leaving_persistent_image();

        foreach(var persistent_child in persistent_children) {
            persistent_child.leave_persistent_residue();
        }
    }
    
    public void destroy_object() {
        ((Component)this).destroy_object();
    }

    public void freeze_for_leaving_persistent_image() {
        deactivate_all_behaviors();
        holder.add_piece(this);
    }

    private void deactivate_all_behaviors() {
        foreach (var behaviour in gameObject.GetComponents<Behaviour>()) {
            behaviour.enabled = false;
        }
    }
    
    public void die() {
        leave_persistent_residue();
    }
}

}