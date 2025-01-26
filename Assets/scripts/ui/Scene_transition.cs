using System.Collections;
using Animancer;
using rvinowise.unity.extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity {
public class Scene_transition : 
    MonoBehaviour
    ,IInput_receiver
{
    public string target_scene;
    public Scene_transition_effect effect;
    public TextMeshProUGUI text;
    private AsyncOperation loading_scene;
    private bool touching_player;

    public AnimancerComponent text_animator;
    public AnimationClip show_text_clip;
    public AnimationClip hide_text_clip;
    public Loading_indicator loading_indicator;

    void Awake() {
        if (effect == null) {
            effect = GetComponentInChildren<Scene_transition_effect>(true);
        }
        effect.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }
 

    void OnTriggerEnter2D(Collider2D collider2d) {
        if (is_player(collider2d.gameObject)) {
            if (loading_scene == null) {
                StartCoroutine(start_loading_scene(target_scene));
            }
            touching_player = true;
            Player_input.instance.add_input_receiver(this);
            if (is_ready_to_switch_scenes()) {
                show_text();
            }
        }
    }
    void OnTriggerExit2D(Collider2D collider2d) {
        if (is_player(collider2d.gameObject)) {
            touching_player = false;
            is_finished = true;
            hide_text();
        }
    }

 

    private bool is_ready_to_switch_scenes() {
        return (
            scene_is_loaded() && 
            touching_player
        );
    }

    public bool is_finished { get; set; }

    public bool process_input() {
        if (
            player_wants_to_switch_scenes()&&
            is_ready_to_switch_scenes()
        ) {
            go_to_next_scene();
            is_finished = true;
            return true;
        }
        return false;
    }

    private void go_to_next_scene() {
        effect.activate();
        effect.start_transition(loading_scene);
        hide_text();
    }
    private bool player_wants_to_switch_scenes() {
        return Input.GetKeyDown(KeyCode.Return);
    }

    private bool is_player(GameObject in_go) {
        return Player_input.instance.player.GetComponent<Humanoid>().damageable_body.gameObject == in_go;
    }

    private IEnumerator start_loading_scene(string scene_name)
    {
        loading_scene = SceneManager.LoadSceneAsync(scene_name);
        loading_scene.allowSceneActivation = false;
        loading_indicator.activate();
        
        while(loading_scene.progress < 0.89f)
        {
            Debug.Log($"[scene]:{scene_name} [load progress]: {loading_scene.progress}");
            loading_indicator.set_loaded_amount(loading_scene.progress);
            yield return new WaitForEndOfFrame();
        }
        loading_indicator.set_loaded_amount(loading_scene.progress);
        loading_indicator.show_button_to_start_game(go_to_next_scene);
        
        if (is_ready_to_switch_scenes()) {
            show_text();
        }
    }

    private void show_text() {
        text.gameObject.SetActive(true);
        text_animator.play_from_scratch(show_text_clip, null);
    }
    private void hide_text() {
        //text_animator.play_from_scratch(hide_text_clip, null);
    }

    private bool scene_is_loaded() {
        return (
            (loading_scene != null) && 
            (loading_scene.progress >= 0.9f)
        );
    }

    

}


}