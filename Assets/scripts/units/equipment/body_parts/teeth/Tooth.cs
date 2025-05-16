using UnityEngine;


namespace rvinowise.unity {
public class Tooth :
    Child_of_group

{
    public SpriteRenderer sprite_renderer;

    protected void Awake() {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }
    
}

}