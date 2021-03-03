using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.Serialization;
using System.Linq;

namespace rvinowise.unity.effects.persistent_residue {

public class Leaving_persistent_sprite_residue: 
MonoBehaviour
,ILeaving_persistent_residue 
{

    public int max_images = 1500;
    public Sprite left_sprite;// or left_sprite_sheet
    
    [HideInInspector]
    private List<Leaving_persistent_sprite_residue> children = new List<Leaving_persistent_sprite_residue>(); 

    /* since Unity messes up the imported sprite sheet, i have to keep its size in separate fields */
    //private Dimensionf residue_dimension; //if left_sprite is a sprite_sheet
    
    private SpriteRenderer sprite_renderer;
    private Persistent_residue_sprite_holder holder;
    private SpriteResolver sprite_resolver;
    private SpriteLibrary sprite_library;

    private void Awake() {
        sprite_renderer = GetComponent<SpriteRenderer>();
        if (left_sprite == null) {
            left_sprite = sprite_renderer.sprite;
        }
        sprite_resolver = GetComponent<SpriteResolver>();
        sprite_library = GetComponent<SpriteLibrary>();
    }


    private void Start() {

        int n_frames = 1;
        if (sprite_library != null) {
            n_frames = sprite_library.get_n_frames();
        }
        holder = Persistent_residue_router.instance.get_holder_for_texture(
            left_sprite,
            max_images,
            n_frames
        );
        children = 
            GetComponentsInChildren<Leaving_persistent_sprite_residue>().
            Where(component => component != this).
            ToList();
 
    }

    
    public void leave_persistent_residue() {

        int current_frame = 0;
        if (sprite_resolver != null) {
            current_frame = sprite_resolver.get_label_as_number();
        }
        leave_persistent_image(current_frame);

        foreach(var child in children) {
            child.leave_persistent_residue();
        }
    }

    public void leave_persistent_image(int in_frame) {
        holder.add_piece(
            transform.position,
            transform.rotation,
            transform.localScale.x,
            in_frame,
            sprite_renderer.flipX,
            sprite_renderer.flipY
        );
    }
}

}