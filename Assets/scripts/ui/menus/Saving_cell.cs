using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class Saving_cell : MonoBehaviour
{
    public Image preview;
    public TextMeshProUGUI title;
    public Button button;

    [HideInInspector] // it doesn't exist as a prefab, and should be linked from the scene
    public Savings_menu savings_menu;
    
    private void Awake() {
        button.onClick.AddListener(on_click);
    }

    public void load_saving_from_file(Saved_game saving) {
        //preview = saving.scene;
        title.text = saving.scene;
    }

    private void on_click() {
        savings_menu.load_saving(title.text);
    }
    
}
