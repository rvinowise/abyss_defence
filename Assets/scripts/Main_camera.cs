using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.rvi.contracts;
using UnityEngine;

public class Main_camera : MonoBehaviour {
    
    public float min_zoom = 0.1f;
    public float max_zoom = 10f;

    private float zoom;
    private Camera camera;
    void Awake() {
        camera = GetComponent<Camera>();
        Contract.Requires(camera != null, "Main_camera component should be attached only to Cameras");
        Contract.Requires(camera.orthographic, "the 2D game should use orthographic cameras only");
        zoom = camera.orthographicSize;
    }

    void Update() {
        float wheel_movement = Input.GetAxis("Mouse ScrollWheel");
        if (wheel_movement != 0) {
            zoom -= adjust_to_current_zoom(wheel_movement);
            zoom = preserve_possible_zoom(zoom);
            camera.orthographicSize = zoom;
            //Debug.Log("orthographicSize:" + camera.orthographicSize);
        }
        
    }

    private static float zoom_speed = 2f;
    private float adjust_to_current_zoom(float zoom_delta) {
        //zoom_delta = (zoom / max_zoom) * zoom_delta * zoom_speed;
        zoom_delta = Mathf.Pow(zoom, 0.8f) * zoom_delta * zoom_speed;
        return zoom_delta;
    }

    private float preserve_possible_zoom(float zoom) {
        return Mathf.Clamp(zoom, min_zoom, max_zoom);
    }
}
