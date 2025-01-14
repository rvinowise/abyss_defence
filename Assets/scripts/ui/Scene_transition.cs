using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity {
public class Scene_transition : MonoBehaviour
{
    public string target_scene;
    public Scene_transition_effect effect;
    public TextMeshProUGUI text;
    public Animator text_animator;
    private AsyncOperation loading_scene;
    private bool touching_player;

    

    void Awake() {
        if (effect == null) {
            effect = GetComponentInChildren<Scene_transition_effect>(true);
        }
    }
 

    void OnTriggerEnter2D(Collider2D collider) {
        if (is_player(collider.gameObject)) {
            if (loading_scene == null) {
                StartCoroutine(start_loading_scene(target_scene));
            }
            touching_player = true;
            if (is_ready_to_switch_scenes()) {
                show_text();
            }
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        if (is_player(collider.gameObject)) {
            touching_player = false;
            hide_text();
        }
    }

    void Update() {
        if (
            player_wants_to_switch_scenes()&&
            is_ready_to_switch_scenes()
        ) {
            effect.start_transition(loading_scene);
            hide_text();
        }
    }

    private bool is_ready_to_switch_scenes() {
        return (
            scene_is_loaded() && 
            touching_player
        );
    }

    private bool player_wants_to_switch_scenes() {
        return Input.GetKeyDown(KeyCode.Return);
    }

    private bool is_player(GameObject in_go) {
        return in_go.CompareTag("player");
    }

    private IEnumerator start_loading_scene(string sceneName)
    {
        loading_scene = SceneManager.LoadSceneAsync(sceneName);

        loading_scene.allowSceneActivation = false;

        //while (!loading_scene.isDone)
        while(loading_scene.progress < 0.9f)
        {
            Debug.Log($"[scene]:{sceneName} [load progress]: {this.loading_scene.progress}");
            yield return null;
        }
        if (is_ready_to_switch_scenes()) {
            show_text();
        }
    }

    private void show_text() {
        text_animator.SetBool("visible", true);
    }
    private void hide_text() {
        text_animator.SetBool("visible", false);
    }

    private bool scene_is_loaded() {
        return (
            (loading_scene != null) && 
            (loading_scene.progress >= 0.9f)
        );
    }

    

}


}