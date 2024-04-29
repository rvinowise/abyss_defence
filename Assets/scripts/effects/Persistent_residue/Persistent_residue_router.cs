

using System.Collections.Generic;
using rvinowise.contracts;
using UnityEngine;


namespace rvinowise.unity {

public class Persistent_residue_router: MonoBehaviour {

    public Persistent_residue_sprite_holder sprite_holder_prefab;
    public Persistent_residue_mesh_holder material_holder_prefab;

    private readonly Dictionary<Sprite, Persistent_residue_sprite_holder> sprite_to_holder = 
        new Dictionary<Sprite, Persistent_residue_sprite_holder>();

    private readonly Dictionary<Material, Persistent_residue_mesh_holder> material_to_holder = 
        new Dictionary<Material, Persistent_residue_mesh_holder>();

    public static Persistent_residue_router instance;

    public float last_depth;
    private const float depth_increment = 0.0001f;
    private void Awake() {
        Contract.Requires(instance == null, "Persistent_residue_router is a singleton");
        instance = this;
    }

    public float get_next_depth() {
        last_depth-=depth_increment;
        return last_depth;
    }

    private const int max_images_default = 1500;
    public Persistent_residue_mesh_holder provide_holder_for_material(
        Material in_material,
        int in_max_images = max_images_default
    ) {
        if (!material_to_holder.TryGetValue(in_material, out var holder)) {
            holder = create_holder_for_material(
                in_material,
                in_max_images
            );
            material_to_holder.Add(in_material, holder);
        }
        return holder;
    }

    private Persistent_residue_mesh_holder create_holder_for_material(
        Material in_material,
        int in_max_images
    ) {
        Persistent_residue_mesh_holder new_holder = GameObject.Instantiate(material_holder_prefab);
        new_holder.transform.SetParent(this.transform, false);
        new_holder.max_residue = in_max_images;
        MeshRenderer mesh_renderer = new_holder.GetComponent<MeshRenderer>();
        mesh_renderer.material = in_material;

        return new_holder;
    }

    public Persistent_residue_sprite_holder provide_holder_for_texture(
        Sprite sprite,
        int in_max_images,
        int in_n_frames
    ) {
        if (!sprite_to_holder.TryGetValue(sprite, out var holder)) {
            holder = create_holder_for_texture(
                sprite,
                in_max_images,
                in_n_frames
            );
            sprite_to_holder.Add(sprite,holder);
        }
        return holder;
    }

    private Persistent_residue_sprite_holder create_holder_for_texture(
        Sprite sprite,
        int in_max_images,
        int in_n_frames
    ) {
        Persistent_residue_sprite_holder new_holder = GameObject.Instantiate(sprite_holder_prefab);
        new_holder.transform.SetParent(this.transform, false);
        
        new_holder.init_for_sprite(sprite, in_n_frames, in_max_images);
        
        return new_holder;
    }

    /* private add_layer_of_residue(GameObject prefab, int max_images) {
        Persistent_residue_sprite_holder new_holder = GameObject.Instantiate(sprite_holder_prefab);
        new_holder.transform.SetParent(this.transform, false);
        new_holder.max_residue = in_max_images;
    } */
}

}