using UnityEngine;
using rvinowise.unity.extensions;


namespace rvinowise.unity.units.parts.limbs.arms {

public class Segment: limbs.Segment {

    [HideInInspector]
    public Quaternion desired_idle_rotation;
    


    public override void mirror_from(limbs.Segment src) {
        base.mirror_from(src);
        if (src is arms.Segment arms_src) {
            this.desired_idle_rotation = Quaternion.Inverse(arms_src.desired_idle_rotation);
        }
    }

    
    /* limbs.Segment interface */
  
    
    public static Segment create(string in_name) {
        GameObject game_object = new GameObject(in_name);
        game_object.AddComponent<SpriteRenderer>();
        var new_component = game_object.add_component<Segment>();
        return new_component;
    }
}
}