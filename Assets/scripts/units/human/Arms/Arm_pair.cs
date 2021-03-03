
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;

using rvinowise.contracts;

using rvinowise.unity.units.parts.limbs.arms.actions;
using rvinowise.unity.units.parts.limbs.arms.actions.using_guns.reloading;
using rvinowise.unity.units.parts.actions;
using rvinowise.unity.units.parts.tools;
using rvinowise.unity.units.parts.transport;
using rvinowise.unity.units.parts.weapons.guns;
using Action = rvinowise.unity.units.parts.actions.Action;
using MoreLinq;
using rvinowise.unity.management;
using rvinowise.unity.extensions;
using rvinowise.unity.units.humanoid;
using rvinowise.unity.units.control;
using rvinowise.unity.ui.input;

namespace rvinowise.unity.units.parts.limbs.arms.humanoid {

public class Arm_pair:
    //Children_group
    MonoBehaviour
    ,IWeaponry
{



    #region IWeaponry interface
    public void fire() {
    }

    public void shoot(Transform target) {
        var fastest_weapon = get_weapon_reaching_faster(target);
        fastest_weapon.shoot(target);
    }

    private Held_tool get_weapon_reaching_faster(Transform target) {
        Held_tool fastest_tool = held_tools.MinBy(
            held_weapon => held_weapon.time_to_shooting(target)
        ).First();
        return fastest_tool;
    }
    #endregion


    #region Arm_controller itself
    
    public List<Held_tool> held_tools = new List<Held_tool>();

    public ITransporter transporter;

    public units.humanoid.Humanoid user;
    public Arm left_arm;
    
    public Arm right_arm;
    public float shoulder_span { get; set; }
    private Unit unit;
    

    void Awake() {
        unit = GetComponent<Unit>();
        user = GetComponent<Humanoid>();
        transporter = GetComponent<ITransporter>();
        left_arm.pair = right_arm.pair = this;
    }

    protected void Start() {
        init_directions();
        start_default_activity();
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

    private void start_default_activity() {
        /* left_arm.start_idle_action();
        right_arm.start_idle_action(); */
        Turning_element turning_element = GetComponent<Turning_element>();
        right_arm.set_root_action(
            Idle_vigilant_only_arm.create(
                right_arm,
                rvinowise.unity.ui.input.Player_input.instance.cursor.transform,
                transporter
            )
        );
        left_arm.set_root_action(
            Idle_vigilant_only_arm.create(
                left_arm,
                rvinowise.unity.ui.input.Player_input.instance.cursor.transform,
                transporter
            )
        );
    }

    public List<Arm> get_all_arms() {
        List<Arm> arms = new List<Arm>() {
            left_arm, right_arm
        };
        return arms;
    }

    void FixedUpdate() {
        
        
    }
   

    private Tool previous_tool;
    public void reload(Arm gun_arm) {
        Contract.Requires(gun_arm.held_tool is Gun, "reloaded arm must hold a gun");

        if (is_reloading_now(gun_arm)) {
            return;
        }
        
        Arm ammo_arm = other_arm(gun_arm);

        Action reloading_action = null;
        if (gun_arm.held_tool is Pistol pistol) {

            Ammunition magazine = user.baggage.get_magazine_for_gun(pistol);
            Contract.Requires(magazine != null);
            
            reloading_action = Reload_pistol.create(
                user,
                gun_arm,
                ammo_arm,
                user.baggage,
                pistol,
                magazine
            );
        } else if (gun_arm.held_tool is Pump_shotgun shotgun) {
            reloading_action = Reload_shotgun.create(
                user,
                gun_arm,
                ammo_arm,
                user.baggage,
                shotgun,
                user.baggage.get_magazine_for_gun(shotgun)
            );
        }
        
        previous_tool = ammo_arm.held_tool;
        user.set_root_action(
            Action_sequential_parent.create(
                reloading_action,
                /* rvinowise.unity.units.parts.actions.Action_parallel_parent.create(
                    actions.Aim_at_target.create(
                        gun_arm,
                        ammo_arm.transform,//test
                        user
                    ),
                    actions.Take_tool_from_bag.create(
                        ammo_arm,
                        user.baggage,
                        previous_tool
                    )
                ) */
                Action_parallel_parent.create(
                    Idle_vigilant_only_arm.create(gun_arm,gun_arm.attention_target, transporter),
                    Idle_vigilant_only_arm.create(ammo_arm,gun_arm.attention_target, transporter)
                )
            )
        );

    }

    public bool is_reloading_now(Arm weapon_holder) {
        //Arm ammo_taker = other_arm(weapon_holder); 
        if (
            (user.current_action is Action_sequential_parent sequential_parent)&&
            (sequential_parent.current_child_action is Reload_pistol action_reload_pistol)&&
            (action_reload_pistol.gun_arm == weapon_holder)
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

    public void aim_at(Transform in_target) {
        if (is_aiming_at(in_target)) {
            return;
        }
        Arm best_arm = null;
    if (get_free_arm_closest_to(in_target, typeof(Gun)) is Arm free_arm) {
            best_arm = free_arm;
        } else {
            best_arm = get_arm_closest_to(get_all_arms(), in_target, typeof(Gun));
            if (get_target_of(best_arm) is Transform old_target) {
                unsubscribe_from_disappearance_of(old_target);
            }
        }
        if (best_arm != null) {
            best_arm.aim_at(in_target);
            subscribe_to_disappearance_of(in_target);
        }
    }

    public void attack(Transform in_target) {
        if (get_arm_targeting(in_target) is Arm arm) {
            if (arm.held_tool is Gun gun) {
                gun.pull_trigger();
                on_ammo_changed(arm, gun.get_loaded_ammo());
            }
        }
    }

    public void set_target_for(Arm in_arm, Transform in_target) {
        if (get_target_of(in_arm) is Transform old_target) {
            unsubscribe_from_disappearance_of(old_target);
        }
        in_arm.aim_at(in_target);
        subscribe_to_disappearance_of(in_target);
    }

    public List<Transform> get_all_targets() {
        List<Transform> result = new List<Transform>();
        foreach (Arm arm in get_all_arms()) {
            if (arm.current_action is Aim_at_target aiming_action) {
                result.Add(aiming_action.get_target());
            }
        }
        return result;
    }

    private void subscribe_to_disappearance_of(Transform in_unit) {
        if (in_unit.GetComponent<Damage_receiver>() is Damage_receiver damage_receiver) {
            damage_receiver.on_destroyed+=handle_target_disappearence;
        }
    }
    private void unsubscribe_from_disappearance_of(Transform in_unit) {
        if (in_unit.GetComponent<Damage_receiver>() is Damage_receiver damage_receiver) {
            damage_receiver.on_destroyed-=handle_target_disappearence;
        }
    }

    private void handle_target_disappearence(Damage_receiver disappearing_unit) {
        foreach (Arm arm in get_all_arms()) {
            if (arm.current_action is Aim_at_target aiming_action) {
                if (aiming_action.get_target() == disappearing_unit.transform) {
                    on_target_disappeared(arm);
                    if (get_target_of(arm) == disappearing_unit.transform) {
                        arm.start_idle_action();
                    }
                }
            }
        }
    }

    public delegate void EventHandler(Arm arm);
    public event EventHandler on_target_disappeared;


    private Arm get_free_arm_closest_to(Transform in_target, System.Type needed_tool) {
        var free_arms = get_iddling_arms();
        return get_arm_closest_to(free_arms, in_target, needed_tool);
    }

    private Arm get_arm_closest_to(List<Arm> in_arms, Transform in_target, System.Type needed_tool) {
        float closest_distance = float.MaxValue;
        Arm closest_arm = null;
        foreach(Arm arm in in_arms) {
            if (arm.held_tool.GetType().IsSubclassOf(needed_tool)) {
                var this_distance = arm.get_aiming_distance_to(in_target);
                if (this_distance < closest_distance) {
                    closest_distance = this_distance;
                    closest_arm = arm;
                }
            }
        }
        return closest_arm;
    }

    public List<Arm> get_iddling_arms() {
        List<Arm> free_arms = new List<Arm>();
        if (right_arm.current_action?.GetType() == typeof(Idle_vigilant_only_arm)) {
            free_arms.Add(right_arm);
        }
        if (left_arm.current_action?.GetType() == typeof(Idle_vigilant_only_arm)) {
            free_arms.Add(left_arm);
        }
        return free_arms;
    }

    private bool is_aiming_at(Transform in_target) {
        return get_all_targets().IndexOf(in_target) > -1;
    }

     #endregion

    public Transform get_target_of(Arm in_arm) {
        if (in_arm.current_action is Aim_at_target aiming) {
            return aiming.get_target();
        }
        return null;
    }
    public Arm get_arm_targeting(Transform in_target) {
        if (
            (left_arm.current_action is Aim_at_target left_aiming)&&
            (left_aiming.get_target() == in_target)
        ) {
            return left_arm;
        } else if (
            (right_arm.current_action is Aim_at_target right_aiming)&&
            (right_aiming.get_target() == in_target) 
        ) {
            return right_arm;
        }
        return null;
    }

    public delegate void Handler_of_changing(Arm arm, int ammo);
    public event Handler_of_changing on_tools_changed;
    public event Handler_of_changing on_ammo_changed;

    void OnDrawGizmos() {
        if (Application.isPlaying) {
            var right_target = get_target_of(right_arm);
            if (right_target != null) {
                rvinowise.unity.debug.Debug.DrawLine_simple(
                    ui.input.Player_input.instance.cursor.transform.position, 
                right_target.transform.position,
                    Color.yellow,
                    3
                );
            }
            var left_target = get_target_of(left_arm);
            if (left_target != null) {
                rvinowise.unity.debug.Debug.DrawLine_simple(
                    ui.input.Player_input.instance.cursor.transform.position, 
                    left_target.transform.position,
                    Color.white,
                    3
                );
            }
        }
    }
}
}