// using rvinowise.unity.geometry2d;
// using UnityEngine;
// using rvinowise.unity.extensions;
//
//
// namespace rvinowise.unity.actions {
//
// public class Hitting_with_limb2_impact: Action_leaf {
//
//     public Limb2 limb{ set; get; }
//     private ITransporter transporter;
//     private Transform target;
//     public Side_type impact_side { set; get; } = Side_type.NONE;
//
//     
//     public static Hitting_with_limb2_impact create(
//         Limb2 in_limb,
//         ITransporter in_transporter,
//         Transform in_target
//     ) {
//         var action = (Hitting_with_limb2_impact)pool.get(typeof(Hitting_with_limb2_impact));
//         action.add_actor(in_limb);
//         
//         action.limb = in_limb;
//         action.target = in_target;
//         action.transporter = in_transporter;
//         return action;
//     }
//     public Hitting_with_limb2_impact() {
//         
//     }
//     
//     
//
//
//     public override void update() {
//         base.update();
//         // limb.set_desired_directions_by_position(target.position);
//         // limb.move_segments_towards_desired_direction();
//         limb.segment1.change_rotation_speed(impact_side, float.MaxValue);
//         limb.segment2.change_rotation_speed(impact_side, float.MaxValue);
//         limb.segment1.update_rotation();
//         limb.segment2.update_rotation();
//         transporter.command_batch.face_direction_degrees = limb.transform.degrees_to(target.position);
//         keep_optimal_distance_from_target();
//         if (is_directed_towards_target()) {
//             var test = is_directed_towards_target();
//             mark_as_completed();
//         } else {
//             mark_as_not_completed();
//         }
//     }
//     
//     private void keep_optimal_distance_from_target() {
//         float distance_to_target = limb.transform.distance_to(target.position);
//         float reaching_distance = limb.get_reaching_distance();
//
//         if (distance_to_target > reaching_distance) {
//             transporter.command_batch.moving_direction_vector = 
//                 (target.position - limb.transform.position).normalized;
//         } else if (distance_to_target < reaching_distance * 0.7) {
//             transporter.command_batch.moving_direction_vector = 
//                 (limb.transform.position - target.position).normalized;
//         }
//     }
//     
//     
//     private bool is_directed_towards_target() {
//         var dir_to_target = get_direction_to_target();
//         var segment1_offset = Mathf.DeltaAngle(
//             limb.segment1.transform.get_degrees(),
//             dir_to_target
//         );
//         var segment2_offset = Mathf.DeltaAngle(
//             limb.segment2.transform.get_degrees(),
//             dir_to_target
//         );
//         return Mathf.Abs(segment1_offset + segment2_offset) <= 10;
//     }
//     
//     private float get_direction_to_target() {
//         return limb.transform.degrees_to(target.position);
//     }
// }
// }