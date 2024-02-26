using System;
using rvinowise.unity.extensions;
using rvinowise.unity.geometry2d;
using rvinowise.unity.actions;
using UnityEngine;
using Action = rvinowise.unity.actions.Action;


namespace rvinowise.unity {

public class Wing_group:
    MonoBehaviour
    ,ITransporter 
{

    public Turning_element moved_body;
    
    public Action current_action { get; set; }
    public void on_lacking_action() {
        Action_sequential_parent.create(
            Move_towards_target.create(
                this,
                0,
                GameObject.FindWithTag("player")?.transform
            )
            //,meeting_action
            
        ).start_as_root(action_runner);
    }

    private Action_runner action_runner;
    public void init_for_runner(Action_runner in_action_runner) {
        action_runner = in_action_runner;
    }

    public float rotation_speed = 100f;
    public float acceleration_speed = 1f;
    public float possible_rotation {
        get=>rotation_speed;
        set { throw new System.NotImplementedException(); }
    }
    public float possible_impulse {
        get=>acceleration_speed;
        set { throw new System.NotImplementedException(); }
    }
    public Transporter_commands command_batch { get; } = new Transporter_commands();


    private Rigidbody2D rigid_body;
    void Awake() {
        rigid_body = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate() {
        execute_commands();
    }

    private bool is_on_the_right_way;
    protected void execute_commands() {
        //Vector2 delta_movement = (rvi.Time.deltaTime * possible_impulse * command_batch.moving_direction_vector );
        is_on_the_right_way =
            Quaternion.Angle(moved_body.rotation, command_batch.moving_direction_vector.to_quaternion()) 
            < 
            moved_body.position.distance_to(command_batch.get_target_position())*8
            ;

        var needed_forward_acceleration = possible_impulse;
        if (is_on_the_right_way) {
            //needed_acceleration = 0;
            rigid_body.AddForce(needed_forward_acceleration*Physics_consts.rigidbody_impulse_multiplier*moved_body.rotation.to_vector());
            rigid_body.drag = 0;
        }
        else {
            rigid_body.drag = possible_impulse*35;
        }

        // var rotation_side =
        //     moved_body.rotation.degrees_to(command_batch.face_direction_quaternion).side(); 
            
        // Side.from_degrees(
        //     command_batch.face_direction_degrees - moved_body.transform.rotation.to_float_degrees()
        // );
        //rigid_body.AddTorque(rvi.Time.deltaTime *possible_rotation*(float)rotation_side);
        
        moved_body.rotation_acceleration = possible_rotation;
        moved_body.target_rotation = command_batch.face_direction_quaternion;
        moved_body.rotate_to_desired_direction();
    }

    private void OnDrawGizmos() {
        
        var line_length = 1f;
        Gizmos.color = Color.cyan;
        
        //Gizmos.DrawLine(moved_body.position, moved_body.position + Vector3.right.rotate((float)rotation_side * 45f).rotate(moved_body.rotation)  * line_length);
        
        var rotation_side =
            moved_body.rotation.degrees_to(command_batch.face_direction_quaternion).side(); 
        
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
}

}