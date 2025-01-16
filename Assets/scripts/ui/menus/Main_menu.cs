using System;
using rvinowise.unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class Main_menu : MonoBehaviour
{
    public string first_scene = "hallway";
    public Transform settings_menu;
    public Save_load_game loading_game;

    private void Awake() {
        
    }

    private void Start() {
        loading_game.load_progress();
    }

    public void start_new_game() {
        SceneManager.LoadScene(first_scene);
    }

    public void quit_game() {
        Debug.Log("quit game");
        Application.Quit();
    }

    public void switch_to_settings() {
        settings_menu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
