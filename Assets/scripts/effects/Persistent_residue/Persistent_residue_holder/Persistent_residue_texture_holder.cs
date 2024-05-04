using System;
using System.Collections.Generic;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.pooling;
using rvinowise.unity.geometry2d;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Comparers;


namespace rvinowise.unity {
public class Persistent_residue_texture_holder: 
MonoBehaviour,
IPersistent_residue_holder
{

    public Texture2D screen_texture;
    public Texture2D permanent_texture;
    

    /* adding each piece to the mesh is slow, we need to add many pieces at once, after they accumulate as simple game objects */
    private List<Leaving_persistent_residue_on_texture> batched_residues = new List<Leaving_persistent_residue_on_texture>();


    private const int max_batch_amount = 50;


    private void Awake() {
        RenderTexture previous_texture = UnityEngine.RenderTexture.active;
        //UnityEngine.RenderTexture.active = screen_texture;
        GL.Clear(true, true, Color.clear);
        UnityEngine.RenderTexture.active = previous_texture;
    }

    public void fix_residue_on_texture() {
        //Graphics.CopyTexture(screen_texture, permanent_texture);

        //Texture_drawer.instance.draw_texture_on_texture(permanent_texture,screen_texture);
        
        RenderTexture text_texture = 
            new RenderTexture(
                screen_texture.width, screen_texture.height, 32, RenderTextureFormat.ARGB32
            );
        
        var new_texture = Texture_drawer.instance.draw_texture_on_texture(text_texture,screen_texture);
        //permanent_texture.Release();
        //screen_texture.Release();
        //Graphics.Blit(new_texture, text_texture);
        text_texture.save_to_file("text_texture");
        screen_texture.save_to_file("screen_texture");
        new_texture.save_to_file("new_texture");
    }
    
    public void add_piece(
        Leaving_persistent_residue_on_texture in_residue
    ) {
        fix_residue_on_texture();
        in_residue.destroy_object();
    }

    private void turn_piece_into_texture() {

        foreach (var piece in batched_residues) {
            int current_frame = 0;
            if (piece.sprite_resolver != null) {
                current_frame = piece.sprite_resolver.get_label_as_number();
            }
            
            draw_piece_on_texture(
                piece.transform.position, 
                piece.transform.rotation,
                piece.sprite_renderer.transform.localScale.x,
                current_frame,
                piece.sprite_renderer.flipX,
                piece.sprite_renderer.flipY
            );
            
            piece.destroy_object();
        }
    }
    
    private void draw_piece_on_texture(
        Vector3 in_position,
        Quaternion in_rotation,
        float in_size =1,
        int in_current_frame=0,
        bool flip_x = false,
        bool flip_y = false
    ) {
        Vector3 relative_position = (Vector3)in_position - transform.position;
        
        
    }

    



}

}
