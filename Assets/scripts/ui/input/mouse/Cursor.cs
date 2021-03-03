using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;
using rvinowise.unity.units.control;
using rvinowise.unity.units.parts.weapons.guns.common;

namespace rvinowise.unity.ui.input.mouse {

public class Cursor: MonoBehaviour {
    private SpriteRenderer sprite_renderer;

    void Awake() {
        init_components();
    }

    private void init_components() {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
    }

    

}
}