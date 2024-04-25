
using System;
using UnityEngine;

namespace rvinowise.unity {

public class Fading_piece:MonoBehaviour {

    public float final_alpha = 0.5f;
    public float alpha_change = 0.1f;

    public UnityEngine.Events.UnityEvent on_faded;
    
    private SpriteRenderer sprite_renderer;
    private bool is_fading;

    private void Awake() {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    public void start_fading() {
        is_fading = true;
    }


    private void Update() {
        if (is_fading) {
            var old_color = sprite_renderer.color;
            var new_color = new Color(
                old_color.r,    
                old_color.g,    
                old_color.b,    
                old_color.a - alpha_change*Time.deltaTime
            );
            sprite_renderer.color = new_color;

            if (new_color.a <= final_alpha) {
                is_fading = false;
                on_faded?.Invoke();
            }
        }
    }
}

}