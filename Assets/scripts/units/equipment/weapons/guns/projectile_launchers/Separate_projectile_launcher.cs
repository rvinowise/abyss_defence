// using System;
// using System.Collections.Generic;
// using rvinowise.unity.extensions;
// using rvinowise.unity.extensions.attributes;
// using UnityEngine;
// using UnityEngine.Serialization;
// using Action = rvinowise.unity.actions.Action;
//
//
// namespace rvinowise.unity {
// public class Separate_projectile_launcher : //e.g., bazooka wich can be used with several rockets which are part of the weapon
//     MonoBehaviour,
//     IGun,IReloadable 
// {
//     public Rocket ballistic_rocket_prefab;
//     public Rocket homing_missile_prefab;
//     
//     public Dictionary<Ammo_compatibility, Transform> ammo_to_prefab = new Dictionary<Ammo_compatibility, Transform>();
//     
//     public Trajectory_flyer projectile_prefab;
//     public Rocket rocket;
//     public bool is_projectile_attached;
//     public Transform muzzle;
//     
//     
//     public Transform breech;
//     public Explosion launching_fire_prefab;
//
//     public Team team;
//     public Transform target;
//     
//     #region IGun interface
//
//     public UnityEngine.Events.UnityEvent on_launch_projectile;
//
//     public void set_target(Transform target) {
//         this.target = target;
//     }
//     
//     public void pull_trigger() {
//         if (is_projectile_attached) {
//             launch_projectile();
//         }
//     }
//
//     public Rocket get_rocket_prefab_from_ammo(Ammo_compatibility ammo) {
//         switch (ammo) {
//             case Ammo_compatibility.ballistic_rocket:
//                 return ballistic_rocket_prefab;
//             case Ammo_compatibility.homing_missile:
//                 return homing_missile_prefab;
//         }
//         return null;
//     }
//
//     public void launch_projectile() {
//         on_launch_projectile.Invoke();
//
//         activate_launching_fire();
//         
//         var landing_distance = muzzle.distance_to(Player_input.instance.mouse_world_position);
//         rocket.transform.SetParent(null,true);
//
//         if (rocket.GetComponent<Homing_missile>() is { } homing_missile) {
//             homing_missile.target = target;
//             Player_input.instance.unhighlight_target(target);
//         }
//         rocket.launch();
//         if (
//             (GetComponent<Parabolic_projectile_launcher>() is {} parabolic_launcher)
//             &&
//             (this.rocket.GetComponent<Trajectory_flyer>() is {} trajectory_flyer_rocket)
//         )
//         {
//             parabolic_launcher.launch_projectile(
//                 trajectory_flyer_rocket,
//                 landing_distance
//             );
//         }
//         is_projectile_attached = false;
//     }
//
//     private void activate_launching_fire() {
//         if (launching_fire_prefab != null) {
//             Instantiate(launching_fire_prefab, breech.transform.position, breech.transform.rotation);
//         }
//     }
//
//     public void release_trigger() {
//         
//     }
//
//     public bool can_fire() {
//         return is_projectile_attached;
//     }
//
//     #endregion IGun interface
//
//     
//
//     public int get_loaded_ammo() {
//         return is_projectile_attached? 1 : 0;
//     }
//
//     public int get_lacking_ammo() {
//         return is_projectile_attached? 0 : 1;
//     }
//
//     
//     public virtual void insert_ammunition(Ammo_compatibility ammo_type, int rounds_amount) {
//         if (rounds_amount == 1) {
//             var rocket_prefab = get_rocket_prefab_from_ammo(ammo_type);
//
//             rocket =
//                 Instantiate<Rocket>(
//                     rocket_prefab,
//                     muzzle
//                 );
//             rocket.transform.localPosition = -rocket.attachment.localPosition;
//
//             is_projectile_attached = true;
//
//             //on_ammo_changed();
//             if (rocket.GetComponent<Computer_intelligence>() is {} intelligence) {
//                 intelligence.set_team(team);
//                 intelligence.enabled = false;
//             }
//         }
//         
//     }
//     
//
//
// }
//
// }