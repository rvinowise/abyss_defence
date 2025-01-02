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
    private RenderTexture permanent_texture_buffer;
    public Camera residue_camera;
    public Shader photo_of_residue_shader;

    /* adding each piece to the mesh is slow, we need to add many pieces at once, after they accumulate as simple game objects */
    public List<Leaving_persistent_residue_on_texture> batched_residues = new List<Leaving_persistent_residue_on_texture>();


    public int max_batch_amount = 200;
    private int captured_layer;

    private void Awake() {
        
        clear_textures();
        
        residue_camera.enabled = false;
        captured_layer = LayerMask.NameToLayer("litter");

        permanent_texture_buffer = new RenderTexture(
            permanent_texture.width,
            permanent_texture.height,
            permanent_texture.depth,
            permanent_texture.graphicsFormat
        );
    }

    private void clear_textures() {
        RenderTexture previous_texture = UnityEngine.RenderTexture.active;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = previous_texture;
        //permanent_texture.clear();
        //screen_texture.clear();
    }

    private void OnDestroy() {
        //clear_textures();
    }

    
    
    public void add_piece(
        Leaving_persistent_residue_on_texture in_residue
    ) {
        batched_residues.Add(in_residue);
        if (batched_residues.Count > max_batch_amount) {
            fix_residue_on_texture();
        }
    }

    
    [ContextMenu("fix_residue_on_texture")]
    public void fix_residue_on_texture() {
        foreach (var piece in batched_residues) {
            piece.sprite_renderer.gameObject.layer = captured_layer;
        }
        take_photo_of_residue();
        foreach (var piece in batched_residues) {
            piece.destroy_object();
        }
        batched_residues.Clear();
    }
    
    private void take_photo_of_residue() {
        residue_camera.enabled = true;
        residue_camera.targetTexture = screen_texture;
        
        //residue_camera.clearFlags = CameraClearFlags.SolidColor;
        //residue_camera.backgroundColor = Color.clear;
        
        residue_camera.Render();
        //residue_camera.RenderWithShader(photo_of_residue_shader,"");
        residue_camera.targetTexture = null;
        
        
        
        residue_camera.enabled = false;
        RenderTexture.active = null;
        
        Texture_drawer.instance.draw_texture_on_texture(
            permanent_texture_buffer,
            permanent_texture,
            screen_texture
        );
    }

    

    



}

}
