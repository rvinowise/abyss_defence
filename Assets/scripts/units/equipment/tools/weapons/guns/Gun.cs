using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rvinowise;
using rvinowise.rvi.contracts;
using rvinowise.units.parts.tools;
using UnityEngine.Serialization;


namespace rvinowise.units.parts.weapons.guns {

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
    public UnityEngine.GameObject spark;
    
    [SerializeField]
    public GameObject bullet_shell_prefab;
    [SerializeField] public GameObject magazine_prefab;
        
    /* components */
    public Animator animator { get; set; }

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

    private guns.Magazine magazine;



    protected override void Awake() {
        base.Awake();
    }

    protected override void init_components() {
        base.init_components();
        animator = GetComponent<Animator>();
    }

    public void insert_magazine(Magazine in_magazine) {
        magazine = in_magazine;
    }
    
    protected virtual Bullet fire() {
        Contract.Requires(can_fire(), "function Fire must be invoked after making sure it's possible");
        last_shot_time = Time.time;
        Bullet new_projectile = magazine.retrieve_round();
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
        if (can_fire()) {
            fire();
        }
        else {
            
        }
    }

    private bool can_fire() {
        return ready_to_fire() && !magazine.empty();
    }

    public virtual bool ready_to_fire() {
        return (Time.time - last_shot_time) > fire_rate_delay;
    }
}
}