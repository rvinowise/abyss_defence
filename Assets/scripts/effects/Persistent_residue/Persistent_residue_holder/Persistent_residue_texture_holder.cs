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

    public RenderTexture screen_texture;
    public RenderTexture permanent_texture;
    public Camera residue_camera;
    

    /* adding each piece to the mesh is slow, we need to add many pieces at once, after they accumulate as simple game objects */
    private List<Leaving_persistent_residue_on_texture> batched_residues = new List<Leaving_persistent_residue_on_texture>();


    private const int max_batch_amount = 200;
    private int captured_layer;

    private void Awake() {
        RenderTexture previous_texture = UnityEngine.RenderTexture.active;
        //UnityEngine.RenderTexture.active = screen_texture;
        GL.Clear(true, true, Color.clear);
        UnityEngine.RenderTexture.active = previous_texture;
        residue_camera.enabled = false;
        captured_layer = LayerMask.NameToLayer("litter");
        
        permanent_texture.clear();
    }

    private void OnDestroy() {
        permanent_texture.clear();
    }

    
    
    public void add_piece(
        Leaving_persistent_residue_on_texture in_residue
    ) {
        batched_residues.Add(in_residue);
        if (batched_residues.Count > max_batch_amount) {
            fix_residue_on_texture();
            batched_residues.Clear();
        }
    }

    private void fix_residue_on_texture() {
        foreach (var piece in batched_residues) {
            piece.sprite_renderer.gameObject.layer = captured_layer;
        }
        take_photo_of_residue();
        foreach (var piece in batched_residues) {
            piece.destroy_object();
        }
    }
    
    private void take_photo_of_residue() {
        residue_camera.enabled = true;
        residue_camera.targetTexture = screen_texture;
        residue_camera.Render();
        residue_camera.targetTexture = null;
        residue_camera.enabled = false;
        
        Texture_drawer.instance.draw_texture_on_texture(permanent_texture,screen_texture);
        RenderTexture.active = null;
    }

    

    



}

}
