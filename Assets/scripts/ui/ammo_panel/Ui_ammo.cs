using rvinowise.contracts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace rvinowise.unity {
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

