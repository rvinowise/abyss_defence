using System;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Flying_transporter:
    MonoBehaviour
    ,ITransporter
{

    public Turning_element moved_body;
    private Rigidbody2D rigid_body;

    public float rotation_speed = 100f;
    public float rotation_slowing = 250;
    public float acceleration_speed = 1f;
    public float slowing_speed = 1f;
    
    public void set_moved_body(Turning_element body) {
        moved_body = body;
        rigid_body = moved_body.GetComponent<Rigidbody2D>();
        rigid_body.drag = 0.18f;
        rigid_body.angularDrag = 1f;
        moved_body.gameObject.layer = Keep_distance_from_target.flying_layer;
        moved_body.transform.set_z(-11);
    }

    public Turning_element get_moved_body() {
        return moved_body;
    }

    public Actor actor { get; set; }

    public void on_lacking_action() {
        Idle.create(this).start_as_root(actor.action_runner);
    }
    
    public float get_possible_rotation() {
        return rotation_speed;
    }
    public float get_possible_impulse() {
        return acceleration_speed;
    }
    public float get_possible_slowing() {
        return slowing_speed;
    }


    void Awake() {
        actor = GetComponent<Actor>();
    }
    

    private bool is_on_the_right_way;
    public void move_towards_destination(Vector2 destination) {
        //Vector2 delta_movement = (rvi.Time.deltaTime * possible_impulse * command_batch.moving_direction_vector );
        var moving_direction_vector = (destination - (Vector2) transform.position).normalized;
        
        is_on_the_right_way =
            Quaternion.Angle(moved_body.rotation, moving_direction_vector.to_quaternion()) 
            < 
            moved_body.position.distance_to(destination)*8
            ;

        var needed_forward_acceleration = get_possible_impulse();
        if (is_on_the_right_way) {
            rigid_body.AddForce(get_possible_impulse()*Physics_consts.rigidbody_impulse_multiplier*moved_body.rotation.to_vector());
        }
        else if (is_flying_forward()){
            rigid_body.AddForce(-get_possible_slowing()*Physics_consts.rigidbody_impulse_multiplier*moved_body.rotation.to_vector());
        }

        // var rotation_side =
        //     moved_body.rotation.degrees_to(command_batch.face_direction_quaternion).side(); 
            
        // Side.from_degrees(
        //     command_batch.face_direction_degrees - moved_body.transform.rotation.to_float_degrees()
        // );
        //rigid_body.AddTorque(rvi.Time.deltaTime *possible_rotation*(float)rotation_side);
        
        moved_body.rotation_acceleration = get_possible_rotation();
        moved_body.rotation_slowing = rotation_slowing;
        moved_body.set_target_rotation(moving_direction_vector.to_quaternion());
        moved_body.rotate_to_desired_direction();
    }

    private bool is_flying_forward() {
        return 
            Mathf.Abs(
                new Degree(rigid_body.velocity.to_dergees())
                    .angle_to(moved_body.rotation)
            ) 
            < 90f;
    }
    
    public void face_rotation(Quaternion rotation) {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        
        var line_length = 1f;
        Gizmos.color = Color.cyan;
        
        //Gizmos.DrawLine(moved_body.position, moved_body.position + Vector3.right.rotate((float)rotation_side * 45f).rotate(moved_body.rotation)  * line_length);

        if (!moved_body) return;
        
        var rotation_side =
            moved_body.rotation.degrees_to(moved_body.get_target_rotation()).side(); 
        
        rvinowise.unity.debug.Debug.DrawLine_simple(
            moved_body.position, 
            moved_body.position + Vector3.right.rotate((float)rotation_side * 45f).rotate(moved_body.rotation)  * line_length,
            Color.cyan,
            2
        );

        if (is_on_the_right_way) {
            rvinowise.unity.debug.Debug.DrawLine_simple(
                moved_body.position, 
                moved_body.position + moved_body.rotation*Vector3.right,
                Color.yellow,
                3
            );
        }
        else {
            rvinowise.unity.debug.Debug.DrawLine_simple(
                moved_body.position, 
                moved_body.position - moved_body.rotation*Vector3.right,
                Color.red,
                3
            );
        }
    }
#endif
    
}

}