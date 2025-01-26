using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class Settings_menu : MonoBehaviour {

    public Main_menu main_menu;
    public Button back_button;
    private void Awake() {
        back_button.onClick.AddListener(switch_to_main_menu);
    }

    private void Start() {
        
    }

    

    private void switch_to_main_menu() {
        main_menu.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    
}
