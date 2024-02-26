using UnityEngine;


namespace rvinowise.unity {

public class Stained_surface : MonoBehaviour {
    public SpriteRenderer sprite_renderer;
    public BoxCollider2D collider2d;

    void Awake() {
        sprite_renderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<BoxCollider2D>();
    }

    void Update() { }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<Puddle>() is Puddle puddle) {
            //draw_image_forever(puddle.);
        }
    }

    private void draw_image_forever() { }
}

}