using System.Collections;
using System.Collections.Generic;

using rvinowise.unity.geometry2d;
using rvinowise.unity.units;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.maps;
using rvinowise.unity.debug;
using rvinowise.unity.effects.physics;
using rvinowise.unity.effects.persistent_residue;
using rvinowise.unity.effects.trails.mesh_impl;

namespace rvinowise.unity.units.parts.weapons.guns.common {

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : MonoBehaviour {

    [HideInInspector]
    public Rigidbody2D rigid_body;

    protected new Collider2D collider;
    
    /* OnCollision callback fires after the velocity changes. The relevant velocity should be stored here  */
    public Saved_physics last_physics = new Saved_physics();
    public void store_last_physics() {
        last_physics.velocity = rigid_body.velocity;
        last_physics.position = transform.position;
    }
    public abstract Polygon get_damaged_area(Ray2D in_ray);

    public abstract void stop_at_position(Vector2 in_point);
    
}


}