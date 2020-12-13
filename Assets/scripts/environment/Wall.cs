using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using rvinowise.unity.units.parts.weapons.guns.common;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.extensions;

namespace rvinowise.unity.obstacles {
public class Wall : MonoBehaviour
{
    public Persistent_residue_sprite_holder residue_holder;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /* void OnCollisionEnter2DChild (Collision2D collision) {
        if (collision.get_damaging_projectile() is Projectile projectile) {
            residue_holder.add_piece(
                projectile.last_physics.position,
                projectile.last_physics.velocity.to_quaternion()
            );
        }
    } */

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.get_damaging_projectile() is Projectile projectile) {
            var contact = collision.GetContact(0);
            residue_holder.add_piece(
                contact.point,
                contact.relativeVelocity.to_quaternion()
            );
        }
    }
}

}