using rvinowise.contracts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace rvinowise.unity {
public class Player_input: MonoBehaviour {
    
    public Cursor cursor;
    public UnityEngine.Input unity;
    
    public Vector2 mouse_world_position => cursor.transform.position;
    public Vector2 moving_vector { get; private set; }
    public float scroll_value { get; private set; }
    public int mouse_wheel_steps {
        get {
            return (int) System.Math.Round(scroll_value / 120);
        }
    }
    public bool zoom_held { get; set; }

    public static Player_input instance;

    public Player_human player;

    void Awake() {
        Contract.Requires(instance == null, "singleton");
        instance = this;
        
    }
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        player = GameObject.FindWithTag("player")?.GetComponent<Player_human>();
    }
    public Vector2 read_mouse_world_position()
    {
        return Camera.main.ScreenToWorldPoint(
            UnityEngine.Input.mousePosition
        );

    }

    public bool button_presed(string name) {
        return UnityEngine.Input.GetButtonDown(name);
    }
    
    public bool GetMouseButtonDown(int index) {
        return UnityEngine.Input.GetMouseButtonDown(index);
    }

    private Vector2 read_moving_vector() {
        float horizontal = UnityEngine.Input.GetAxis("Horizontal");
        float vertical = UnityEngine.Input.GetAxis("Vertical");

        Vector2 direction_vector = new Vector2(horizontal, vertical);
 
        return direction_vector.normalized;
    }

    private float read_scroll() {
        float wheel_movement = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        return wheel_movement * 1200;
    }
    

    private void Update() {
        cursor.transform.position = read_mouse_world_position();
        moving_vector = read_moving_vector();
        scroll_value = read_scroll();
        zoom_held = UnityEngine.Input.GetButton("Zoom");
    }


    
}

}