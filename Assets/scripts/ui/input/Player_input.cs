using System;
using System.Collections.Generic;
using System.Linq;
using rvinowise.contracts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace rvinowise.unity {
public class Player_input: MonoBehaviour {
    
    public Cursor cursor;
    
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

    public static Player_human find_player() {
        return GameObject.FindWithTag("player root")?.GetComponent<Player_human>();
    }
    private void Start() {
        player = find_player();
    }
    
    public Vector2 read_mouse_world_position()
    {
        return Camera.main.ScreenToWorldPoint(
            UnityEngine.Input.mousePosition
        );

    }

    public bool is_button_presed(string name) {
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


    private readonly Stack<IInput_receiver> input_receivers = new Stack<IInput_receiver>();

    public void add_input_receiver(IInput_receiver receiver) {
        input_receivers.Push(receiver);
    }
    private void Update() {
        // bool input_processed = false;
        // do {
        //     IInput_receiver input_receiver = ;
        //     input_processed = input_receiver.process_input();
        // } while (!input_processed);
        input_receivers.Peek().process_input();

        while (
            input_receivers.Peek().is_finished
        ) {
            input_receivers.Pop();
        }
        
        cursor.transform.position = read_mouse_world_position();
        moving_vector = read_moving_vector();
        scroll_value = read_scroll();
        zoom_held = UnityEngine.Input.GetButton("Zoom");
    }

    public Color highlighting_color = new Color(0.7f,0.0f,0.0f);
    private ISet<Transform> highlighted_targets = new HashSet<Transform>();
    public void highlight_target(Transform target) {
        foreach (var sprite_renderer in target.GetComponentsInChildren<SpriteRenderer>()) {
            sprite_renderer.color += highlighting_color;
        }
        highlighted_targets.Add(target);
    }
    
    public void unhighlight_target(Transform target) {
        if (highlighted_targets.Contains(target)) {
            foreach (var sprite_renderer in target.GetComponentsInChildren<SpriteRenderer>()) {
                sprite_renderer.color += highlighting_color;
            }
            highlighted_targets.Remove(target);
        }
    }
    
}

}