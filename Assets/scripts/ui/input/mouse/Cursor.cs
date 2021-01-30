using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;


namespace rvinowise.unity.ui.input.mouse {

public class Cursor: MonoBehaviour {
    private SpriteRenderer sprite_renderer;
    private Input input;

    void Awake() {
        input = Input.instance;
        init_components();
    }

    private void init_components() {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        transform.position = input.mouse_world_position;
    }

}
}