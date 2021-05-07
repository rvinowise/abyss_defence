using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.extensions.pooling;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.unity.geometry2d;
using rvinowise.unity.effects.physics;
using UnityEngine.Experimental.U2D.Animation;
using Random = UnityEngine.Random;


namespace rvinowise.unity.units.parts.weapons.guns.common {

public class Gun_shell : MonoBehaviour {
    
    private Leaving_persistent_sprite_residue residue_leaver;
    private Trajectory_flyer trajectory_flyer;
    private Pooled_object pooled_object;
    private SpriteResolver sprite_resolver; 

    void Awake() {
        residue_leaver = GetComponent<Leaving_persistent_sprite_residue>();
        
        trajectory_flyer = GetComponent<Trajectory_flyer>();
        trajectory_flyer.on_fell_on_the_ground.AddListener(leave_residue);

        pooled_object = GetComponent<Pooled_object>();

        sprite_resolver = GetComponent<SpriteResolver>();
    }

    void OnEnable() {
        trajectory_flyer.enabled = true;
    }

    private void leave_residue() {
        residue_leaver.leave_persistent_image(
            0//(int)Math.Round((double)Random.Range(0,1))
        );
        pooled_object.destroy();
    } 
  

}
}