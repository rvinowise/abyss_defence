using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;
using System.Linq;

namespace rvinowise.unity.effects.persistent_residue {

public class Leaving_persistent_unique_sprite_residue: 
MonoBehaviour 
,ILeaving_persistent_residue
{

    public int max_images = 1500;
    
    public SpriteRenderer residue_prefab;

    [HideInInspector]
    private List<Leaving_persistent_sprite_residue> children = new List<Leaving_persistent_sprite_residue>(); 

    private SpriteRenderer sprite_renderer;
    /* private SpriteResolver sprite_resolver;
    private SpriteLibrary sprite_library; */

    private void Awake() {
        sprite_renderer = GetComponent<SpriteRenderer>();
        /* sprite_resolver = GetComponent<SpriteResolver>();
        sprite_library = GetComponent<SpriteLibrary>(); */
    }


    private void Start() {
        children = 
            GetComponentsInChildren<Leaving_persistent_sprite_residue>().
            Where(component => component != this).
            ToList();
 
    }

    
    public void leave_persistent_residue() {
        leave_persistent_image();

        foreach(var child in children) {
            child.leave_persistent_residue();
        }
    }

    public void leave_persistent_image( ) {
        SpriteRenderer residue = residue_prefab.instantiate<SpriteRenderer>();
        residue.transform.position = transform.position;
        residue.transform.set_z(Persistent_residue_router.instance.get_next_depth());
        residue.transform.SetParent(Persistent_residue_router.instance.transform, false);
        residue.transform.rotation = transform.rotation;
        //residue.transform.scale = transform.scale;

        residue.sprite = sprite_renderer.sprite;
        residue.flipX = sprite_renderer.flipX;
        residue.flipY = sprite_renderer.flipY;

        
    }
}

}