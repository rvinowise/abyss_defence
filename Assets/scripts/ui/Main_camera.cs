using rvinowise.contracts;
using UnityEngine;
using Player_input = rvinowise.unity.ui.input.Player_input;

public class Main_camera : MonoBehaviour {
    
    public float min_zoom = 0.1f;
    public float max_zoom = 10f;

    private float zoom;
    private Camera main_camera;

    private rvinowise.unity.ui.input.Player_input input;

    
    void Awake() {
        main_camera = GetComponent<Camera>();
        Contract.Requires(main_camera != null, "Main_camera component should be attached only to Cameras");
        Contract.Requires(main_camera.orthographic, "the 2D game should use orthographic cameras only");
        zoom = main_camera.orthographicSize;
        
    }

    void Start() {
        input = Player_input.instance;
    }


    private void input_change_zoom() {
        //float wheel_movement = context.ReadValue<float>();
        float wheel_movement = input.scroll_value;
        
        if (input.zoom_held 
            &&
            wheel_movement != 0) 
        {
            zoom -= adjust_to_current_zoom(wheel_movement);
            zoom = preserve_possible_zoom(zoom);
            main_camera.orthographicSize = zoom;
        }
    }


    void Update() {
        input_change_zoom();
    }

    private static float zoom_speed = 0.0016f;
    private float adjust_to_current_zoom(float zoom_delta) {
        //zoom_delta = (zoom / max_zoom) * zoom_delta * zoom_speed;
        zoom_delta = Mathf.Pow(zoom, 0.8f) * zoom_delta * zoom_speed;
        return zoom_delta;
    }

    private float preserve_possible_zoom(float zoom) {
        return Mathf.Clamp(zoom, min_zoom, max_zoom);
    }
}
