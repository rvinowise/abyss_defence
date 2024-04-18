// using UnityEngine;
// using rvinowise.unity.extensions;
//
//
// namespace rvinowise.unity {
//
// public class Player_spider: Intelligence {
//
//     protected override void read_input() {
//         read_transporter_input();
//     }
//
//     
//
//     private void read_transporter_input() {
//         if (transporter == null) {
//             return;
//         }
//         transporter.command_batch.moving_direction_vector = read_moving_direction();
//         transporter.command_batch.face_direction_quaternion = read_face_direction();
//
//         Vector2 read_moving_direction() {
//             Vector2 direction_vector = Player_input.instance.moving_vector;
//             return direction_vector.normalized;
//         }
//
//         Quaternion read_face_direction() {
//             Vector2 mousePos = Player_input.instance.mouse_world_position;
//             Quaternion needed_direction = (mousePos - (Vector2) transform.position).to_quaternion();
//
//             save_last_rotation(needed_direction);
//
//             return needed_direction;
//         }
//     }
//
// }
// }