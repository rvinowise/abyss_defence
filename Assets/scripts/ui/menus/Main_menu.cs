using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_menu : MonoBehaviour
{
    [SerializeField] public string first_scene = "hallway";
    public Transform options_menu;

    public void start_new_game() {
        SceneManager.LoadScene(first_scene);
    }

    public void quit_game() {
        Debug.Log("quit game");
        Application.Quit();
    }

    public void switch_to_options() {
        options_menu.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
