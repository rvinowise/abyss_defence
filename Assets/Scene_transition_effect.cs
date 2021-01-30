using System.Collections;
using System.Collections.Generic;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;
using UnityEngine;

namespace rvinowise.unity {

public class Scene_transition_effect : MonoBehaviour
{

    private AsyncOperation loading_scene;
    private Animator animator;

    void Awake() 
    {
        animator = GetComponent<Animator>();
        
    }

    public void start_transition(AsyncOperation in_scene) {
        loading_scene = in_scene;
        gameObject.SetActive(true);
        gameObject.transform.parent = null;
        DontDestroyOnLoad(gameObject);
        animator.SetTrigger("transition");
    }

    [called_in_animation]
    public void on_scene_changed() {
        Contract.Requires(
            scene_is_loaded(),
            "switching scene animation should play after scene is loaded completely"
        );
        loading_scene.allowSceneActivation = true;
    }

    [called_in_animation]
    void on_effect_finished() {
        Destroy(gameObject);
    }

    private bool scene_is_loaded() {
        return (
            (loading_scene != null) && 
            (loading_scene.progress >= 0.9f)
        );
    }
}

}