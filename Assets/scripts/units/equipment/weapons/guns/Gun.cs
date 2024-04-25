using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;

namespace rvinowise.unity {

/* a tool consisting of one element which can shoot */
public abstract class Gun: 
    Tool,
    IGun 
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
    public Projectile projectile_prefab;
    [SerializeField]
    public Gun_shell bullet_shell_prefab;
    
    

    [SerializeField]
    public bool aiming_automatically;

    public float stock_length = 0.45f;

    
    public Vector2 tip =>muzzle.localPosition;

    public float butt_to_second_grip_distance {
        get { return stock_length + second_holding.place_on_tool.magnitude; }
    }

    
    protected float last_shot_time = 0f;

    protected IReceive_recoil recoil_receiver;

    protected override void init_components() {
        base.init_components();
        animator = GetComponent<Animator>();
    }

    public override void hold_by(Hand in_hand) {
        base.hold_by(in_hand);
        recoil_receiver = in_hand.arm;
    }
    
    public override void drop_from_hand() {
        recoil_receiver = null;
    }
    

    public virtual void insert_ammunition(Ammunition in_ammunition) {
        in_ammunition.deactivate();
        ammo_qty += in_ammunition.rounds_qty;
        on_ammo_changed();
    }
    

    public float recoil_force = 150f;
    protected Projectile get_projectile() {
        Contract.Requires(can_fire(), "function Fire must be invoked after making sure it's possible");
        last_shot_time = Time.time;
        Projectile new_projectile = projectile_prefab.get_from_pool<Projectile>(
            muzzle.position, muzzle.rotation
        );
        ammo_qty -= ammo_value;
        Transform new_spark = spark_prefab.get_from_pool<Transform>();
        new_spark.copy_physics_from(muzzle);
        
        this.recoil_receiver?.push_with_recoil(recoil_force);
        on_ammo_changed();
        return new_projectile;
    }

    


    public virtual int get_loaded_ammo() {
        return ammo_qty;
    }

    public virtual float time_to_readiness() {
        return 0;
    }

    public void pull_trigger() {
        if (can_fire()) {
            fire();
        }
    }

    protected abstract void fire();

    protected virtual bool is_on_cooldown() {
        return Time.time - last_shot_time <= fire_rate_delay;
    }

    public virtual bool can_fire() {
        return 
            ammo_qty >0 &&
            !is_on_cooldown();
    }

    public delegate void EventHandler();
    public event EventHandler on_ammo_changed = delegate{};

    protected void notify_that_ammo_changed() {
        on_ammo_changed?.Invoke();
    }
}
}