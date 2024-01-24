
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
using rvinowise.unity.units.humanoid;
using rvinowise.unity.units.control;
using rvinowise.unity.extensions.attributes;
using System;

namespace rvinowise.unity.units.parts.limbs.arms.humanoid {

public class Arm_pair:
    MonoBehaviour
    ,IWeaponry
{

    #region IWeaponry interface

    public bool can_reach(Transform target) {
        return true;
    }
     public void attack(Transform in_target) {
        if (
            get_arm_targeting(in_target) is {held_tool: Gun gun} arm &&
            gun.can_fire() &&
            aimed_at_target(gun, in_target)) 
        {
            gun.pull_trigger();
            on_ammo_changed(arm, gun.get_loaded_ammo());
        }
    }
     
    #endregion


    #region Arm_controller itself
    
    public List<Held_tool> held_tools = new List<Held_tool>();
    private Toolset toolset_being_equipped;

    public ITransporter transporter;

    public units.humanoid.Humanoid user;
    public Intelligence intelligence;
    public Action_runner action_runner => intelligence.action_runner;
    private Baggage baggage;
    public Arm left_arm;
    
    public Arm right_arm;

    public Tool left_tool => left_arm.held_tool;
    public Tool right_tool => right_arm.held_tool;
    public float shoulder_span { get; set; }
    private Unit unit;
    

    void Awake() {
        unit = GetComponent<Unit>();
        user = GetComponent<Humanoid>();
        intelligence = GetComponent<Intelligence>();
        baggage = user.baggage;
        transporter = GetComponent<ITransporter>();
        
        init_arms();
    }

    private void init_arms() {
        left_arm.init();
        right_arm.init();
        left_arm.pair = right_arm.pair = this;
    }

    protected void Start() {
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
    

    public List<Arm> get_all_armed_autoaimed_arms() {
        List<Arm> arms = new List<Arm>();

        if (arm_is_autoaimed(right_arm))
        {
            arms.Add(right_arm);
        }
        if (arm_is_autoaimed(left_arm))
        {
            arms.Add(left_arm);
        }
        return arms;
    }


    public void wants_to_reload(Side in_side) {

        Arm gun_arm = get_arm_on_side(in_side);
        
        Contract.Requires(gun_arm.held_tool is Gun, "reloaded arm must hold a gun");

        if (!can_reload(gun_arm)) {
            return;
        }
        
        Arm ammo_arm = other_arm(gun_arm);

        Action reloading_action = null;
        if (gun_arm.held_tool is Pistol pistol) {

            Ammunition magazine = user.baggage.get_ammo_object_for_tool(pistol);
            Contract.Requires(magazine != null);
            
            reloading_action = Reload_pistol.create(
                user.animator,
                gun_arm,
                ammo_arm,
                user.baggage,
                pistol
            );
        } else if (gun_arm.held_tool is Pump_shotgun shotgun) {
            reloading_action = Reload_shotgun.create(
                user.animator,
                gun_arm,
                ammo_arm,
                user.baggage,
                shotgun,
                user.baggage.get_ammo_object_for_tool(shotgun)
            );
        } else if (gun_arm.held_tool is Break_shotgun break_shotgun) {
            reloading_action = Reload_break_shotgun.create(
                user.animator,
                gun_arm,
                ammo_arm,
                user.baggage,
                break_shotgun,
                user.baggage.get_ammo_object_for_tool(break_shotgun)
            );
        }
        
        Action_sequential_parent.create(
            reloading_action,
            Action_parallel_parent.create(
                Idle_vigilant_only_arm.create(gun_arm,gun_arm.attention_target, transporter),
                Idle_vigilant_only_arm.create(ammo_arm,gun_arm.attention_target, transporter)
            )
        ).start_as_root(action_runner);

    }

    

    public Arm get_arm_on_side(Side in_side) {
        if (in_side == Side.LEFT) {
            return left_arm;
        } else if (in_side == Side.RIGHT) {
            return right_arm;
        }
        return null;
    }

    private bool can_reload(Arm weapon_holder) {
        return 
            !is_reloading_now(weapon_holder)
            &&
            baggage.check_ammo_qty(weapon_holder.held_tool.ammo_compatibility) > 0;
    }

    private bool is_reloading_now(Arm weapon_holder) {
        //Arm ammo_taker = other_arm(weapon_holder); 
        if (
            user.current_action is Action_sequential_parent sequential_parent&&
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

    public void aim_at(Transform in_target) {
        if (is_aiming_at(in_target)) {
            return;
        }
        Arm best_arm = null;
        if (get_free_arm_closest_to(in_target, typeof(Gun)) is Arm free_arm) {
            best_arm = free_arm;
        } else {
            best_arm = get_arm_closest_to(
                get_all_armed_autoaimed_arms(), 
                in_target, 
                typeof(Gun)
            );
            if (best_arm == null) {
                return;
            }
            if (get_target_of(best_arm) is Transform old_target) {
                unsubscribe_from_disappearance_of(old_target);
            }
        }
        if (best_arm != null) {
            best_arm.aim_at(in_target);
            subscribe_to_disappearance_of(in_target);
        }
    }

    

    private bool aimed_at_target(Gun gun, Transform in_target) {
        RaycastHit2D hit = Physics2D.Raycast(
            gun.transform.position, 
            gun.muzzle.right
        );

        if (hit.transform == in_target) {
            return true;
        }

        return false;
    }

    

    public void attack_with_arm(Side in_side) {
        Arm arm = get_arm_on_side(in_side);
        if (arm.held_tool is Gun gun) {
            gun.pull_trigger();
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
        foreach (Arm arm in get_all_armed_autoaimed_arms()) {
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
        foreach (Arm arm in get_all_armed_autoaimed_arms()) {
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
        var free_armed_arms = get_iddling_armed_autoaimed_arms();
        return get_arm_closest_to(free_armed_arms, in_target, needed_tool);
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

    public List<Arm> get_iddling_armed_autoaimed_arms() {
        List<Arm> free_armed_arms = new List<Arm>();
        if (
            right_arm.current_action is Idle_vigilant_only_arm
            &&
            arm_is_autoaimed(right_arm)
        )
        {
            free_armed_arms.Add(right_arm);
        }
        if (
            left_arm.current_action is Idle_vigilant_only_arm 
            &&
            arm_is_autoaimed(left_arm)
        )
        {
            free_armed_arms.Add(left_arm);
        }
        return free_armed_arms;
    }

    private bool arm_is_autoaimed(Arm in_arm) {
        if (
            in_arm.held_tool is Gun r_gun
            &&
            r_gun.aiming_automatically
        ) {
            return true;
        }
        return false;
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
            left_arm.current_action is Aim_at_target left_aiming&&
            left_aiming.get_target() == in_target
        ) {
            return left_arm;
        } else if (
            right_arm.current_action is Aim_at_target right_aiming&&
            right_aiming.get_target() == in_target 
        ) {
            return right_arm;
        }
        return null;
    }

    public delegate void Handler_of_changing(Arm arm, int ammo);
    public event Handler_of_changing on_tools_changed;
    public event Handler_of_changing on_ammo_changed = delegate{};

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
            gun_arm.held_tool is Gun,
            "must hold a gun"
        );
        if (gun_arm.held_tool is Break_sewedoff shotgun)
        {
            shotgun.eject_shells();
        }
    }
    
    /* invoked from the animation (in keyframes).*/
    [called_in_animation]
    void apply_ammunition_to_gun(AnimationEvent in_event) {
        Arm gun_arm = right_arm;
        /* if (user.is_flipped()) {
            gun_arm = left_arm;
        } */
        Arm ammo_arm = other_arm(gun_arm);
        Contract.Requires(
            gun_arm.held_tool is Gun,
            "this arm must hold a gun to reload it"
        );
        Contract.Requires(
            ammo_arm.held_tool is Ammunition,
            "this arm must hold ammunition to reload"
        );

        Gun gun = gun_arm.held_tool as Gun;
        Ammunition ammo = ammo_arm.held_tool as Ammunition;
        gun.insert_ammunition(ammo);

    }

    public void switch_tools() {
        Holding_place left_holding = left_arm.held_part;
        Holding_place right_holding = right_arm.held_part;
        left_arm.drop_tool();
        right_arm.drop_tool();
        left_arm.grab_tool(right_holding);
        right_arm.grab_tool(left_holding);
    }
    
    public void attack() {
        if (left_arm.held_tool is Gun gun) {
            gun.pull_trigger();
            on_ammo_changed(left_arm, gun.get_loaded_ammo());
        }
    }

       
    
}
}