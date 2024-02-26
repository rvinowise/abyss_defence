using UnityEngine;


namespace rvinowise.unity {

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