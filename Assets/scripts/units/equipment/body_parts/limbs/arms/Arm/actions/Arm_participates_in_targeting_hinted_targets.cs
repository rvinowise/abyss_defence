// using System;
// using rvinowise.unity.geometry2d;
// using UnityEngine;
// using rvinowise.unity.extensions;
// using rvinowise.contracts;
//
// namespace rvinowise.unity.actions {
//
// public class Arm_participates_in_targeting_hinted_targets: Action_of_arm {
//
//     
//     public static Arm_participates_in_targeting_hinted_targets create(
//         Arm in_arm
//     ) {
//         Contract.Assume(in_arm.get_held_gun() != null, "aiming arm should hold a gun");
//
//         var action = 
//             object_pool.get<Arm_participates_in_targeting_hinted_targets>();
//         action.add_actor(in_arm);
//         
//         action.arm = in_arm;
//         return action;
//     }
//     
//     
//
// }
// }