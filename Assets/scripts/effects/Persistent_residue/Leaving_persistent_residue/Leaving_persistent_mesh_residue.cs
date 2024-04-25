using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity {

public class Leaving_persistent_mesh_residue: 
    MonoBehaviour
    ,ILeaving_persistent_residue
    ,IDestructible
{

    public List<Mesh> left_meshes; 
    public Material left_material;


    private Persistent_residue_mesh_holder holder;

    public void Awake() {
        //holder = GetComponent<Persistent_residue_mesh_holder>();
    }

    void Start() {
        holder = Persistent_residue_router.instance.get_holder_for_material(
            left_material
        );
    }

    public void leave_persistent_residue() {

        holder.add_piece(
            left_meshes.get_random_item(),
            transform
        );
    }

    public void on_start_dying() {
        leave_persistent_residue();
    }

}

}