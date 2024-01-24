using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity.management {
public class Transition_to_first_scene : MonoBehaviour
{

    [SerializeField] public string first_scene = "entrance";

    private AsyncOperation loading_scene;
    void Awake()
    {
        switch_from_preaload_to_first();
    }

    private void switch_from_preaload_to_first() {
        //StartCoroutine(start_loading_scene(first_scene));
        SceneManager.LoadScene(first_scene);
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