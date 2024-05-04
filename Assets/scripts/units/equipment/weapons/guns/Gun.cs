using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

/* a tool consisting of one element which can shoot */
[RequireComponent(typeof(Tool))]
public class Gun: MonoBehaviour
    ,IGun 
{
    public bool aiming_automatically;

    public Transform muzzle;
    public Transform spark_prefab;
    public float stock_length = 0.45f;
    public Transform shell_ejector;
    public Slot magazine_slot;
    public int animation_fire = Animator.StringToHash("slide");
    
    public float fire_rate_delay;
    public Projectile projectile_prefab;
    public Gun_shell bullet_shell_prefab;
    public Ammunition ammo_prefab;
    public Ammo_compatibility ammo_compatibility;
    public int ammo_qty;
    public int max_ammo_qty;
    public float projectile_force = 1000f;
    public bool is_full_auto_fire;

    public Animator animator;
    public Tool tool;
    
    public Vector2 tip =>muzzle.localPosition;

    public float butt_to_second_grip_distance {
        get { return stock_length + tool.second_holding.place_on_tool.magnitude; }
    }

    
    private float last_shot_time = 0f;

    private IReceive_recoil recoil_receiver;

    private bool is_trigger_pulled;

    private void Awake() {
        animator = GetComponent<Animator>();
        magazine_slot = GetComponentInChildren<Slot>();
        tool = GetComponent<Tool>();
    }

    public void hold_by(Hand in_hand) {
        recoil_receiver = in_hand.arm;
    }
    
    public void drop_from_hand() {
        recoil_receiver = null;
    }
    

    public virtual void insert_ammunition(Ammunition in_ammunition) {
        in_ammunition.deactivate();
        ammo_qty += in_ammunition.rounds_qty;
        on_ammo_changed();
    }
    

    public float recoil_force = 150f;
    private Projectile get_projectile() {
        Contract.Requires(can_fire(), "function Fire must be invoked after making sure it's possible");
        last_shot_time = Time.time;
        Projectile new_projectile = projectile_prefab.instantiate<Projectile>(
            muzzle.position, muzzle.rotation
        );
        ammo_qty -= 1;
        Transform new_spark = spark_prefab.get_from_pool<Transform>();
        new_spark.copy_physics_from(muzzle);
        
        this.recoil_receiver?.push_with_recoil(recoil_force);
        on_ammo_changed();
        return new_projectile;
    }


    public int get_loaded_ammo() {
        return ammo_qty;
    }

    public float time_to_readiness() {
        return 0;
    }


    private void Update() {
        if (
            (is_trigger_pulled)&&
            (is_full_auto_fire)&&
            (can_fire())
        )
        {
            fire();
        }
    }

    public void pull_trigger() {
        if (
            (!is_trigger_pulled)
            &&
            (can_fire())
        )
        {
            fire();
        }
        
        is_trigger_pulled = true;
    }

    public void release_trigger() {
        is_trigger_pulled = false;
    }

    private void fire() {
        var new_projectile = get_projectile();
        if (new_projectile == null) {
            return;
        }
        new_projectile.damage_dealer?.set_attacker(tool.main_holding.holding_hand.arm.pair.user.transform);
        
        animator.SetTrigger(animation_fire);
        propell_projectile(new_projectile);
    }
    
    private void propell_projectile(Projectile projectile) {
        projectile.rigid_body.AddForce(transform.rotation.to_vector() * (projectile_force * Time.deltaTime), ForceMode2D.Impulse);
        projectile.store_last_physics();
    }

    private bool is_on_cooldown() {
        return Time.time - last_shot_time <= fire_rate_delay;
    }

    public bool can_fire() {
        return 
            ammo_qty >0 &&
            !is_on_cooldown();
    }
    

    public delegate void EventHandler();
    public event EventHandler on_ammo_changed = delegate{};

    protected void notify_that_ammo_changed() {
        on_ammo_changed?.Invoke();
    }
    
    /* invoked from an animation */
    [called_in_animation]
    private void eject_bullet_shell() {
        
        Gun_shell new_shell = bullet_shell_prefab.get_from_pool<Gun_shell>(
            shell_ejector.position,
            transform.rotation
        );
        new_shell.enabled = true;
            
        const float ejection_force = 5f;
        Vector2 ejection_vector = Directions.degrees_to_quaternion(-15+Random.value*30) *
                                  shell_ejector.right *
                                  ejection_force;
        
        Vector2 gun_vector = (Vector2)transform.position - tool.last_physics.position;
        
        var shell_rigidbody = new_shell.GetComponent<Rigidbody2D>();
        shell_rigidbody.AddForce(ejection_vector + gun_vector*50);
        shell_rigidbody.AddTorque(-360f + Random.value*300f);
        
        Trajectory_flyer flyer = new_shell.GetComponent<Trajectory_flyer>();
        flyer.height = 1;
        flyer.vertical_velocity = 1f + Random.value * 3f;
    }
    
    
    private readonly RaycastHit2D[] targeted_targets = new RaycastHit2D[1000];
    public bool is_aimed_at_target(Transform in_target) {
        var targets_number = 
            Physics2D.RaycastNonAlloc(
                muzzle.position, muzzle.right, targeted_targets
            );
        for (var i_target=0;i_target<targets_number;++i_target) {
            var hit = targeted_targets[i_target];
            if (hit.transform == in_target) {
                return true;
            }
        }

        return false;
    }
    
    public bool is_directed_at_target(Transform in_target) {
        var vector_to_target =
            (in_target.position - transform.position);
        
        var direction_to_tarrget =
            vector_to_target.to_dergees();

        var distance_to_target = vector_to_target.magnitude;

        var precision_angle = 10f;
        
        return 
            Math.Abs(
                transform.rotation.to_degree().angle_to(direction_to_tarrget)
            ) 
            < 
            precision_angle/distance_to_target;
    }
    
}
}