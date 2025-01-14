using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


namespace rvinowise.unity {

public class Disappearing_body:
    MonoBehaviour
    ,IDestructible
{


    public bool will_be_settled_when_stops = false;
    public bool is_decaying = false;
    private float decaying_speed = 0.2f;
    
    private Rigidbody2D rigid_body;
    public SpriteRenderer sprite_renderer;

    private List<SpriteRenderer> decaying_sprite_renders = new List<SpriteRenderer>();
    
    private void Awake() {
        rigid_body = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }


    public void settle_when_stops() {
        if (!is_decaying) {
            will_be_settled_when_stops = true;
        }
    }
    
    private void Update() {
        if (
            (will_be_settled_when_stops)&&
            (!is_moving())
        ) {
            settle_on_ground();
        }
        else if (
            is_decaying
        ) {
            decaying_step();
        }
    }

    private bool is_moving() {
        return 
            (rigid_body.velocity.magnitude > 0.01f)
            ||
            (rigid_body.angularVelocity > 0.01f);
    }

    private void settle_on_ground() {
        Debug.Log($"DYING: ({name})Disappearing_body.settle_on_ground");
        Destroy(GetComponent<Divisible_body>());
        Destroy(rigid_body);
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Turning_element>());
        Destroy(GetComponent<Bleeding_body_droplet_objects>());
        start_decaying();
        will_be_settled_when_stops = false;
        
    }

    private void start_decaying() {
        is_decaying = true;
        decaying_sprite_renders = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    private void decaying_step() {
        foreach (var sprite_trenderer in decaying_sprite_renders) {
            decaying_sprite_step(sprite_trenderer);
        }
        
        if (has_decayed()) {
            Debug.Log($"DYING: ${name} has decayed and will be destroyed");
            Destroy(gameObject);
        }
    }

    private void decaying_sprite_step(SpriteRenderer in_sprite_renderer) {
        var old_color = in_sprite_renderer.color;
        in_sprite_renderer.color =
            new Color(
                old_color.r,
                old_color.g,
                old_color.b,
                old_color.a - decaying_speed * Time.deltaTime
            );
    }

    private bool has_decayed() {
        return sprite_renderer.color.a <=0f;
    }
    
    public void on_start_dying() {
        settle_when_stops();
    }
}


}