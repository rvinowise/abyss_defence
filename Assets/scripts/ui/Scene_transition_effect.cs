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
        //gameObject.SetActive(true);
        gameObject.transform.parent = null; // otherwise dont_destroy_on_load won't work, it only preserves root objects
        DontDestroyOnLoad(gameObject);
        animancer.play_from_scratch(transition_clip, null);
    }

    [called_in_animation]
    public void on_scene_changed() {
        Contract.Requires(
            is_scene_loaded(),
            "switching scene animation should play after scene is loaded completely"
        );
        pause_game(); // present the new level in a paused state to avoid instant death
        loading_scene.allowSceneActivation = true;
    }

    [called_in_animation]
    void on_effect_finished() {
        unpause_game();
        Destroy(gameObject); // because this object is kept from the previous scene for a while
    }

    private bool is_scene_loaded() {
        return (
            (loading_scene != null) && 
            (loading_scene.progress >= 0.9f - float.Epsilon)
        );
    }

    private static float old_timescale;
    public static void pause_game() {
        old_timescale = Time.timeScale;
        Time.timeScale = 0;
    }
    public static void unpause_game() {
        Time.timeScale = old_timescale;
    }
}

}