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

    public Transform muzzle;

    public UnityEngine.GameObject projectile;// { get; set; }
    public UnityEngine.GameObject spark;
    [SerializeField]
    public GameObject bullet_shell;
    
    /* components */
    public Animator animator;
    
    /* inner characteristics */
    public Vector2 tip {
        get { return muzzle.localPosition; }
        set { muzzle.localPosition = value; }
    }

    public bool has_stock {
        get {
            return stock_length > 0f;
        }
    }
    public virtual float stock_length { get; }
    public float butt_to_second_grip_distance {
        get { return stock_length + second_holding.place_on_tool.magnitude; }
    }

    public IHave_velocity recoil_receiver {
        get { return main_holding.holder; }
    }
    
    /* current values */
    private float last_shot_time = 0f;


    protected override void Awake() {
        base.Awake();
    }

    protected override void init_components() {
        base.init_components();
        animator = GetComponent<Animator>();
    }

    protected virtual GameObject fire() {
        last_shot_time = Time.time;
        GameObject new_projectile = GameObject.Instantiate(
            projectile, 
            muzzle.position,
            transform.rotation
        );
        GameObject new_spark = Instantiate(
            spark,
            muzzle.position,
            transform.rotation
        );    
        return new_projectile;
    }

    

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