using System;
using System.Collections.Generic;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.contracts;
using rvinowise.unity.extensions.attributes;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using Line = rvinowise.unity.geometry2d.for_unmanaged.Line;
using Random = UnityEngine.Random;


namespace rvinowise.unity {

/* a tool consisting of one element which can shoot */
[RequireComponent(typeof(Tool))]
public class Double_barreled_shotgun: MonoBehaviour
    ,IGun
{
    public bool aiming_automatically;
    
    public Transform left_muzzle;
    public Transform right_muzzle;
    
    public Transform shell_ejector;
    public int shooting_animation = Animator.StringToHash("shoot");
    
    public float fire_rate_delay;

    public Ammo_compatibility ammo_compatibility = Ammo_compatibility.shotgun;
    public int projectiles_amount = 6;
    public float spread_angle = 20;
    public float range = 20;
    public float range_randomness = 5;
    
    public Gun_shell bullet_shell_prefab;

    public Animator animator;
    
    public Tool tool;
    public Damage_dealer damage_dealer;
    public Reloadable reloadable;
    
    public LineRenderer line_renderer;
    private List<LineRenderer> line_renderers = new List<LineRenderer>();
    
    public Spark left_spark;
    private Spark right_spark;

    public GameObject hit_impact_prefab;

    
    private float last_shot_time = 0f;

    private IReceive_recoil recoil_receiver;

    private bool is_trigger_pulled;


    private void Awake() {
        if (!reloadable) {
            reloadable = GetComponent<Reloadable>();
        }
        init_line_renderers();
        init_sparks();
    }

    private void init_line_renderers() {
        line_renderers.Add(line_renderer);
        for (int i_shot = 0; i_shot < projectiles_amount*reloadable.max_ammo_qty-1; i_shot++) {
            line_renderers.Add(Instantiate(line_renderer,line_renderer.transform.parent));
        }
        for (int i_shot = 0; i_shot < projectiles_amount*reloadable.max_ammo_qty; i_shot++) {
            trail_fading.Add(1);
            line_renderers[i_shot].positionCount = 2;
        }
    }
    private void init_sparks() {
        left_spark.muzzle = left_muzzle;
        right_spark = Instantiate(left_spark, right_muzzle);
        right_spark.muzzle = right_muzzle;
        
    }

    public float recoil_force = 150f;



    private Transform get_active_muzzle() {
        if (reloadable.get_loaded_ammo() > 1) {
            return left_muzzle;
        }
        return right_muzzle;
    }

    public void pull_trigger() {
        if (
            (!is_trigger_pulled)
            &&
            (can_fire())
        )
        {
            fire(get_active_muzzle());
        }
        
        is_trigger_pulled = true;
    }

    public void release_trigger() {
        is_trigger_pulled = false;
    }

    public float emitting_randomisation = 0.05f;
    public AudioSource audio_source;
    public AudioClip shot_sound;
    private void fire(Transform muzzle) {
        
        last_shot_time = Time.time;
        reloadable.spend_ammo(1);
        
        recoil_receiver?.push_with_recoil(recoil_force);
        
        damage_dealer.set_attacker(tool.main_holding.holding_hand.arm.pair.user.transform);
        
        animator.SetTrigger(shooting_animation);
        create_spark(muzzle);
        audio_source?.PlayOneShot(shot_sound);
 
        
        float angle_step = spread_angle / projectiles_amount;
        float leftmost_direction = -spread_angle/2;
        for (int i_shot = 0; i_shot<projectiles_amount; i_shot++) {
            var perfect_shot_direction = leftmost_direction + i_shot * angle_step;
            var randomised_shot_direction = perfect_shot_direction - angle_step + Random.value * angle_step * 2;
            var absolute_shot_direction = muzzle.rotation * new Degree(randomised_shot_direction).to_quaternion();
            var impact_vector = absolute_shot_direction.to_vector();

            var emitting_point = 
                muzzle.position + 
                new Vector3(
                    -emitting_randomisation+Random.value*emitting_randomisation*2,
                    -emitting_randomisation+Random.value*emitting_randomisation*2
                );
            var randomized_range = range - range_randomness / 2 + Random.value * range_randomness;
            RaycastHit2D hit = Finding_objects.raycast(emitting_point, impact_vector, randomized_range);
            if (hit) {
                if (hit.transform.GetComponent<Damage_receiver>() is {} damage_receiver) {
                    damage_receiver.receive_damage(1);
                }
                if (hit.transform.GetComponent<IBleeding_body>() is {} bleeding_body) {
                    bleeding_body.create_splash(hit.point,impact_vector*10);
                } else {
                    damage_dealer.create_hit_impact(hit.point,hit.normal);
                }
                draw_trail(emitting_point, hit.point, i_shot);
            } else {
                var hit_point = emitting_point + (Vector3) absolute_shot_direction.to_vector() * randomized_range;
                draw_trail(emitting_point, hit_point, i_shot);
                Damaged_floor.instance.damage_point(hit_point, impact_vector);
            }
        }
    }

    
    private void create_spark(Transform muzzle) {
        if (muzzle == left_muzzle) {
            left_spark.activate_flash_detached();
        } else if (muzzle == right_muzzle) {
            right_spark.activate_flash_detached();
        }
    }
    
    private List<float> trail_fading = new List<float>(50);
    public float trail_lasting_seconds = 1;

    public float fading_difference = 0.2f;
    public void fade_trail(int i_trail) {
        var fading_randomness = (1 - fading_difference / 2) * Random.value + fading_difference;
        trail_fading[i_trail] += Time.deltaTime/trail_lasting_seconds*fading_randomness;
        Color start_color = Color.Lerp(new Color(1f, 1f, 1f , 1f), new Color(0f, 0f, 0f, 0f), trail_fading[i_trail]*1.5f);
        Color end_color = Color.Lerp(new Color(1f, 1f, 1f , 1f), new Color(0f, 0f, 0f, 0f), trail_fading[i_trail]);
        //Debug.Log(m_color);
        line_renderers[i_trail].startColor = start_color;
        line_renderers[i_trail].endColor = end_color;
    }

    public bool is_trail_visible(int i_trail) {
        return trail_fading[i_trail] < 1;
    }
    public void Update() {
        for (int i_trail=0;i_trail<projectiles_amount;i_trail++) {
            if (is_trail_visible(i_trail)) {
                fade_trail(i_trail);
            }
        }
    }

    // private Transform create_spark(Transform muzzle) {
    //     
    //     Projectile new_projectile = projectile_prefab.instantiate<Projectile>(
    //         muzzle.position, muzzle.rotation
    //     );
    //     
    //     Transform new_spark = spark.get_from_pool<Transform>();
    //     new_spark.copy_physics_from(muzzle);
    //     return new_projectile;
    // }
    
    private void draw_trail(Vector3 start, Vector3 end, int i_projectile) {
        //Debug.DrawLine(start, end, Color.green, 1f);
        var line_renderer = line_renderers[i_projectile];
        trail_fading[i_projectile] = 0;
        var list = new List<Vector3>() {start, end};
        line_renderer.SetPositions(list.ToArray());
    }

    private bool is_on_cooldown() {
        return Time.time - last_shot_time <= fire_rate_delay;
    }

    public bool can_fire() {
        return 
            reloadable.get_loaded_ammo() >0 &&
            !is_on_cooldown();
    }
    
    public bool is_aiming_automatically() {
        return aiming_automatically;
    }

    public bool is_ready_for_target(Transform target) {
        return Gun.is_muzzle_aimed_at_collider(get_active_muzzle(), target);
    }
    public void set_vertical_pointing(IGun.Vertical_pointing pointing) {
    }

    public Reloadable get_reloadable() {
        return reloadable;
    }

    
    
    /* invoked from an animation */
    [called_in_animation]
    private void eject_bullet_shell() {
        
        Gun_shell new_shell = bullet_shell_prefab.get_from_pool<Gun_shell>(
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
                left_muzzle.position, left_muzzle.right, targeted_targets
            );
        for (var i_target=0;i_target<targets_number;++i_target) {
            var hit = targeted_targets[i_target];
            if (hit.transform == in_target) {
                return true;
            }
        }

        return false;
    }
    
    
    
    
}
}