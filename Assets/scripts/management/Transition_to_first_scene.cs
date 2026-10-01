using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity {
public class Transition_to_first_scene : MonoBehaviour
{

    [SerializeField] public string first_scene = "entrance";

    private AsyncOperation loading_scene;
    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "_preload") {
            return;
        }
        switch_from_preaload_to_first();
    }

    private void switch_from_preaload_to_first() {
        if (Application.CanStreamedLevelBeLoaded(first_scene)) {
            SceneManager.LoadScene(first_scene);
            return;
        }
        if (Application.CanStreamedLevelBeLoaded("arena")) {
            SceneManager.LoadScene("arena");
            return;
        }
        if (SceneManager.sceneCountInBuildSettings > 0) {
            SceneManager.LoadScene(0);
        }
    }


    private IEnumerator start_loading_scene(string sceneName)
    {
        loading_scene = SceneManager.LoadSceneAsync(sceneName);

        loading_scene.allowSceneActivation = false;

        while (!loading_scene.isDone)
        {
            Debug.Log($"[scene]:{sceneName} [load progress]: {this.loading_scene.progress}");
            yield return null;
        }
    }
    


}

}