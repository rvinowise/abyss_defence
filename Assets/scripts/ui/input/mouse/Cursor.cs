using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;


namespace rvinowise.ui.input.mouse {

public class Cursor {


    public GameObject center;

    private SpriteRenderer sprite_renderer;
    
    

    public Cursor() {
        init_cursor();
    }

    private void init_cursor() {
        center = new GameObject("mouse center");
        center.AddComponent<SpriteRenderer>();
        sprite_renderer = center.GetComponent<SpriteRenderer>();
        sprite_renderer.sprite = Resources.Load<Sprite>("ui/mouse/cursor");
    }

    public void update() {
        center.transform.position = Input.instance.mouse_world_position;
        //var test = Input.instance.mouse_world_position;
        //center.transform.position = test;
        //center.transform.position = new Vector2(1f,1f);
    }

}
}