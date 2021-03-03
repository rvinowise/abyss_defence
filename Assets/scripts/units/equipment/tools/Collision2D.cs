using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.units.humanoid;

namespace rvinowise.unity.units.parts.tools {
public static partial class Unity_extension {

    //private const float damaging_velocity = 5f;
    private const float damaging_velocity = 0f;
    public static Humanoid get_user_of_tools(
        this Collision2D collision
    ) {
        Humanoid user = collision.gameObject.GetComponent<Humanoid>();
        if (user != null) {
            return user;
        }
        return null;
    }
    
}

}