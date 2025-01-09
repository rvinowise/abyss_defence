using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.pooling;
using rvinowise.unity.geometry2d;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Comparers;

/* this is the interface to draw persistent residu on some of the tiling textures */

namespace rvinowise.unity {
public class Persistent_residue_all_textures: 
MonoBehaviour,
IPersistent_residue_holder {

    public Persistent_residue_texture_holder texture_holder_prefab;
    public List<List<Persistent_residue_texture_holder>> textures_matrix = new List<List<Persistent_residue_texture_holder>>();
    public Camera residue_camera;

    /* adding each piece to the mesh is slow, we need to add many pieces at once, after they accumulate as simple game objects */
    public List<Leaving_persistent_residue_on_texture> batched_residues = new List<Leaving_persistent_residue_on_texture>();
    
    public int rows_amount = 5;
    public int columns_amount = 5;

    public int max_batch_amount = 10;
    public static int captured_layer;

    public static Persistent_residue_all_textures instance;
    private void Awake() {
        Contract.Requires(instance == null, "singleton");
        instance = this; 
        
        clear_textures();
        init_texture_holders();
        
        residue_camera.enabled = false;
        captured_layer = LayerMask.NameToLayer("litter");
        
    }

    private void init_texture_holders() {
        
        Vector3 texture_mesh_size = texture_holder_prefab.transform.localScale;
        for (int i_row = 0; i_row < rows_amount; i_row++) {
            textures_matrix.Add(new List<Persistent_residue_texture_holder>());
            var current_row = textures_matrix.Last();
            for (int i_column = 0; i_column < columns_amount; i_column++) {
                var texture_holder = Instantiate(
                    texture_holder_prefab,
                    new Vector3(
                        transform.position.x+i_column*texture_mesh_size.x, 
                        transform.position.y-i_row*texture_mesh_size.y
                    ),
                    Quaternion.identity,
                    transform
                );
                texture_holder.name = $"texture_holder {i_column}:{i_row}";
                current_row.Add(texture_holder);
            }
        }
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

    public bool is_residue_on_texture_holder(
        Persistent_residue_texture_holder texture_holder,
        Leaving_persistent_residue_on_texture residue
    ) {
        Vector2 distance = residue.transform.position - texture_holder.transform.position;
        Vector2 residue_extents = residue.sprite_renderer.bounds.extents;
         if (
             (Math.Abs(distance.x) < texture_holder.transform.localScale.x/2+residue_extents.x)
             &&
             (Math.Abs(distance.y) < texture_holder.transform.localScale.y/2+residue_extents.y)
         ) {
             return true;
         }
        // if (
        //     (Math.Abs(distance.x) < texture_holder.transform.localScale.x/2)
        //     &&
        //     (Math.Abs(distance.y) < texture_holder.transform.localScale.y/2)
        // ){
        //     return true;
        // }
        // Vector2 residue_extents = residue.sprite_renderer.bounds.extents;
        // if (
        //     (Math.Abs(distance.x)+2 < texture_holder.transform.localScale.x/2)
        //     &&
        //     (Math.Abs(distance.y)+2 < texture_holder.transform.localScale.y/2)
        // ) {
        //     return true;
        // }
        return false;
    }
    
    private readonly List<Persistent_residue_texture_holder> texture_holders_with_residue = new List<Persistent_residue_texture_holder>();
    
    public List<Persistent_residue_texture_holder> get_texture_holders_with_residue(
        Leaving_persistent_residue_on_texture residue
    ) {
        texture_holders_with_residue.Clear();
        foreach (var row in textures_matrix) {
            foreach (var texture_holder in row) {
                if (is_residue_on_texture_holder(texture_holder, residue)) {
                    texture_holders_with_residue.Add(texture_holder);
                }
            }
        }
        return texture_holders_with_residue;
    }

    private ISet<Persistent_residue_texture_holder> texture_holders_with_batched_residue
        = new HashSet<Persistent_residue_texture_holder>();
    
    public void add_piece(
        Leaving_persistent_residue_on_texture in_residue
    ) {
        batched_residues.Add(in_residue);
        
#if RVI_DEBUG
        Debug.Log($"RESIDUE: Persistent_residue_all_textures:add_piece #{in_residue.number} at {in_residue.transform.position.x}, {in_residue.transform.position.y}");
#endif
        
        foreach (var texture_holder in get_texture_holders_with_residue(in_residue)) {
            texture_holder.add_piece(in_residue);
            texture_holders_with_batched_residue.Add(texture_holder);
        }
        if (batched_residues.Count >= max_batch_amount) {
            fix_residue_on_texture();
        }
    }

    
    [ContextMenu("fix_residue_on_texture")]
    public void fix_residue_on_texture() {
        Debug.Log("RESIDUE: Persistent_residue_all_textures::fix_residue_on_texture");
        // foreach (var piece in batched_residues) {
        //     piece.sprite_renderer.gameObject.layer = captured_layer;
        // }
        take_photos_of_residue();
        foreach (var piece in batched_residues) {
#if RVI_DEBUG
            Debug.Log($"RESIDUE: piece# {piece.number} is destroyed by Persistent_residue_all_textures");
#endif
            piece.destroy_object();
        }
        Debug.Log($"RESIDUE: Persistent_residue_all_textures: destryed {batched_residues.Count} residues");
        texture_holders_with_batched_residue.Clear();
        batched_residues.Clear();
        
    }
    
    private void take_photos_of_residue() {
        residue_camera.enabled = true;

        foreach (var texture_holder in texture_holders_with_batched_residue) {
            texture_holder.fix_residue_on_texture();
        }
        
        residue_camera.enabled = false;
        residue_camera.targetTexture = null;
        RenderTexture.active = null;
        
    }

    

    



}

}
