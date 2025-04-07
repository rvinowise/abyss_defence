// using System.Linq;
// using rvinowise.unity.actions;
// using rvinowise.unity.extensions;
// using UnityEngine;
// using rvinowise.unity.geometry2d;
//
//
// namespace rvinowise.unity {
//
// public class Bazooka_with_ballistic_rocket: 
//     Supertool_description
// {
//
//     public Ammo_compatibility ammo_compatibility;
//     
//     
//     private IGun gun;
//     private IReloadable reloadable;
//     public Separate_projectile_launcher launcher;
//
//     private void Awake() {
//         gun = tool.GetComponent<IGun>();
//         reloadable = tool.GetComponent<IReloadable>();
//     }
//     
//
//     public override void start_using_action(Humanoid user) {
//
//         if (user.GetComponent<Player_human>() is { } intelligence) {
//             var hinted_targets = 
//                 intelligence.team.get_enemies_closest_to(Player_input.instance.cursor.transform.position);
//             
//             Transform best_target;
//             if (hinted_targets.Any()) {
//                 best_target = hinted_targets.First().Item1;
//             }
//             else {
//                 best_target = Player_input.instance.cursor.transform;
//             }
//             
//             launcher.set_target(best_target);
//             Player_input.instance.highlight_target(best_target);
//         }
//         
//         Fire_gun_till_empty.create(
//             user,
//             gun,
//             tool,
//             reloadable,
//             ammo_compatibility
//         ).start_as_root(user.actor.action_runner);
//     }
// }
// }