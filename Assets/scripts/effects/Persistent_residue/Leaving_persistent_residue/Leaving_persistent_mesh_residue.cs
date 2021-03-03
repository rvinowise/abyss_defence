using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.Serialization;

namespace rvinowise.unity.effects.persistent_residue {

public class Leaving_persistent_mesh_residue: 
MonoBehaviour
,ILeaving_persistent_residue
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

}

}