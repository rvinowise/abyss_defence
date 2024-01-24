using rvinowise.contracts;
using rvinowise.unity.units.control.human;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.parts.tools;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity.ui {
public class Ui_ammo: 
MonoBehaviour
{
    public Baggage baggage;

    public static Ui_ammo instance;

    public TextMeshProUGUI ammo_label;

    void Awake() {
        Contract.Requires(instance == null);
        instance = this;
    }

    void Start() {
        baggage.on_ammo_changed += update_ammo;
        update_everything();
    }
    private void update_everything() {
        update_ammo();
    }

    public void update_ammo()
    {
        //ammo_label.text = String.Format("{0}",baggage.rounds);
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
        var player = GameObject.FindWithTag("player")?.GetComponent<Player_human>();
        if (player != null) {
            baggage = player.baggage;
        }
    }

  

    void Update()
    {
        
    }
}

}

