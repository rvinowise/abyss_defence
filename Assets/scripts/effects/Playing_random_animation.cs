
using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

public class Playing_random_animation:MonoBehaviour {

    private Animator animator;
    public SpriteRenderer sprite_renderer;
    public String animation_name;
    public int animations_amount;
    public bool random_y_flip = true;
    
    private void Awake() {
        animator = GetComponent<Animator>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        int random_number = Random.Range(1, animations_amount+1);
        animator.Play(animation_name + random_number);
        if (random_y_flip) {
            sprite_renderer.flipY = Random.Range(0, 2) == 1;
        }
    }

    
}

}