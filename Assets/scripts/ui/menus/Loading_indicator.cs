using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity;
using rvinowise.unity.extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


public interface ILoading_indicator {
    void set_loaded_amount(float loaded);
}
public class Loading_indicator : 
    MonoBehaviour
    ,ILoading_indicator
{

    public Image moving_bar;
    public Transform progress_indicator;
    public Button launch_scene_button;

    private void Awake() {
        launch_scene_button.deactivate();
        progress_indicator.activate();
    }

    public void set_loaded_amount(float loaded) {
        moving_bar.fillAmount = loaded;
    }

    public void show_button_to_start_game(UnityAction on_click) {
        launch_scene_button.onClick.AddListener(on_click);
        launch_scene_button.activate();
        progress_indicator.deactivate();

    }
    

    
}
