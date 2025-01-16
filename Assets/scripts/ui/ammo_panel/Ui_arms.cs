using System;
using rvinowise.contracts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace rvinowise.unity {
public class Ui_arms: 
MonoBehaviour
{
    /* public Arm left_arm;
    public Arm right_arm; */
    public Arm_pair arm_pair;

    public static Ui_arms instance;

    public Image left_tool;
    public TextMeshProUGUI left_ammo;
    public Image right_tool;
    public TextMeshProUGUI right_ammo;

    void Awake() {
        Contract.Requires(instance == null);
        instance = this;
    }

    void Start() {
        //arm_pair.on_tools_changed += update_held_tools;
        arm_pair.on_ammo_changed += update_ammo;
        update_everything();
    }
    private void update_everything() {
        update_held_tools();
    }

    public void update_held_tools()
    {
        //left_tool.mainTexture == 
    }

    public void update_ammo(Arm arm, int new_ammo)
    {
        if (arm == arm_pair.left_arm) {
            left_ammo.text = String.Format("{0}",new_ammo);
        } else if (arm == arm_pair.right_arm) {
            right_ammo.text = String.Format("{0}",new_ammo);
        } else {
            Contract.Assert(false);
        }
        
    }

    public void update_available_tools(Tool in_tool, int change)
    {
        throw new System.NotImplementedException();
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = GameObject.FindWithTag("MainCamera")?.GetComponent<Camera>();
        canvas.planeDistance = -50;
        var player = GameObject.FindWithTag("player root")?.GetComponent<Player_human>();
        if (player != null) {
            //baggage = player.baggage;
            arm_pair = player.arm_pair;
        }
    }

  

    void Update()
    {
        
    }
}

}


