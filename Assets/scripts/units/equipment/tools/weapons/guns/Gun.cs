using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;

using rvinowise;
using rvinowise.contracts;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.units.parts.weapons.guns.common;

using UnityEngine.Serialization;


namespace rvinowise.unity.units.parts.weapons.guns {

/* a tool consisting of one element which can shoot */
public abstract class Gun: 
    Tool,
    IWeapon 
{
    /* constant characteristics */
    [SerializeField]
    public float fire_rate_delay;

    //public float reload_time;
    [SerializeField]
    public Transform muzzle;
    [SerializeField]
    public Transform spark_prefab;
    
    [SerializeField]
    public Bullet_shell bullet_shell_prefab;
    [SerializeField] public Magazine magazine_prefab;
    public guns.Magazine magazine;

    public virtual float stock_length { get; }

        
    public Animator animator { get; set; }

    public Vector2 tip {
        get { return muzzle.localPosition; }
        set { muzzle.localPosition = value; }
    }

    public bool has_stock {
        get {
            return stock_length > 0f;
        }
    }
    
    public float butt_to_second_grip_distance {
        get { return stock_length + second_holding.place_on_tool.magnitude; }
    }

    public IHave_velocity recoil_receiver {
        get { return main_holding.holder; }
    }
    
    private float last_shot_time = 0f;



    protected override void Awake() {
        base.Awake();
    }

    protected override void init_components() {
        base.init_components();
        animator = GetComponent<Animator>();
    }

    

    public virtual void apply_ammunition(Ammunition in_ammunition) {
        in_ammunition.deactivate();
    }
    
    protected virtual Projectile fire() {
        Contract.Requires(can_fire(), "function Fire must be invoked after making sure it's possible");
        last_shot_time = Time.time;
        Projectile new_projectile = magazine.retrieve_round(muzzle);
        Transform new_spark = spark_prefab.get_from_pool<Transform>();
        new_spark.copy_physics_from(muzzle);
            
        return new_projectile;
    }

    

    public virtual float time_to_readiness() {
        return 0;
    }

    public void pull_trigger() {
        if (can_fire()) {
            fire();
        }
        else {
            
        }
    }

    public virtual bool ready_to_fire() {
        return (Time.time - last_shot_time) > fire_rate_delay;
    }

    protected virtual bool can_fire() {
        return ready_to_fire();
    }
}
}