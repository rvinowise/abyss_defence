using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.units.parts.tools;


namespace rvinowise.units.parts.weapons.guns {

/* a tool consisting of one element which can shoot */
public abstract class Gun: 
    Tool,
    IWeapon 
{
    /* constant characteristics */
    public float fire_rate_delay;
    public float reload_time;

    public Vector2 muzzle_place;
    public Vector2 tip {
        get { return muzzle_place; }
        set { muzzle_place = value; }
    }
    
    /* current values */
    private float last_shot_time = 0f;

    public virtual void fire() {
        last_shot_time = Time.time;
        GameObject.Instantiate(
            get_projectile(), 
            transform.TransformPoint(muzzle_place), 
            transform.rotation
        );
    }

    public abstract UnityEngine.Object get_projectile();

    public virtual float time_to_readiness() {
        return 0;
    }

    public void pull_trigger() {
        if (ready_to_fire()) {
            fire();
        }
        else {
            
        }
    }

    public virtual bool ready_to_fire() {
        return (Time.time - last_shot_time) > fire_rate_delay;
    }
}
}