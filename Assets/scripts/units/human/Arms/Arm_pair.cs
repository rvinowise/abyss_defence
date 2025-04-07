
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;

using rvinowise.contracts;

using rvinowise.unity.actions;
using rvinowise.unity.extensions.attributes;
using System;
using System.Linq;
//using static MoreLinq.Extensions.ToHashSetExtension;
using rvinowise.unity.extensions;
using rvinowise.unity.music;
using Action = rvinowise.unity.actions.Action;
using Transform = UnityEngine.Transform;


namespace rvinowise.unity {

public class Arm_pair:
    MonoBehaviour
    ,IAttacker
{

    #region IAttacker

    public bool is_weapon_ready_for_target(Transform in_target) {
        return 
            left_tool.GetComponent<Gun>() is {} left_gun && left_gun != null &&
            left_gun.is_ready_for_target(in_target)
            ||
            right_tool.GetComponent<Gun>() is {} right_gun && right_gun != null &&
            right_gun.is_ready_for_target(in_target)
            ;
    }

    public IEnumerable<Damage_receiver> get_targets() {
        return Enumerable.Empty<Damage_receiver>();
    }

    public float get_reaching_distance() {
        return float.MaxValue;
    }
    public void attack(Transform in_target, System.Action on_completed = null) {
        Arm_pair_shooting.attack(this, in_target,on_completed);
    }

    private Transform explicit_target;

    public Transform get_explicit_target() {
        return explicit_target;
    }
    public void set_explicit_target(Transform target) {
        explicit_target = target;
    }
    public void remove_explicit_target(Transform target) {
        if (explicit_target == target) {
            explicit_target = null;
        };
    }

    public bool is_arm_ready_to_fire(Arm arm) {
        return
            arm.actor.current_action?.GetType() == typeof(Idle_vigilant_only_arm)
            ||
            arm.actor.current_action?.GetType() == typeof(Aim_at_target);

    }

    public void stop_attacking() {
        foreach (var arm in arms) {
            if (arm.get_held_gun() is { } gun) {
                gun.release_trigger();
            }
        }
    }
     
    #endregion


    
    public Humanoid user;
    public Intelligence intelligence;
    public Arm left_arm;
    public Arm right_arm;
    public Baggage baggage;
    public Team team => intelligence.team;

    public GameObject transporter_object;
    public ITransporter transporter;
    
    private Toolset toolset_being_equipped;
    public float last_shot_time = 0;

    public Tool left_tool => left_arm.held_tool;
    public Tool right_tool => right_arm.held_tool;
    public float shoulder_span { get; set; }


    private Transform cursor_transform;

    public List<Arm> arms;
    void Awake() {
        transporter = transporter_object.GetComponent<ITransporter>();
        arms = new List<Arm>{left_arm,right_arm};
        intelligence = GetComponentInParent<Intelligence>();
    }

    protected void Start() {
        cursor_transform = Player_input.instance.cursor.transform;
        init_directions();
    }

    private void init_directions() {
        left_arm.shoulder.desired_idle_rotation = Directions.degrees_to_quaternion(90f);
        left_arm.upper_arm.desired_idle_rotation = Directions.degrees_to_quaternion(20f);
        left_arm.forearm.desired_idle_rotation = Directions.degrees_to_quaternion(-20f);
        left_arm.hand.desired_idle_rotation = Directions.degrees_to_quaternion(0f);

        right_arm.shoulder.desired_idle_rotation = Directions.degrees_to_quaternion(-90f);
        right_arm.upper_arm.desired_idle_rotation = Directions.degrees_to_quaternion(-20f);
        right_arm.forearm.desired_idle_rotation = Directions.degrees_to_quaternion(20f);
        right_arm.hand.desired_idle_rotation = Directions.degrees_to_quaternion(0f);
    }


    private void Update() {
        detect_selected_object();
        Arm_pair_aiming.aim_at_hinted_targets(this);
    }

    public bool is_on_cooldown() {
        return Time.time - last_shot_time <= (left_arm.get_held_gun() as Gun).fire_rate_delay;
    }
    
    private Ray ray;
    private RaycastHit hit;
    private void detect_selected_object() {
        set_explicit_target(null);
        
        if(
            Physics2D.OverlapPoint(
                Player_input.instance.mouse_world_position
            ) is {} collider
        ) {
            if (collider.transform.GetComponent<Intelligence>() is { } intelligence) {
                if (this.intelligence.team.is_enemy_team(intelligence.team)) {
                    set_explicit_target(intelligence.transform);
                }
            } else if (collider.transform.GetComponent<Targetable>() is { } targetable) {
                set_explicit_target(targetable.transform);
            }
        }
    }


    
    // public void wants_to_reload(Side_type in_side) {
    //
    //     Arm gun_arm = Arm_pair_helpers.get_arm_on_side(this, in_side);
    //     
    //     //Contract.Requires(gun_arm.held_tool is Gun, "reloaded arm must hold a gun");
    //
    //     if (!can_reload(gun_arm)) {
    //         return;
    //     }
    //     
    //     Arm ammo_arm = other_arm(gun_arm);
    //
    //     Action reloading_action = null;
    //     if (gun_arm.get_held_gun() is {} gun) {
    //
    //         Ammunition magazine = user.baggage.get_ammo_object_for_gun(gun);
    //         Contract.Requires(magazine != null);
    //         
    //         reloading_action = Reload_pistol.create(
    //             user.animator,
    //             gun_arm,
    //             ammo_arm,
    //             user.baggage,
    //             gun
    //         );
    //     } 
    //     // else if (gun_arm.held_tool is Pump_shotgun shotgun) {
    //     //     reloading_action = Reload_shotgun.create(
    //     //         user.animator,
    //     //         gun_arm,
    //     //         ammo_arm,
    //     //         user.baggage,
    //     //         shotgun,
    //     //         user.baggage.get_ammo_object_for_tool(shotgun)
    //     //     );
    //     // } else if (gun_arm.held_tool is Break_shotgun break_shotgun) {
    //     //     reloading_action = Reload_break_shotgun.create(
    //     //         user.animator,
    //     //         gun_arm,
    //     //         ammo_arm,
    //     //         user.baggage,
    //     //         break_shotgun,
    //     //         user.baggage.get_ammo_object_for_tool(break_shotgun)
    //     //     );
    //     // }
    //     
    //     Action_sequential_parent.create(
    //         reloading_action,
    //         Action_parallel_parent.create(
    //             Idle_vigilant_only_arm.create(gun_arm,gun_arm.attention_target, transporter),
    //             Idle_vigilant_only_arm.create(ammo_arm,gun_arm.attention_target, transporter)
    //         )
    //     ).start_as_root(actor.action_runner);
    //
    // }

    

    

    // private bool can_reload(Arm weapon_holder) {
    //     return 
    //         !is_reloading_now(weapon_holder)
    //         &&
    //         baggage.check_ammo_qty(weapon_holder.get_held_gun().ammo_compatibility) > 0;
    // }

    private bool is_reloading_now(Arm weapon_holder) {
        //Arm ammo_taker = other_arm(weapon_holder); 
        if (
            user.actor.current_action is Action_sequential_parent sequential_parent&&
            sequential_parent.current_child_action is Reload_pistol action_reload_pistol&&
            action_reload_pistol.gun_arm == weapon_holder
        )
        {
            return true;
        }
        
        return false;
    }


    public Arm other_arm(Arm in_arm) {
        Contract.Requires(in_arm == left_arm || in_arm == right_arm, "arm should be mine");
        if (in_arm == left_arm) {
            return right_arm;
        }
        return left_arm;
    }

    // public void aim_at(Transform in_target) {
    //     Debug.Log($"AIMING: Arm_pair.aim_at({in_target.name})");
    //     if (is_aiming_at(in_target)) {
    //         return;
    //     }
    //     Arm best_arm;
    //     if (get_free_arm_closest_to<Gun>(in_target) is {} free_arm) {
    //         best_arm = free_arm;
    //     } else {
    //         best_arm = get_arm_closest_to<Gun>(
    //             get_all_armed_autoaimed_arms(), 
    //             in_target
    //         );
    //         if (best_arm == null) {
    //             return;
    //         }
    //     }
    //     if (best_arm != null) {
    //         best_arm.start_aiming_at_target(in_target);
    //     }
    // }
    
    public void attack_with_arm(Side_type in_side) {
        Arm arm = Arm_pair_helpers.get_arm_on_side(this, in_side);
        if (arm.get_held_gun() is {} gun) {
            gun.pull_trigger();
        }
    }


    public void handle_target_disappearence(Intelligence disappearing_unit) {
        Debug.Log($"AIMING: handle_target_disappearence {disappearing_unit.gameObject.name}");
        foreach (Arm arm in Arm_pair_aiming.get_all_armed_autoaimed_arms(this)) {
            if (Arm_pair_aiming.get_target_of(arm) == disappearing_unit.transform) {
                Arm_pair_aiming.try_find_new_target(this,arm);
            }
        }
    }


    public delegate void EventHandler(Arm arm);
    //public event EventHandler on_target_disappeared;


    public Arm get_free_arm_closest_to<Tool_component>(Transform in_target) {
        var free_armed_arms = get_iddling_armed_autoaimed_arms();
        return get_arm_closest_to<Tool_component>(free_armed_arms, in_target);
    }

    public Arm get_arm_closest_to<Tool_component>(IEnumerable<Arm> in_arms, Transform in_target) {
        float closest_distance = float.MaxValue;
        Arm closest_arm = null;
        foreach(Arm arm in in_arms) {
            if (arm.held_tool.GetComponent<Tool_component>() is {}) {
                var this_distance = arm.get_aiming_distance_to(in_target);
                if (this_distance < closest_distance) {
                    closest_distance = this_distance;
                    closest_arm = arm;
                }
            }
        }
        return closest_arm;
    }

    private bool arm_is_ready_for_autoaiming(Arm in_arm) {
        return
            (right_arm.actor.current_action is Idle_vigilant_only_arm)
            ||
            (right_arm.actor.current_action == null);
    }

    public List<Arm> get_iddling_armed_autoaimed_arms() {
        List<Arm> free_armed_arms = new List<Arm>();
        if (
            arm_is_ready_for_autoaiming(right_arm)
            &&
            Arm_pair_aiming.is_arm_autoaimed(right_arm)
        )
        {
            free_armed_arms.Add(right_arm);
        }
        if (
            arm_is_ready_for_autoaiming(left_arm)
            &&
            Arm_pair_aiming.is_arm_autoaimed(left_arm)
        )
        {
            free_armed_arms.Add(left_arm);
        }
        return free_armed_arms;
    }

    

    public delegate void Handler_of_changing(Arm arm, int ammo);
    //public event Handler_of_changing on_tools_changed;
    public event Handler_of_changing on_ammo_changed = delegate{};

    public void raise_on_ammo_changed(Arm arm, int ammo) {
        on_ammo_changed(arm, ammo);
    }
            
#if UNITY_EDITOR
    void OnDrawGizmos() {
        if (Application.isPlaying) {
            var right_target = Arm_pair_aiming.get_target_of(right_arm);
            if (right_target != null) {
                rvinowise.unity.debug.Debug.DrawLine_simple(
                    Player_input.instance.cursor.transform.position, 
                right_target.transform.position,
                    Color.yellow,
                    3
                );
            }
            var left_target = Arm_pair_aiming.get_target_of(left_arm);
            if (left_target != null) {
                rvinowise.unity.debug.Debug.DrawLine_simple(
                    Player_input.instance.cursor.transform.position, 
                    left_target.transform.position,
                    Color.white,
                    3
                );
            }
        }
    }
#endif
    
    struct Arm_angles {
        Degree shoulder;
        Degree upper_arm;
        Degree forearm;
        Degree hand;

        public static Arm_angles get_from_arm(Arm in_arm) {
            return new Arm_angles {
                shoulder = new Degree(in_arm.shoulder.transform.localRotation).use_minus(),
                upper_arm = new Degree(in_arm.upper_arm.transform.localRotation).use_minus(),
                forearm = new Degree(in_arm.forearm.transform.localRotation).use_minus(),
                hand = new Degree(in_arm.hand.transform.localRotation).use_minus()
            };
        }

        public void apply_to_arm(Arm in_arm) {
            in_arm.shoulder.transform.localRotation = shoulder.to_quaternion();
            in_arm.upper_arm.transform.localRotation = upper_arm.to_quaternion();
            in_arm.forearm.transform.localRotation = forearm.to_quaternion();
            in_arm.hand.transform.localRotation = hand.to_quaternion();
        }

        public Arm_angles flipped()  {
            return new Arm_angles {
                shoulder = Quaternion.Inverse(shoulder.to_quaternion()),
                upper_arm = Quaternion.Inverse(upper_arm.to_quaternion()),
                forearm = Quaternion.Inverse(forearm.to_quaternion()),
                hand = Quaternion.Inverse(hand.to_quaternion())
            };
            // return new Arm_angles {
            //     shoulder = -shoulder,
            //     upper_arm = -upper_arm,
            //     forearm = -forearm,
            //     hand = -hand
            // };
        }
    }
    public void switch_arms_angles() {
        Arm_angles left_angles_orig = Arm_angles.get_from_arm(left_arm);
        Arm_angles right_angles_orig = Arm_angles.get_from_arm(right_arm);
        Arm_angles left_angles = Arm_angles.get_from_arm(left_arm).flipped();
        Arm_angles right_angles = Arm_angles.get_from_arm(right_arm).flipped();
        left_angles.apply_to_arm(right_arm);
        right_angles.apply_to_arm(left_arm);
    }

    [called_in_animation]
    void change_main_tool_animation(AnimationEvent in_event) {
        Arm arm = right_arm;
        /* if (user.is_flipped()) {
            arm = left_arm;
        } */
        Contract.Requires(
            arm.held_tool != null,
            "must hold a tool to change its animation"
        );
        Tool held_tool = arm.held_tool;
        switch (in_event.stringParameter) {
            case "sideview":
                held_tool.animator.SetBool(in_event.stringParameter, Convert.ToBoolean(in_event.intParameter));
                held_tool.transform.localScale = new Vector3(1,1,1);
                break;
            case "slide":
                held_tool.animator.SetTrigger(in_event.stringParameter);
                held_tool.animator.SetBool("sideview", false);
                break;
            
            case "opened":
                held_tool.animator.SetBool(in_event.stringParameter, Convert.ToBoolean(in_event.intParameter));
                break;
        }
        
    } 

    [called_in_animation]
    void eject_used_ammo_from_gun(AnimationEvent in_event) {
        
        Arm gun_arm = right_arm;
        /* if (user.is_flipped()) {
            gun_arm = left_arm;
        } */
        
        Contract.Requires(
            gun_arm.get_held_gun() != null,
            "must hold a gun"
        );
        // if (gun_arm.get_held_gun() is Break_sewedoff shotgun)
        // {
        //     shotgun.eject_shells();
        // }
    }
    

    public void switch_tools() {
        Holding_place left_holding = left_arm.held_part;
        Holding_place right_holding = right_arm.held_part;
        left_arm.drop_tool();
        right_arm.drop_tool();
        left_arm.grab_tool(right_holding);
        right_arm.grab_tool(left_holding);
    }
    
    

    // public void use_supertool() {
    //     Debug.Log($"AIMING: Arm_pair.use_supertool()");
    //     Fire_gun_till_empty.create();
    //     if (left_arm.get_held_gun() is {} gun) {
    //         gun.pull_trigger();
    //         on_ammo_changed(left_arm, gun.get_loaded_ammo());
    //     }
    // }   
    
    
    #region IActor

    public Actor actor { get; set; }

    public void on_lacking_action() {
        
    }

    #endregion
}
}