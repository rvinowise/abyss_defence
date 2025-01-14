using System;
using System.Collections;
using Animancer;
using rvinowise.unity.extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity {
public interface IInput_receiver {
    //bool process_input(KeyCode key);
    bool is_finished { get; }
    bool process_input();
}


}