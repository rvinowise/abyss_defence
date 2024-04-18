// using System.Collections.Generic;
// using UnityEngine;
// using rvinowise.contracts;
// using rvinowise.unity.actions;
//
// namespace rvinowise.unity {
// public class Human_leg_group: 
//     Children_group
//     ,ITransporter
// {
//     
//     #region Children_group
//     public override IEnumerable<IChild_of_group> children  {
//         get { return legs; }
//     }
//
//     public override void add_child(IChild_of_group compound_object) {
//         var leg = compound_object as Humanoid_leg;
//         legs.Add(leg);
//         leg.transform.SetParent(transform, false);
//     }
//
//     #endregion Children_group
//
//     
//
//     #region ITransporter
//     
//     public float possible_impulse { get; set; }
//
//     private float calculate_possible_impulse() {
//         float impulse = 0;
//         foreach (ILeg leg in legs) {
//             if (!leg.is_up()) {
//                 impulse += leg.get_provided_impulse();
//             }
//         }
//         
//         return impulse;
//     }
//     
//
//     public float possible_rotation {
//         get { 
//             return GetComponent<Turning_element>().rotation_acceleration;
//         }
//         set{ Contract.Assert(false, "set possible_impulse instead");}
//     }
//
//
//
//     public Transporter_commands command_batch { get; } = new Transporter_commands();
//
//
//     public void move_in_direction_as_rigidbody(Vector2 moving_direction) {
//         Vector2 delta_movement = (Time.deltaTime * possible_impulse * moving_direction );
//         rigid_body.AddForce(delta_movement*10000f);
//     }
//     public void move_in_direction_as_transform(Vector2 moving_direction) {
//         Vector2 delta_movement = (Time.deltaTime * possible_impulse * moving_direction );
//         moved_unit.transform.position += (Vector3)delta_movement;
//     }
//     
//     public void rotate_to_direction(Quaternion face_direction) {
//
//         moved_unit.target_rotation = face_direction;
//         moved_unit.rotate_to_desired_direction();
//     }
//     
//
//     void Update() {
//         execute_commands();
//         move_legs();
//     }
//     
//     protected void execute_commands() {
//         move_in_direction_as_transform(command_batch.moving_direction_vector);
//         
//         UnityEngine.Debug.DrawLine(transform.position,(Vector2)transform.position+command_batch.moving_direction_vector.normalized*1,Color.magenta);
//         
//         UnityEngine.Debug.DrawLine(transform.position,(Vector2)transform.position+Vector2.right*possible_impulse,Color.black);
//         
//         rotate_to_direction(command_batch.face_direction_quaternion);
//     }
//     
//     
//     #endregion ITransporter
//     
//     
//     #region Human_legs itself
//     
//
//     public Turning_element moved_unit;
//
//
//     [SerializeField]
//     public List<Humanoid_leg> legs = new List<Humanoid_leg>();
//
//     private Rigidbody2D rigid_body;
//
//
//     protected override void Awake() {
//         base.Awake();
//         foreach (var leg in legs) {
//             leg.transform.parent = null;
//         }
//         possible_impulse = calculate_possible_impulse();
//     }
//     
//     public void raise_up(ILeg leg) {
//         Contract.Requires(!leg.is_up());
//         
//         leg.raise_up();
//         possible_impulse -= leg.get_provided_impulse();
//     }
//
//     
//     private bool can_move_without(ILeg in_leg) {
//         foreach (ILeg leg in legs) {
//             if (leg == in_leg) {
//                 continue; 
//             }
//             if (!leg.is_up()) {
//                 return true;
//             }
//         }
//         return false;
//     }
//     private void move_on_the_ground(ILeg leg) {
//         bool can_hold = leg.hold_onto_ground();
//         if (
//             (leg.is_twisted_badly())||
//             (!can_hold)
//         ) 
//         {
//             leg.draw_directions(Color.red);
//             raise_up(leg);
//         } 
//         else if (leg.is_twisted_uncomfortably()) {
//             if (can_move_without(leg)) {
//                 raise_up(leg);
//             }
//         }
//     }
//     
//     private void move_legs() {
//         foreach (ILeg leg in legs) {
//             leg.set_desired_position(get_optimal_position_for(leg)); 
//             if (leg.is_up()) {
//                 move_in_the_air(leg);
//             } else {
//                 move_on_the_ground(leg);
//             }
//         }
//     }
//     
//     private void move_in_the_air(ILeg leg) {
//         if (leg.has_reached_aim()) {
//             put_down(leg);
//         } else {
//             leg.move_segments_towards_desired_direction();
//         }
//     }
//
//     private void put_down(ILeg leg) {
//         Contract.Requires(leg.is_up());
//         leg.put_down();
//         possible_impulse += leg.get_provided_impulse();
//     }
//
//     
//     private Vector2 get_optimal_position_for(ILeg leg) {
//         Vector2 shift_to_moving_direction =
//             command_batch.moving_direction_vector * (leg.get_moving_offset_distance() * leg.transform.lossyScale.x);
//
//         return leg.get_optimal_position_standing() + 
//                shift_to_moving_direction;
//     }
//
//     
//
//
//     bool can_move() {
//         foreach (ILeg leg in legs) {
//             if (!leg.is_up()) {
//                 return true;
//             }
//         }
//         return false;
//     }
//
//     
//
//     public void OnDrawGizmos() {
//         foreach (ILeg leg in legs) {
//             if (leg != null) {
//                 if (Application.isPlaying) {
//                     leg.draw_positions();
//                     leg.draw_desired_directions();
//                 }
//                 leg.draw_directions(Color.white);
//             }
//         }
//     }
//     
//
//     #endregion Human_legs itself
//
//     
//
//     #region IActor
//     public Action current_action { get; set; }
//     private Action_runner action_runner { get; set; }
//     public void on_lacking_action() {
//         Idle.create(this).start_as_root(action_runner);
//     }
//     public void init_for_runner(Action_runner in_action_runner) {
//         this.action_runner = in_action_runner;
//     }
//     #endregion IActor
//     
// }
//
// }
//
