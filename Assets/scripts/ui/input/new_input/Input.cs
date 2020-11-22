/*using System;
using rvinowise.rvi.contracts;
using rvinowise.unity.ui.input;
using UnityEngine;
using rvinowise.unity.extensions;

using UnityEngine.InputSystem;


namespace rvinowise.unity.ui.new_input {
public class Input: MonoBehaviour {
    
    static public Input instance;
    
    public Unity_input unity_input;

    public Vector2 mouse_world_position { get; private set; }
    public Vector2 moving_vector { get; private set; }
    public float scroll_value { get; private set; }
    public int mouse_wheel_steps {
        get {
            return (int) System.Math.Round(scroll_value / 120);
        }
    }


    public input.mouse.Cursor cursor;

    public void Awake() {
        init_singleton();
        init_unity_input();
        cursor = new input.mouse.Cursor();
    }

    private void init_singleton() {
        Contract.Requires(instance == null, "singleton should be initialised once");
        instance = this;
    }
    
    private void init_unity_input() {
        unity_input = new Unity_input();
        unity_input.Player.Mouse_position.register(on_mouse_moved);
        unity_input.Player.Move.register(on_move);
        unity_input.Player.Scroll.register(on_scroll);
    }

    private void OnDisable() {
        unity_input.Player.Mouse_position.unregister(on_mouse_moved);
        unity_input.Player.Move.unregister(on_move);
        unity_input.Player.Scroll.unregister(on_scroll);
    }

    private void on_move(InputAction.CallbackContext obj) {
        moving_vector = obj.ReadValue<Vector2>();
    }

    private void on_scroll(InputAction.CallbackContext obj) {
        scroll_value = obj.ReadValue<float>();
    }


    private void on_mouse_moved(InputAction.CallbackContext obj) {
        mouse_world_position = Camera.main.ScreenToWorldPoint(
            obj.ReadValue<Vector2>()
        );
    }

    
    

    private void Update() {
        cursor.update();
    }

}

}*/