using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;

using UnityEngine.Serialization;

namespace rvinowise.unity.effects.persistent_residue {
public class Persistent_residue_mask_holder:
MonoBehaviour,
IPersistent_residue_holder
{

    public int max_residue;
    private int last_quad_index;

    private Mesh mesh;
    private MeshFilter mesh_filter;
    private Vector3[] vertices;
    private int[] triangles;


    public void Awake() {
        mesh_filter = GetComponent<MeshFilter>();
    }

    public void Start() {
        //test
        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
        Texture texture = mesh_renderer.material.mainTexture;
        
        
        mesh = new Mesh();

        vertices = new Vector3[4 * max_residue];
        triangles = new int[6 * max_residue];

        mesh_filter.mesh = mesh;

    }


    public void add_piece(
        Mesh in_mesh,
        Transform in_transform
    ) {
        if (last_quad_index >= max_residue) {
            return;
        }


        CombineInstance[] combine = new CombineInstance[2];
        combine[0].mesh = mesh_filter.sharedMesh;
        combine[1].mesh = in_mesh;
        combine[0].transform = Matrix4x4.identity;
        combine[1].transform = in_transform.localToWorldMatrix;

        Mesh combined_mesh = new Mesh();
        combined_mesh.CombineMeshes(combine, true, true);

        mesh_filter.sharedMesh = combined_mesh;


    }



}

}
