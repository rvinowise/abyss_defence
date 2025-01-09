using System;
using System.Collections.Generic;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.pooling;
using rvinowise.unity.geometry2d;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

/* this is one texture, there are many of them, tiling the floor. otherwise it's too slow to draw on one huge texture */

namespace rvinowise.unity {

[RequireComponent(typeof(MeshRenderer))]
public class Persistent_residue_texture_holder: 
MonoBehaviour,
IPersistent_residue_holder
{

    public RenderTexture screen_texture;
    public RenderTexture permanent_texture;
    private RenderTexture permanent_texture_buffer;
    public Camera residue_camera;

    /* adding each piece to the mesh is slow, we need to add many pieces at once, after they accumulate as simple game objects */
    public List<Leaving_persistent_residue_on_texture> batched_residues = new List<Leaving_persistent_residue_on_texture>();

    private int captured_layer;

    private void Awake() {
        
        clear_textures();
        
        residue_camera.enabled = false;
        captured_layer = LayerMask.NameToLayer("litter");

        //since this component is a multiplied prefab -- we need to make its own instances of textures 
        screen_texture = new RenderTexture(
            screen_texture.width,
            screen_texture.height,
            screen_texture.depth,
            screen_texture.graphicsFormat
        );
        permanent_texture = new RenderTexture(
            permanent_texture.width,
            permanent_texture.height,
            permanent_texture.depth,
            permanent_texture.graphicsFormat
        );
        var mesh_renderer = GetComponent<MeshRenderer>();
        mesh_renderer.material.mainTexture = permanent_texture;
        
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

    public List<String> batched_residues_debug = new List<String>();
    
    
    public void add_piece(
        Leaving_persistent_residue_on_texture in_residue
    ) {
        batched_residues.Add(in_residue);
        batched_residues_debug.Add(in_residue.ToString());
    }

    
    [ContextMenu("fix_residue_on_texture")]
    public void fix_residue_on_texture() {
        var i_piece_debug = 0;  
        foreach (var piece in batched_residues) {
            if (!piece) {
                var broken_object_debug = batched_residues_debug[i_piece_debug];
                Debug.LogError($"fix_residue_on_texture: {piece} has its sprite_renderer destroyed");
                Debug.Break();
            } else {
                piece.sprite_renderer.gameObject.layer = captured_layer;
            }
            i_piece_debug++;
        }
        take_photo_of_residue();
        batched_residues.Clear();
        batched_residues_debug.Clear();
    }
    
    private void take_photo_of_residue() {
        residue_camera.targetTexture = screen_texture;
        residue_camera.transform.position = transform.position; 
            
        residue_camera.Render();
        
        Texture_drawer.instance.draw_texture_on_texture(
            permanent_texture_buffer,
            permanent_texture,
            screen_texture
        );
    }

    

    



}

}
