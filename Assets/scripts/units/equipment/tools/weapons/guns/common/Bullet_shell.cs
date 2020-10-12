using System;
using System.Collections;
using System.Collections.Generic;
using effects.persistent_residue;
using extensions.pooling;
using UnityEngine;
using rvinowise;
using geometry2d;
using rvinowise.effects.physics;
using UnityEngine.Experimental.U2D.Animation;
using Random = UnityEngine.Random;


namespace rvinowise.units.parts.weapons.guns.common {

public class Bullet_shell : MonoBehaviour {
    
    private Leaving_persistent_residue residue_leaver;
    private Trajectory_flyer trajectory_flyer;
    private Pooled_object pooled_object;
    private SpriteResolver sprite_resolver; 

    void Awake() {
        residue_leaver = GetComponent<Leaving_persistent_residue>();
        
        trajectory_flyer = GetComponent<Trajectory_flyer>();
        trajectory_flyer.on_fell_on_the_ground.AddListener(leave_residue);
        trajectory_flyer.weight = 5f;

        pooled_object = GetComponent<Pooled_object>();

        sprite_resolver = GetComponent<SpriteResolver>();
    }

    private void leave_residue() {
        residue_leaver.leave_persistent_image(
            0//(int)Math.Round((double)Random.Range(0,1))
        );
        pooled_object.destroy();
    } 
  

}
}