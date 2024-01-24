using UnityEngine;


namespace rvinowise.unity.units.parts.weapons.thorny_mine {

public class Thorn_interval: MonoBehaviour {

    private SpriteRenderer sprite_renderer;
    
    void Awake() {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {

    }

    public void go_off(Quaternion in_direction) {

    }

}

}