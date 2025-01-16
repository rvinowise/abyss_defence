using Animancer;
using rvinowise.contracts;
using rvinowise.unity.extensions;
using rvinowise.unity.extensions.attributes;
using UnityEngine;

namespace rvinowise.unity {

/* e.g, when a menu appears, this effect blackens the screen, to focus the attention to the menu */
public class Unfocusing_game_effect : MonoBehaviour
{

    private AnimancerComponent animancer;
    public AnimationClip unfocusing_clip;

    void Awake() 
    {
        animancer = GetComponent<AnimancerComponent>();
        
    }

    private AnimancerState unfocusing_state;
    public void start_unfocusing() {
        unfocusing_state = animancer.play_from_scratch(unfocusing_clip, null);
    }

    private void freeze_animation() {
        unfocusing_state.IsPlaying = false;
    }

    [called_in_animation]
    public void on_scene_changed() {
        
    }

    [called_in_animation]
    void on_effect_finished() {
    }


}

}