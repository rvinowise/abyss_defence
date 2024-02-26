// using UnityEngine;
// using rvinowise.unity;
//
//
// namespace rvinowise.unity.actions {
//
// public class Haymaker_with_creeping_leg: Action_sequential_parent {
//     
//     private static readonly int animation_haymaker = Animator.StringToHash("Base Layer.hitting_with_haymaker");
//
//     protected Animator animator;
//     private Creeping_leg_group leg_group;
//     private Transform target;
//     private Leg2 attacking_leg;
//     
//     public static Haymaker_with_creeping_leg create(
//         Animator in_animator,
//         Creeping_leg_group leg_group,
//         Transform in_target
//     ) {
//         var action = (Haymaker_with_creeping_leg)pool.get(typeof(Haymaker_with_creeping_leg));
//         action.animator = in_animator;
//         action.leg_group = leg_group;
//         action.target = in_target;
//         action.attacking_leg = leg_group.dominant_leg as Leg2;
//         action.init_child_actions();
//         return action;
//     }
//
//     private void init_child_actions() {
//         
//         add_child(
//             Limb2_reach_relative_directions.create_assuming_left_limb(
//                 attacking_leg,
//                 60,
//                 -90,
//                 leg_group.transform
//             )
//         );
//         add_child(
//             Play_recorded_animation.create(
//                 animator,
//                 animation_haymaker
//             )
//         );
//
//     }
//
//
//     protected override void on_start_execution() {
//         base.on_start_execution();
//         leg_group.ensure_leg_raised(attacking_leg);
//     }
//
//   
// }
// }