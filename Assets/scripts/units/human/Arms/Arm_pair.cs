
using System.Collections.Generic;
using rvinowise.unity.geometry2d;
using UnityEngine;

using rvinowise.contracts;

using rvinowise.unity.actions;
using rvinowise.unity.extensions.attributes;
using System;
using System.Linq;
using MoreLinq.Extensions;
using rvinowise.unity.extensions;
using Action = rvinowise.unity.actions.Action;
using Transform = UnityEngine.Transform;


namespace rvinowise.unity {

public class Arm_pair:
    MonoBehaviour
    ,IAttacker
{

    #region IWeaponry interface

    public bool can_reach(Transform in_target) {
        return 
            left_tool is Gun left_gun &&
            aimed_at_target(left_gun, in_target)
            ||
            right_tool is Gun right_gun &&
            aimed_at_target(right_gun, in_target)
            ;
    }

    public float get_reaching_distance() {
        return float.MaxValue;
    }
    public void attack(Transform in_target, System.Action on_completed = null) {
        Debug.Log($"AIMING: Arm_pair.attack({in_target.name})");
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
    
    public Humanoid user;
    public Intelligence intelligence;
    public Arm left_arm;
    public Arm right_arm;
    public Baggage baggage;
    public Team team;
    
    public GameObject transporter_object;
    public ITransporter transporter;
    
    public List<Held_tool> held_tools = new List<Held_tool>();
    private Toolset toolset_being_equipped;


    public Action_runner action_runner;// => action_runner;
    

    public Tool left_tool => left_arm.held_tool;
    public Tool right_tool => right_arm.held_tool;
    public float shoulder_span { get; set; }


    private Transform cursor_transform;

    private List<Arm> arms;
    void Awake() {
        transporter = transporter_object.GetComponent<ITransporter>();
        arms = new List<Arm>{left_arm,right_arm};
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
        aim_at_hinted_targets();
    }

    private void aim_at_hinted_targets() {
        var hinted_targets =
            get_enemies_closest_to(Player_input.instance.cursor.transform.position);

        var aiming_arms = get_arms_searching_for_targets();
        var targets_of_arms =
            aiming_arms
            .Select(arm => new {target = arm.get_target(), arm})
            .Where(p => p.target != null)
            .ToDictionary(p => p.target, p => p.arm);
        
        
        var needed_targets =
            hinted_targets.Take(aiming_arms.Count).Select(tuple => tuple.Item1).ToList();
        
        var current_targets =
            get_all_targets().ToHashSet();

        var needed_not_targeted_enemies =
            needed_targets.Except(current_targets).ToHashSet();

        var arms_changing_targets =
            aiming_arms
                .Where(arm => !needed_targets.Contains(arm.get_target()))
                .ToList();
        
        foreach (var target in needed_not_targeted_enemies) {
            get_arm_closest_to(arms_changing_targets, target, typeof(Gun))
                .aim_at(target);
        }
    }

    private List<Arm> get_arms_searching_for_targets() {
        return arms.Where(is_arm_searching_for_targets).ToList();
    }
    private bool is_arm_searching_for_targets(Arm in_arm) {
        return
            (arm_is_autoaimed(in_arm))
            &&
            (
                (in_arm.current_action == null)
                ||
                (in_arm.current_action.GetType() == typeof(Idle_vigilant_only_arm))
                ||
                (in_arm.current_action.GetType() == typeof(Aim_at_target))
            );
    }


    private List<Tuple<Transform,float>> get_enemies_closest_to(Vector2 in_position) {
        List<Tuple<Transform, float>> enemies_and_distances = new List<Tuple<Transform, float>>();
        foreach (var enemy in team.get_enemiy_transforms()) {
            var distance = enemy.transform.sqr_distance_to(in_position);
            enemies_and_distances.Add(new Tuple<Transform, float>(enemy,distance));
        }
        enemies_and_distances.Sort((tuple1,tuple2) => tuple1.Item2.CompareTo(tuple2.Item2) );
        return enemies_and_distances;
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
    
    public List<Tuple<Arm,Transform>> get_autoaimed_arms_with_targets() {
        List<Tuple<Arm,Transform>> arms = new List<Tuple<Arm, Transform>>();

        if (arm_is_autoaimed(right_arm))
        {
            arms.Add(new Tuple<Arm, Transform>(right_arm,right_arm.get_target()));
        }
        if (arm_is_autoaimed(left_arm))
        {
            arms.Add(new Tuple<Arm, Transform>(left_arm,left_arm.get_target()));
        }
        return arms;
    }


    public void wants_to_reload(Side_type in_side) {

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

    

    public Arm get_arm_on_side(Side_type in_side) {
        if (in_side == Side_type.LEFT) {
            return left_arm;
        } else if (in_side == Side_type.RIGHT) {
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
        Debug.Log($"AIMING: Arm_pair.aim_at({in_target.name})");
        if (is_aiming_at(in_target)) {
            return;
        }
        Arm best_arm;
        if (get_free_arm_closest_to(in_target, typeof(Gun)) is {} free_arm) {
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
        }
        if (best_arm != null) {
            best_arm.aim_at(in_target);
        }
    }


    private readonly RaycastHit2D[] targeted_targets = new RaycastHit2D[1000];
    private bool aimed_at_target(Gun gun, Transform in_target) {
        var targets_number = 
            Physics2D.RaycastNonAlloc(
                gun.transform.position, gun.muzzle.right, targeted_targets
            );
        for (var i_target=0;i_target<targets_number;++i_target) {
            var hit = targeted_targets[i_target];
            if (hit.transform == in_target) {
                return true;
            }
        }

        return false;
    }
    
    private bool directed_at_target(Gun gun, Transform in_target) {
        var vector_to_target =
            (in_target.position - gun.transform.position);
        
        var direction_to_tarrget =
            vector_to_target.to_dergees();

        var distance_to_target = vector_to_target.magnitude;

        var precision_angle = 10f;
        
        return 
            Math.Abs(
                gun.transform.rotation.to_degree().angle_to(direction_to_tarrget)
            ) 
            < 
            precision_angle/distance_to_target;


    }

    

    public void attack_with_arm(Side_type in_side) {
        Arm arm = get_arm_on_side(in_side);
        if (arm.held_tool is Gun gun) {
            gun.pull_trigger();
        }
    }

    public void set_target_for(Arm in_arm, Transform in_target) {
        in_arm.aim_at(in_target);
    }

    public List<Transform> get_all_targets() {
        List<Transform> result = new List<Transform>();
        foreach (Arm arm in get_all_armed_autoaimed_arms()) {
            if (arm.get_target() != null) {
                result.Add(arm.get_target());
            }
        }
        return result;
    }

   

    public void handle_target_disappearence(Intelligence disappearing_unit) {
        Debug.Log($"AIMING: handle_target_disappearence {disappearing_unit.gameObject.name}");
        foreach (Arm arm in get_all_armed_autoaimed_arms()) {
            if (get_target_of(arm) == disappearing_unit.transform) {
                try_find_new_target(arm);
            }
        }
    }
    
    public void try_find_new_target(Arm in_arm) {
        Debug.Log($"AIMING: try_find_new_target({in_arm.name})");
        List<Transform> free_enemies = get_not_targeted_enemies();
        Distance_to_component closest_target = Object_finder.instance.get_closest_object(
            cursor_transform.position,
            free_enemies
        );
        if (closest_target.get_transform() != null) {
            set_target_for(in_arm, closest_target.get_transform());
        }
        else {
            in_arm.start_idle_action();
        }
    }
    private List<Transform> get_not_targeted_enemies() {
        return team.get_enemiy_transforms().Except(get_all_targets()).ToList();
    }

    public delegate void EventHandler(Arm arm);
    public event EventHandler on_target_disappeared;


    private Arm get_free_arm_closest_to(Transform in_target, System.Type needed_tool) {
        var free_armed_arms = get_iddling_armed_autoaimed_arms();
        return get_arm_closest_to(free_armed_arms, in_target, needed_tool);
    }

    private Arm get_arm_closest_to(IEnumerable<Arm> in_arms, Transform in_target, System.Type needed_tool) {
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

    private bool arm_is_ready_for_autoaiming(Arm in_arm) {
        return
            (right_arm.current_action is Idle_vigilant_only_arm)
            ||
            (right_arm.current_action == null);
    }

    public List<Arm> get_iddling_armed_autoaimed_arms() {
        List<Arm> free_armed_arms = new List<Arm>();
        if (
            arm_is_ready_for_autoaiming(right_arm)
            &&
            arm_is_autoaimed(right_arm)
        )
        {
            free_armed_arms.Add(right_arm);
        }
        if (
            arm_is_ready_for_autoaiming(left_arm)
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
                    Player_input.instance.cursor.transform.position, 
                right_target.transform.position,
                    Color.yellow,
                    3
                );
            }
            var left_target = get_target_of(left_arm);
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
        Debug.Log($"AIMING: Arm_pair.attack()");
        if (left_arm.held_tool is Gun gun) {
            gun.pull_trigger();
            on_ammo_changed(left_arm, gun.get_loaded_ammo());
        }
    }

       
    
}
}