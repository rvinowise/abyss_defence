using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class Savings_menu : MonoBehaviour {
    
    public Main_menu main_menu;
    public Button back_button;
    public Transform background;
    
    public GridLayoutGroup savings_table;
    public Saving_cell saving_cell_prefab;

    public Loading_indicator loading_indicator;
    public Scene_transition_effect scene_transition;
    
    private void Awake() {
        back_button.onClick.AddListener(switch_to_main_menu);
        loading_indicator.deactivate();
        scene_transition.deactivate();
    }

    private void Start() {
        
    }

    public void load_savings_into_table(
        IList<Saved_game> savings    
    ) {
        foreach (var saving in savings) {
            add_saving_to_table(saving);
        }
    }

    private void add_saving_to_table(Saved_game saving) {
        var saving_cell = Instantiate(saving_cell_prefab, savings_table.transform);
        saving_cell.load_saving_from_file(saving);
        saving_cell.savings_menu = this;
    }

    private void switch_to_main_menu() {
        main_menu.activate();
        this.deactivate();
    }

    public void load_saving(string scene_title) {
        StartCoroutine(process_of_loading_scene(scene_title));
    }

    private AsyncOperation loading_scene;
    private Scene this_scene;
    private IEnumerator process_of_loading_scene(string scene_name) {
        this_scene = SceneManager.GetActiveScene();
        loading_scene = SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
        
        if (loading_scene == null) {
            Debug.LogError($"trying to load a scene with non-existing name {scene_name}");
            yield break;
        }
        
        loading_scene.allowSceneActivation = true;
        loading_indicator.activate();
        
        while (!loading_scene.isDone) {
            loading_indicator.set_loaded_amount(loading_scene.progress);
            yield return new WaitForEndOfFrame();
        }
        loading_indicator.set_loaded_amount(loading_scene.progress);
        loading_indicator.show_button_to_start_game(launch_loaded_scene);
        
        background.deactivate();
        Time.timeScale = 0;
        scene_transition.start_transition_to_loaded_scene(loading_scene);
        this.deactivate();

    }

    private void launch_loaded_scene() {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync(this_scene);
    }
    
}
