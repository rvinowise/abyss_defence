using System;
using System.Collections;
using Animancer;
using rvinowise.unity.extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity {
public class Restarting_game : 
    MonoBehaviour
    ,IInput_receiver
{
    public Scene_transition_effect effect;
    public TextMeshProUGUI text;
    private AsyncOperation loading_scene;
    public AnimancerComponent animancer;

    public AnimationClip gameover_clip;

    protected Damage_receiver damageable_player;
    
    
    void Awake() {
        if (effect == null) {
            effect = GetComponentInChildren<Scene_transition_effect>(true);
        }
        text.gameObject.SetActive(false);
        
    }

    private void Start() {
        damageable_player = Player_input.instance.player.GetComponent<Humanoid>().damageable_body;
        damageable_player.on_destroyed += finish_game;
    }

    
    public void finish_game(Damage_receiver player) {
        if (loading_scene == null) {
            StartCoroutine(start_loading_scene(SceneManager.GetActiveScene()));
        }
        show_gameover_text();
        Player_input.instance.add_input_receiver(this);
    }
    
    public bool is_finished { get; private set; }
    public bool process_input() {
        
        if (Input.GetKey(KeyCode.Return)) {
            on_player_wants_to_restart();
            is_finished = true;
            return true;
        }
        return false;
    }

    

    public void on_player_wants_to_restart() {
        if (is_scene_loaded()) {
            effect.start_transition(loading_scene);
            text.gameObject.SetActive(false);
        }
    }

    

    private IEnumerator start_loading_scene(UnityEngine.SceneManagement.Scene scene)
    {
        loading_scene = SceneManager.LoadSceneAsync(scene.buildIndex);

        loading_scene.allowSceneActivation = false;

        while (!loading_scene.isDone)
        //while(loading_scene.progress < 0.9f)
        {
            Debug.Log($"[scene]:{scene.name} [load progress]: {this.loading_scene.progress}");
            yield return null;
        }
        if (is_scene_loaded()) {
            show_restart_offer();
        }
    }

    private void show_gameover_text() {
        text.gameObject.SetActive(true);
        animancer.play_from_scratch(gameover_clip, null);
    }
    
    private void show_restart_offer() {
        animancer.play_from_scratch(gameover_clip, null);
    }


    private bool is_scene_loaded() {
        return (
            (loading_scene != null) && 
            (loading_scene.progress >= 0.9f)
        );
    }

    

}


}