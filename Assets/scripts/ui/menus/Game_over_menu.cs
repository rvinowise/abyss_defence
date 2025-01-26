using System;
using System.Collections;
using Animancer;
using rvinowise.unity.extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace rvinowise.unity {
public class Game_over_menu : 
    MonoBehaviour
    ,IInput_receiver
{
    public AnimationClip gameover_clip;
    public Scene_transition_effect scene_transition_effect;
    public Unfocusing_game_effect unfocusing_game_effect;
    
    public TextMeshProUGUI gameover_text;
    public TextMeshProUGUI loaded_percent;
    public RectTransform loading_info;
    public Button restart_button;
    public Button quit_button;
    public Button main_menu_button;
    
    
    private AsyncOperation loading_scene;
    public AnimancerComponent animancer;


    public Damage_receiver damageable_player;
    
    
    void Awake() {
        if (scene_transition_effect == null) {
            scene_transition_effect = GetComponentInChildren<Scene_transition_effect>(true);
        }
        gameover_text.gameObject.SetActive(false);
        
        restart_button.gameObject.SetActive(false);
        restart_button.interactable = false;
        restart_button.onClick.AddListener(on_player_wants_to_restart);
        
        quit_button.gameObject.SetActive(false);
        quit_button.onClick.AddListener(on_player_wants_to_quit);
        
        main_menu_button.gameObject.SetActive(false);
        main_menu_button.onClick.AddListener(on_player_wants_main_menu);
        
        loading_info.gameObject.SetActive(false);
        
    }

    private void Start() {
        damageable_player = Player_input.find_player().GetComponent<Humanoid>().damageable_body;
        damageable_player.on_destroyed += finish_game;
    }

    
    public void finish_game(Damage_receiver player) {
        if (loading_scene == null) {
            StartCoroutine(start_loading_scene(SceneManager.GetActiveScene()));
        }
        show_gameover_text();
        
        Player_input.instance.add_input_receiver(this);
    }
    
    public bool is_finished { get; set; }
    public bool process_input() {
        
        if (Input.GetKey(KeyCode.Return)) {
            on_player_wants_to_restart();
            is_finished = true;
            return true;
        } 
        if (Input.GetKey(KeyCode.Escape)) {
            on_player_wants_to_quit();
            is_finished = true;
            return true;
        }
        return true; // so that all input outside this menu will be blocked 
    }

    

    private void on_player_wants_to_restart() {
        if (is_scene_loaded()) {
            scene_transition_effect.start_transition(loading_scene);
            gameover_text.gameObject.SetActive(false);
        }
    }

    private void on_player_wants_to_quit() {
// #if UNITY_STANDALONE
         Application.Quit();
// #elif UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
// #endif
    }

    public string main_menu_scene_name;
    private void on_player_wants_main_menu() {
        SceneManager.LoadScene(main_menu_scene_name);
    }
    

    private IEnumerator start_loading_scene(UnityEngine.SceneManagement.Scene scene)
    {
        loading_scene = SceneManager.LoadSceneAsync(scene.buildIndex);

        loading_scene.allowSceneActivation = false;

        while(loading_scene.progress < 0.9f - float.Epsilon) // loading is always up to 0.9, the rest is allowing activation
        {
            Debug.Log($"[scene]:{scene.name} [load progress]: {this.loading_scene.progress}");
            loaded_percent.text = $"{loading_scene.progress}%";
            yield return null;
        }
        loaded_percent.text = $"100%";
        enable_restart_offer();
    }

    private void show_gameover_menu() {
        //gameover_appearing_state.IsPlaying = false;
        restart_button.gameObject.SetActive(true);
        loading_info.gameObject.SetActive(true);
        main_menu_button.gameObject.SetActive(true);
        quit_button.gameObject.SetActive(true);
        
    }

    private AnimancerState gameover_appearing_state;
    private void show_gameover_text() {
        gameover_text.gameObject.SetActive(true);
        gameover_appearing_state = animancer.play_from_scratch(gameover_clip, show_gameover_menu);
        unfocusing_game_effect.start_unfocusing(); 
    }
    
    
    private void enable_restart_offer() {
        restart_button.interactable = true;
    }


    private bool is_scene_loaded() {
        return (
            (loading_scene != null) && 
            (loading_scene.progress >= 0.9f)
        );
    }

    

}


}