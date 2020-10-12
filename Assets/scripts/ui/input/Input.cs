using System;
using rvinowise.rvi.contracts;
using UnityEngine;


namespace rvinowise.ui.input {
public class Input: MonoBehaviour {
    
    static public Input instance;

    public UnityEngine.Input unity;
    
    public Vector2 mouse_world_position { get; private set; }
    public Vector2 moving_vector { get; private set; }
    public float scroll_value { get; private set; }
    public int mouse_wheel_steps {
        get {
            return (int) System.Math.Round(scroll_value / 120);
        }
    }
    public bool zoom_held { get; set; }

    public mouse.Cursor cursor;

    
    public void Awake() {
        init_singleton();
        cursor = new mouse.Cursor();
    }

    private void init_singleton() {
        Contract.Requires(instance == null, "singleton should be initialised once");
        instance = this;
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
        //Debug.Log("read_moving_vector");
        float horizontal = UnityEngine.Input.GetAxis("Horizontal");
        float vertical = UnityEngine.Input.GetAxis("Vertical");

        /*Debug.Log(
            String.Format("horizontal: {0}",
                horizontal
            )
        );*/
        
        Vector2 direction_vector = new Vector2(horizontal, vertical);
        
        /*Debug.Log(
            String.Format("Input: {0} ; {1}",
                Input.instance.moving_vector,
                Input.instance.mouse_world_position
            )
        );*/
        
        return direction_vector.normalized;
    }

    private float read_scroll() {
        float wheel_movement = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        return wheel_movement * 1200;
    }
    

    private void Update() {
        mouse_world_position = read_mouse_world_position();
        moving_vector = read_moving_vector();
        scroll_value = read_scroll();
        zoom_held = UnityEngine.Input.GetButton("Zoom");
        cursor.update();
    }


    
}

}