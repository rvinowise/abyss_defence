using System;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

/* a tool consisting of one element which can shoot */
[RequireComponent(typeof(Tool))]
public class Gun: MonoBehaviour
    ,IGun ,IReloadable
{
    public bool aiming_automatically;

    public Transform muzzle;
    
    public Transform shell_ejector;
    public int shooting_animation = Animator.StringToHash("shoot");
    
    public float fire_rate_delay;
    public Gun_shell bullet_shell_prefab;
    public Ammunition ammo_prefab;
    public Ammo_compatibility ammo_compatibility;
    public int ammo_qty;
    public int max_ammo_qty;
    public bool is_full_auto_fire;

    public Animator animator;
    public Tool tool;
    
    

    
    private float last_shot_time = 0f;

    private IReceive_recoil recoil_receiver;

    private bool is_trigger_pulled;

    private void Awake() {
        animator = GetComponent<Animator>();
        tool = GetComponent<Tool>();
    }

    public void hold_by(Hand in_hand) {
        recoil_receiver = in_hand.arm;
    }
    
    public void drop_from_hand() {
        recoil_receiver = null;
    }
    

    public float recoil_force = 150f;
    

    public int get_loaded_ammo() {
        return ammo_qty;
    }
    public int get_lacking_ammo() {
        return max_ammo_qty - ammo_qty;
    }
    
    public virtual void insert_ammunition(Ammo_compatibility ammo, int rounds_amount) {
        ammo_qty += rounds_amount;
        on_ammo_changed();
    }

    public Ammo_compatibility get_ammo_compatibility() => ammo_compatibility;
    
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

    public Direct_projectile_launcher projectile_launcher;
    public void fire() {
        projectile_launcher.fire();
        last_shot_time = Time.time;
        ammo_qty -= 1;
        recoil_receiver?.push_with_recoil(recoil_force);
        on_ammo_changed();
        //animator.SetTrigger(shooting_animation);
        if (animator) {
            animator.Play(shooting_animation);
        }
    }

    public void release_trigger() {
        is_trigger_pulled = false;
    }

    

    public bool is_on_cooldown() {
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
        
        Gun_shell new_shell = bullet_shell_prefab.instantiate<Gun_shell>(
            shell_ejector.position,
            transform.rotation
        );
        //new_shell.enabled = true;
            
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
    public bool is_aimed_at_collider(Transform in_target) {
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

    public Transform get_aiming_target() {
        var hit = 
            Physics2D.Raycast(
                muzzle.position, 
                muzzle.right 
            );
        return hit.transform;
    }
    
    
    
    
}
}