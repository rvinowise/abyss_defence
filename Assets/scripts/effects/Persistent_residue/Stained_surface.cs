using System;
using System.Collections;
using System.Collections.Generic;
using rvinowise.effects.liquids;
using UnityEngine;

public class Stained_surface : MonoBehaviour {
    public SpriteRenderer sprite_renderer;
    public BoxCollider2D collider;
    
    void Awake() {
        sprite_renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<Puddle>() is Puddle puddle) {
            //draw_image_forever(puddle.);
        }
    }

    private void draw_image_forever() {
        
    }
}
