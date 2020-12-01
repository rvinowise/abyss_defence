using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.contracts;
using rvinowise.unity.ui.input;
using UnityEngine;
using rvinowise.unity.extensions;

using Input = UnityEngine.Input;

public class Main_camera : MonoBehaviour {
    
    public float min_zoom = 0.1f;
    public float max_zoom = 10f;

    private float zoom;
    private Camera main_camera;

    
    void Awake() {
        main_camera = GetComponent<Camera>();
        Contract.Requires(main_camera != null, "Main_camera component should be attached only to Cameras");
        Contract.Requires(main_camera.orthographic, "the 2D game should use orthographic cameras only");
        zoom = main_camera.orthographicSize;

        init_input();

    }

    private void init_input() {
        /*input = new Unity_input();
        input.Player.Scroll.performed += input_change_zoom;
        input.Player.Scroll.Enable();*/
    }

    private void OnDisable() {
        /*input.Player.Scroll.performed -= input_change_zoom;
        input.Player.Scroll.Disable();*/
    }

    private void input_change_zoom() {
        //float wheel_movement = context.ReadValue<float>();
        float wheel_movement = rvinowise.unity.ui.input.Input.instance.scroll_value;
        
        if (rvinowise.unity.ui.input.Input.instance.zoom_held 
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
