using UnityEngine;


namespace rvinowise.unity.units.parts.teeth {
public class Tooth :
    MonoBehaviour
    ,IChild_of_group

{
    public SpriteRenderer sprite_renderer;

    protected void Awake() {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }
    
}

}