using UnityEngine;


namespace rvinowise.unity {
public static partial class Unity_extension {

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
    
    public static Projectile get_damaging_projectile(
        this Collider2D collider
    ) {
        Projectile collided_projectile = collider.gameObject.GetComponent<Projectile>();
        if (collided_projectile != null) {
            if (collided_projectile.rigid_body.velocity.magnitude >= damaging_velocity) {
                return collided_projectile;
            }
        }
        return null;
    }
    
}

}