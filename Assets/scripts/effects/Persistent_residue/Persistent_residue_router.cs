

using System.Collections.Generic;
using geometry2d;
using rvinowise.rvi.contracts;
using UnityEngine;

namespace effects.persistent_residue {

public class Persistent_residue_router: MonoBehaviour {

    public Persistent_residue_holder holder_prefab;

    [HideInInspector]
    public Dictionary<Texture2D, Persistent_residue_holder> texture_to_holder = 
        new Dictionary<Texture2D, Persistent_residue_holder>();

    public static Persistent_residue_router instance;

    private void Awake() {
        Contract.Requires(instance == null, "Persistent_residue_router is a singleton");
        instance = this;
    }


    public Persistent_residue_holder get_holder_for_texture(
        Texture2D texture,
        Dimension in_left_texture_dimension,
        int in_max_images,
        int in_n_frames
    ) {
        Persistent_residue_holder holder;
        if (!texture_to_holder.TryGetValue(texture, out holder)) {
            holder = create_holder_for_texture(
                texture,
                in_left_texture_dimension,
                in_max_images,
                in_n_frames
            );
            texture_to_holder.Add(texture,holder);
        }
        return holder;
    }

    private Persistent_residue_holder create_holder_for_texture(
        Texture2D texture,
        Dimension in_left_texture_dimension,
        int in_max_images,
        int in_n_frames
    ) {
        Persistent_residue_holder new_holder = GameObject.Instantiate(holder_prefab);
        new_holder.transform.parent = this.transform;
        new_holder.max_residue = in_max_images;
        new_holder.n_frames_x = in_n_frames;
        MeshRenderer mesh_renderer = new_holder.GetComponent<MeshRenderer>();
        mesh_renderer.material.mainTexture = texture;
        new_holder.left_texture_dimension = in_left_texture_dimension;
        return new_holder;
    }
}

}