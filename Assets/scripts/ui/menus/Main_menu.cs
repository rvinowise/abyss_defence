using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class Main_menu : MonoBehaviour
{
    public string first_scene = "arena";
    public Transform settings_menu;
    public Savings_menu savings_menu;

    public Button start_new_game_button;
    public Button load_game_button;
    public Button continue_game_button;
    public Button settings_button;
    public Button quit_button;

    public Save_load_game save_load_game;
    
    private void Awake() {
        start_new_game_button.onClick.AddListener(start_new_game);
        continue_game_button.onClick.AddListener(continue_from_last_saving);
        load_game_button.onClick.AddListener(switch_to_savings);
        settings_button.onClick.AddListener(switch_to_settings);
        quit_button.onClick.AddListener(quit_game);
    }

    private IList<Saved_game> saved_games;
    private void Start() {
        saved_games = save_load_game.load_all_checkpoints_from_files();
        if (saved_games.Any()) {
            continue_game_button.gameObject.SetActive(true);
            load_game_button.gameObject.SetActive(true);
        }
        else {
            continue_game_button.gameObject.SetActive(false);
            load_game_button.gameObject.SetActive(false);
        }
    }

    public void start_new_game() {
        SceneManager.LoadScene(first_scene);
    }

    

    private void switch_to_savings() {
        savings_menu.gameObject.SetActive(true);
        savings_menu.load_savings_into_table(saved_games);
        gameObject.SetActive(false);
    }

    
    private void continue_from_last_saving() {
        
    }

    private void switch_to_settings() {
        settings_menu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    
    private void quit_game() {
        Debug.Log("quit game");
        Application.Quit();
    }
   
}
