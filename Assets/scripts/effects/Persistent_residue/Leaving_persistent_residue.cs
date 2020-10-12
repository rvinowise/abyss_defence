using System.Collections;
using System.Collections.Generic;
using effects.persistent_residue;
using geometry2d;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.Serialization;

namespace effects.persistent_residue {

public class Leaving_persistent_residue : MonoBehaviour {

    public int max_images = 1500;
    public Texture2D left_texture;// or left_sprite_sheet
    
    /* since Unity messes up the imported sprite sheet, i have to keep its size in separate fields */
    public Dimension left_texture_dimension; //if left_texture is a sprite_sheet
    
    private SpriteRenderer sprite_renderer;
    private Persistent_residue_holder holder;
    private SpriteResolver sprite_resolver;
    private SpriteLibrary sprite_library;

    private void Awake() {
        Debug.Log("Leaving_persistent_residue::Awake for " + this);
        sprite_renderer = GetComponent<SpriteRenderer>();
        if (left_texture == null) {
            left_texture = sprite_renderer.sprite.texture;
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
            left_texture,
            left_texture_dimension,
            max_images,
            n_frames
        );
    }

    public void leave_persistent_image(
        Vector2 in_position,
        Quaternion in_rotation,
        float in_size
    ) {
        holder.add_quad(in_position,in_rotation,in_size);
    }
    
    public void leave_persistent_image() {

        int current_frame = 0;
        if (sprite_resolver != null) {
            current_frame = sprite_resolver.get_label_as_number();
        }

        leave_persistent_image(current_frame);
    }

    public void leave_persistent_image(int in_frame) {
        holder.add_quad(
            transform.position,
            transform.rotation,
            sprite_renderer.get_units_size().x,
            in_frame
        );
    }
}

}