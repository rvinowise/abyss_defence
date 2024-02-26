//
//
//
// using rvinowise.contracts;
// using rvinowise.unity.geometry2d;
// using Unity.Mathematics;
// using UnityEngine;
//
//
// namespace rvinowise.unity.actions {
//
// public class Cover_body_with_shield: Action_leaf {
//
//
//     private ALeg shield;
//     private Transform body;
//     private Transform danger;
//
//     private float old_provided_impulse = 0;
//     
//     public static Cover_body_with_shield create(
//         ALeg leg,
//         Transform body
//     ) {
//         var action = (Cover_body_with_shield)pool.get(typeof(Cover_body_with_shield));
//         action.body = body;
//         
//         return action;
//     }
//     public Cover_body_with_shield() {
//         
//     }
//
//     protected override void on_start_execution() {
//         Debug.Log($"Hide_leg_inside_body::on_start_execution check that Leg is up: {leg}");
//         Contract.Assume(leg.is_up(), $"leg {leg.name} should be raised before starting Hide_leg_inside_body");
//
//         var hideable_leg = leg.GetComponent<Hideable_leg>();
//         var relative_rotation_before_hiding =
//             (hideable_leg.hiding_direction + new Degree(180f)).to_quaternion();
//         var pulling_speed = hideable_leg.pulling_speed;
//
//         var hiding_depth =
//             hideable_leg.hiding_depth;
//         
//         add_children(
//             Limb_rotate_segment_to_direction.create(
//                 leg,
//                 leg.segment1,
//                 body,
//                 relative_rotation_before_hiding
//             ),
//             Leg_pull_inside.create(
//                 leg,
//                 leg.segment1,
//                 body,
//                 pulling_speed,
//                 hiding_depth
//             ),
//             Limb_rotate_segment_to_direction.create(
//                 leg,
//                 leg.segment2,
//                 body,
//                 relative_rotation_before_hiding
//             ),
//             Leg_pull_inside.create(
//                 leg,
//                 leg.segment2,
//                 body,
//                 pulling_speed,
//                 hiding_depth
//             )
//         );
//
//         base.on_start_execution();
//     }
//
//     // protected override void restore_state() {
//     //     base.restore_state();
//     //     if (limb is ALeg leg) {
//     //         leg.provided_impulse = old_provided_impulse;
//     //     } 
//     // }
//
// }
// }