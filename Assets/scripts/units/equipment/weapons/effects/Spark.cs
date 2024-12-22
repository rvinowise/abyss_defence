using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using UnityEngine;
using UnityEngine.Serialization;


namespace rvinowise.unity {

public class Spark: MonoBehaviour {

    public Animator animator;
    //private readonly int animation_hash = Animator.StringToHash("shoot and hide");

    public Transform muzzle;

    // private void Awake() {
    //     muzzle = transform.parent;
    // }

    public void activate_flash() {
        transform.copy_physics_from(muzzle);
        gameObject.SetActive(true);
        //animator.Play(animation_hash);
    }

    public void activate_flash_detached() {
        transform.SetParent(null);
        transform.copy_physics_from(muzzle);
        gameObject.SetActive(true);
    }
    
    public bool is_active() {
        return gameObject.activeSelf;
    }

    
    [called_in_animation]
    public void on_animation_ended() {
        gameObject.SetActive(false);
    }
    
}

}