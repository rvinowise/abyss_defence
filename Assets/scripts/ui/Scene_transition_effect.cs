using Animancer;
using rvinowise.contracts;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using UnityEngine;

namespace rvinowise.unity {

public class Scene_transition_effect : MonoBehaviour
{

    private AsyncOperation loading_scene;
    private AnimancerComponent animancer;
    public AnimationClip transition_clip;

    void Awake() 
    {
        animancer = GetComponent<AnimancerComponent>();
        
    }

    public void start_transition(AsyncOperation in_scene) {
        loading_scene = in_scene;
        gameObject.SetActive(true);
        gameObject.transform.parent = null;
        DontDestroyOnLoad(gameObject);
        animancer.play_from_scratch(transition_clip, null);
    }

    [called_in_animation]
    public void on_scene_changed() {
        Contract.Requires(
            is_scene_loaded(),
            "switching scene animation should play after scene is loaded completely"
        );
        loading_scene.allowSceneActivation = true;
    }

    [called_in_animation]
    void on_effect_finished() {
        Destroy(gameObject);
    }

    private bool is_scene_loaded() {
        return (
            (loading_scene != null) && 
            (loading_scene.progress >= 0.9f)
        );
    }
}

}