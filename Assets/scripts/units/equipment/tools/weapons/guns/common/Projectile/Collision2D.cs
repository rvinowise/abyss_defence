using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;



//namespace rvinowise.unity.extensions {
namespace rvinowise.unity.units.parts.weapons.guns.common {
public static partial class Unity_extension {

    private const float damaging_velocity = 5f;
    public static Projectile get_damaging_projectile(
        this Collision2D collision
    ) {
        Projectile collided_projectile = collision.gameObject.GetComponent<Projectile>();
        if (collided_projectile != null) {
            if (collided_projectile.rigid_body.velocity.magnitude >= damaging_velocity) {
                return collided_projectile;
            }
        }
        return null;
    }
    
}

}