using System;
using System.Collections.Generic;
using System.Linq;
using Animancer;
using rvinowise.unity.effects.trails.mesh_impl;
using rvinowise.unity.geometry2d;
using UnityEngine;
using rvinowise.unity.extensions;
using UnityEditor;
using UnityEngine.Serialization;


namespace rvinowise.unity {

// public static class Animation_clip {
//     public static List<Sprite> GetSpritesFromClip(AnimationClip clip)
//     {
//         var sprites = new List<Sprite> ();
//         if(clip != null)
//         {
//             foreach(var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
//             {
//                 var keyframes = AnimationUtility.GetObjectReferenceCurve (clip, binding);
//                 foreach(var frame in keyframes)
//                 {
//                     sprites.Add((Sprite) frame.value);
//                 }
//             }
//         }
//         return sprites;
//     }
// }

public class Opening_box : MonoBehaviour {

    public AnimancerComponent animancer;
    public AnimationClip opening_clip;
    public List<Sprite> animation_frames = new List<Sprite>();
    public SpriteRenderer lid_sprite_renderer;
    
    public Transform content;
    
    void Awake() {
        if (!animancer) {
            animancer = GetComponent<AnimancerComponent>();
        }
        //animation_frames = Animation_clip.GetSpritesFromClip(opening_clip);
    }

    private void Start() {
    }

    public bool is_opening;
    public void open() {
        is_opening = true;
        animancer.play_from_scratch(opening_clip, on_opening_finished);
    }

    public bool is_closed() {
        return(
            (!is_opening)&&
            (lid_sprite_renderer.sprite == animation_frames.First())
            );
    }

    private void on_opening_finished() {
        //lid_sprite_renderer.sprite = opening_clip.
    }
    
    private void Update() {
        
    }


}


}