using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts.teeth {
public class Tooth :
MonoBehaviour
,IChild_of_group
,IMirrored
{
    [HideInInspector]
    public SpriteRenderer sprite_renderer;

    protected void Awake() {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }
    public IMirrored create_mirrored()
    {
        Tooth dst = GameObject.Instantiate(this).GetComponent<Tooth>();
        // the base direction_quaternion is to the right
        dst.transform.localPosition = 
            transform.localPosition.mirror();
        dst.transform.localRotation = transform.localRotation.inverse();
      
        dst.sprite_renderer.flipY = !sprite_renderer.flipY;

        return dst;
    }
}

}