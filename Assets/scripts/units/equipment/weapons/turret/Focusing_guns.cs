using System;
using rvinowise.unity.actions;
using UnityEngine;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using UnityEngine.Serialization;
using Action = System.Action;


namespace rvinowise.unity {

public struct Paired_gun {
    public Turning_element turning_element;
    public IGun gun_interface;
    public Gun gun;

    public Paired_gun(Component gun_object) {
        turning_element = gun_object.GetComponent<Turning_element>();
        gun_interface = gun_object.GetComponent<IGun>();
        gun = gun_object.GetComponent<Gun>();
    }
} 

public class Focusing_guns:
    MonoBehaviour, 
    IAttacker,
    ITransporter
{

    public Turning_element platform;
    
    public Turning_element left_gun_turning;
    public Turning_element right_gun_turning;
    
    public Paired_gun left_gun;
    public Paired_gun right_gun;
    
    public Target target;
    public System.Action on_attack_completed;

    public Side_type prepared_gun = Side_type.LEFT;

    //public bool is_attacking;
    public Computer_intelligence intelligence;
    
    private void Awake() {
        left_gun = new Paired_gun(left_gun_turning);
        right_gun = new Paired_gun(right_gun_turning);
        pause_between_guns = (left_gun.gun.fire_rate_delay + right_gun.gun.fire_rate_delay) / 2;
        //intelligence = GetComponent<Computer_intelligence>();
    }

    public float get_possible_rotation() {
        return platform.rotation_acceleration;
    }

    public float get_possible_impulse() {
        return 0;
    }

    public void set_moved_body(Turning_element in_body) {
        
    }

    public Turning_element get_moved_body() {
        return platform;
    }

    public void move_towards_destination(Vector2 destination) {
    }

    public void face_rotation(Quaternion rotation) {
        platform.set_target_rotation(rotation);
    }

    public Actor actor { get; set; }

    public void on_lacking_action() {
        
    }

    public Paired_gun get_prepared_gun_to_shoot() {
        if (prepared_gun == Side_type.LEFT) {
            return left_gun;
        }
        return right_gun;
    }

    private float pause_between_guns;
    private float last_fire_moment;
    public void pull_trigger() {
        if (Time.time - last_fire_moment > pause_between_guns) {
            var shooting_gun = get_prepared_gun_to_shoot().gun;
            shooting_gun.pull_trigger();
            shooting_gun.release_trigger();
            prepared_gun = Side.flipped(prepared_gun);
            last_fire_moment = Time.time;
        }
    }

    public float shooting_range = 50f;
    public bool is_weapon_targeting_target(Transform target) {
        var gun = get_prepared_gun_to_shoot().gun;
        var is_target_close = gun.muzzle.sqr_distance_to(target.position) <= shooting_range;
        var is_directed_at_target = gun.is_aimed_at_collider(target);
        return is_target_close && is_directed_at_target;
    }

    public float get_reaching_distance() {
        return shooting_range;
    }

    public void attack(Transform target, Action on_completed = null) {
        this.target = new Target(target);
        on_attack_completed = on_completed;
        if (target.GetComponent<Intelligence>() is {} intelligence) {
            intelligence.on_destroyed += on_target_disappeared;
        }
    }

    public void attacking_step() {
        var direction_to_target = platform.transform.quaternion_to(target.position);
        platform.set_target_rotation(direction_to_target);
        
        if (is_weapon_targeting_target(target.transform)) {
            pull_trigger();
        }
    }
    

    public void focus_on_target(Target target) {
        var distance_to_target = platform.transform.distance_to(target.position);
        var distance_to_center = Math.Abs(left_gun_turning.localPosition.y);
        var focused_angle = Mathf.Atan(distance_to_center / distance_to_target) * Mathf.Rad2Deg;
        left_gun_turning.set_target_rotation(platform.rotation * new Degree(-focused_angle).to_quaternion());
        right_gun_turning.set_target_rotation(platform.rotation * new Degree(focused_angle).to_quaternion());
        left_gun_turning.rotate_to_desired_direction();
        right_gun_turning.rotate_to_desired_direction();
    }

    private void Update() {
        if (intelligence.intelligence_action == Intelligence_action.Attacking) {
            attacking_step();
        }
        if (target.intelligence != null) {
            focus_on_target(target);
        }
        platform.rotate_to_desired_direction();

    }

    public void on_target_disappeared(Intelligence disappeared_unit) {
        //Debug.Assert(disappeared_unit==target.intelligence, "the callback 'disappeared target' should be called with that target in parameters");
        on_attack_completed.Invoke();
    }
}

}