using UnityEngine;

namespace rvinowise.unity {
public static partial class Unity_extension {

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